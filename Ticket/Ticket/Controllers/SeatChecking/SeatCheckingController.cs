using Microsoft.AspNetCore.Mvc;

namespace Ticket.Modules.SeatChecking;

[ApiController]
[Route("api/seat-checking")]
public class SeatCheckingController : ControllerBase
{
    private readonly ISeatCheckingService _seatCheckingService;

    public SeatCheckingController(ISeatCheckingService seatCheckingService)
    {
        _seatCheckingService = seatCheckingService;
    }

    [HttpGet("is-available")]
    public async Task<IActionResult> IsSeatAvailable(
        [FromQuery] string trainId,
        [FromQuery] DateTime date,
        [FromQuery] string seatNumber)
    {
        var isAvailable = await _seatCheckingService.IsSeatAvailable(trainId, date, seatNumber);
        return Ok(new { SeatNumber = seatNumber, IsAvailable = isAvailable });
    }
}