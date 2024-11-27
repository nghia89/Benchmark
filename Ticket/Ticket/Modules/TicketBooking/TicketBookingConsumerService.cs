namespace Ticket.Modules.TicketBooking;

public interface ITicketBookingConsumerService
{
    public Task HandleMessageAsync(string message);
}

public class TicketBookingConsumerService : ITicketBookingConsumerService
{
    public TicketBookingConsumerService()
    {
    }

    public async Task HandleMessageAsync(string message)
    {
        Console.WriteLine($"Message: {message}");
    }
}