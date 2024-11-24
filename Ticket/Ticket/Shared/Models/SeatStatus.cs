namespace Ticket.Shared.Models;

public class SeatStatus
{
    public string TrainId { get; set; }
    public DateTime Date { get; set; }
    public string SeatNumber { get; set; }
    public bool IsAvailable { get; set; }
}