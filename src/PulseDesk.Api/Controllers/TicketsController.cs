using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseDesk.Application.DTOs.Tickets;
using PulseDesk.Application.Interfaces;
using PulseDesk.infrastructure.Authentication;

namespace PulseDesk.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _ticketService.CreateTicketAsync(request, cancellationToken);

                return CreatedAtAction(nameof(GetTicketById), new { id = response.Id }, response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets(CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetTicketsAsync(cancellationToken);
            return Ok(tickets);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTicketById(int id, CancellationToken cancellationToken)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id, cancellationToken);
            if (ticket == null)
            {
                return NotFound(new { message = $"Ticket with ID {id} was not found." });
            }
            return Ok(ticket);
        }
    }
}
