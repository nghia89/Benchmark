namespace Ticket.Modules.SeatChecking;

public interface ISeatCheckingService
{
    Task<bool> IsSeatAvailable(string trainId, DateTime date, string seatNumber);
}