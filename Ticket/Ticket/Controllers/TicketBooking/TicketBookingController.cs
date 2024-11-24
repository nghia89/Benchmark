using Microsoft.AspNetCore.Mvc;

namespace Ticket.Modules.TicketBooking;

[ApiController]
[Route("api/ticket-booking")]
public class TicketBookingController : ControllerBase
{
    private readonly ITicketBookingService _ticketBookingService;

    public TicketBookingController(ITicketBookingService ticketBookingService)
    {
        _ticketBookingService = ticketBookingService;
    }

    [HttpPost("book")]
    public async Task<IActionResult> BookTicket([FromBody] BookingRequest request)
    {
        var isBooked = await _ticketBookingService.BookTicket(request.TrainId, request.Date, request.SeatNumber, request.UserId);
        if (isBooked)
            return Ok(new { Message = "Ticket booked successfully" });
        return BadRequest(new { Message = "Failed to book ticket" });
    }
}

public class BookingRequest
{
    public string TrainId { get; set; }
    public DateTime Date { get; set; }
    public string SeatNumber { get; set; }
    public string UserId { get; set; }
}