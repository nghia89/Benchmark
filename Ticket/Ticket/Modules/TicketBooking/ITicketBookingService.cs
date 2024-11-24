namespace Ticket.Modules.TicketBooking;

public interface ITicketBookingService
{
    Task<bool> BookTicket(string trainId, DateTime date, string seatNumber, string userId);
}