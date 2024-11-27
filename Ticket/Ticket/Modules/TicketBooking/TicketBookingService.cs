using MongoDB.Driver;
using Ticket.Helpers;

namespace Ticket.Modules.TicketBooking;

public class TicketBookingService : ITicketBookingService
{
    private readonly IMongoCollection<Shared.Models.Ticket> _ticketCollection;
    private readonly IRedisCacheHelper _cacheHelper;
    private readonly IKafkaHelper _kafkaHelper;

    public TicketBookingService(IMongoDatabase database, IRedisCacheHelper cacheHelper, IKafkaHelper kafkaHelper)
    {
        _ticketCollection = database.GetCollection<Shared.Models.Ticket>("Tickets");
        _cacheHelper = cacheHelper;
        _kafkaHelper = kafkaHelper;
    }

    public async Task<bool> BookTicket(string trainId, DateTime date, string seatNumber, string userId)
    {
        string cacheKey = $"SeatStatus:{trainId}:{date:yyyyMMdd}:{seatNumber}";
        var lockKey = $"Lock:{cacheKey}";

        if (!await _cacheHelper.AcquireLockAsync(lockKey,lockKey, TimeSpan.FromSeconds(5)))
            return false; // Nếu không lock được, trả về thất bại.

        try
        {
            var isAvailable = await _cacheHelper.GetAsync<bool?>(cacheKey);
            if (isAvailable.HasValue && !isAvailable.Value)
                return false;

            // Đặt vé
            var ticket = new Shared.Models.Ticket
            {
                TrainId = trainId,
                Date = date,
                SeatNumber = seatNumber,
                UserId = userId,
                BookingTime = DateTime.UtcNow
            };

            await _ticketCollection.InsertOneAsync(ticket);

            // Gửi sự kiện đặt vé vào Kafka
            await _kafkaHelper.ProduceAsync("ticket_booking", new { TrainId = trainId, Date = date, SeatNumber = seatNumber, UserId = userId });

            // Cập nhật cache
            await _cacheHelper.SetAsync(cacheKey, false, TimeSpan.FromMinutes(10)); // Đặt trạng thái là đã đặt
            return true;
        }
        finally
        {
            await _cacheHelper.ReleaseLockAsync(lockKey);
        }
    }
}