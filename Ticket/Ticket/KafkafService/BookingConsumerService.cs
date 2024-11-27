using Ticket.Helpers;
using Ticket.Modules.TicketBooking;

namespace Ticket.BackgroundService;

public class BookingConsumerService : IKafkaConsumerService
{
    private readonly IKafkaHelper _kafkaHelper;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BookingConsumerService(IKafkaHelper kafkaHelper, IServiceScopeFactory serviceScopeFactory)
    {
        _kafkaHelper = kafkaHelper;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task StartConsumingAsync(CancellationToken stoppingToken)
    {
        string topic = "ticket_booking";
        string groupId = "booking_group";

        await _kafkaHelper.ConsumeAsync(topic, groupId, async (message) =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var scopedService = scope.ServiceProvider.GetRequiredService<ITicketBookingConsumerService>();
            await scopedService.HandleMessageAsync(message);
        }, stoppingToken);
    }
}
