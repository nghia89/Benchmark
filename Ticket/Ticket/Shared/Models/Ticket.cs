using MongoDB.Bson;

namespace Ticket.Shared.Models;

public class Ticket
{
    public ObjectId Id { get; set; }
    public string TrainId { get; set; }
    public DateTime Date { get; set; }
    public string SeatNumber { get; set; }
    public string UserId { get; set; }
    public DateTime BookingTime { get; set; }
}