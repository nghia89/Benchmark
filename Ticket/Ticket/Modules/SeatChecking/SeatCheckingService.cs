using MongoDB.Driver;
using Ticket.Helpers;
using Ticket.Shared.Models;

namespace Ticket.Modules.SeatChecking;

public class SeatCheckingService : ISeatCheckingService
{
    private readonly IMongoCollection<SeatStatus> _seatStatusCollection;
    private readonly IRedisCacheHelper _cacheHelper;

    public SeatCheckingService(IMongoDatabase database, IRedisCacheHelper cacheHelper)
    {
        _seatStatusCollection = database.GetCollection<SeatStatus>("SeatStatus");
        _cacheHelper = cacheHelper;
    }

    public async Task<bool> IsSeatAvailable(string trainId, DateTime date, string seatNumber)
    {
        string cacheKey = $"SeatStatus:{trainId}:{date:yyyyMMdd}:{seatNumber}";
        var cachedValue = await _cacheHelper.GetAsync<bool?>(cacheKey);
        if (cachedValue.HasValue)
            return cachedValue.Value;

        var seat = await _seatStatusCollection
            .Find(x => x.TrainId == trainId && x.Date == date && x.SeatNumber == seatNumber)
            .FirstOrDefaultAsync();

        bool isAvailable = seat?.IsAvailable ?? false;
        await _cacheHelper.SetAsync(cacheKey, isAvailable, TimeSpan.FromMinutes(10)); 
        return isAvailable;
    }
}