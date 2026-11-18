using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulseDesk.Application.DTOs.Tickets;
using PulseDesk.Application.Interfaces;
using PulseDesk.Domain.Entities;
using PulseDesk.Domain.Enums;
using PulseDesk.infrastructure.Data;

namespace PulseDesk.infrastructure.Authentication
{
    public sealed class TicketService : ITicketService
    {
        private readonly PulseDeskDbContext _context;
        private readonly ICurrentTenantService _currentTenantService;

        public TicketService(PulseDeskDbContext context, ICurrentTenantService currentTenantService)
        {
            _context = context;
            _currentTenantService = currentTenantService;
        }

        public async Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationtoken)
        {
            var userId = _currentTenantService.UserId
               ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var ticket = new Ticket
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                Status = TicketStatus.Open,
                CreatedByUserId = userId
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var creatorName = await _context.Users.Where(u => u.Id == userId).Select(u => u.FullName).FirstOrDefaultAsync(cancellationtoken) ?? "Unknown";

            return new TicketResponse(ticket.Id,
                ticket.Title,
                ticket.Description,
                ticket.Status.ToString(),
                ticket.Priority.ToString(),
                ticket.CreatedByUserId,
                creatorName,
                ticket.AssignedToUserId,
                null,
                ticket.CreatedAtUtc);
        }

        public async Task<IReadOnlyList<TicketResponse>> GetTicketsAsync(CancellationToken cancellationtoken = default)
        {
            return await _context.Tickets.AsNoTracking().Include(t => t.CreatedByUser).Include(t => t.AssignedToUser).OrderByDescending(t => t.CreatedAtUtc).Select(t => new
            TicketResponse(t.Id, t.Title, t.Description, t.Status.ToString(), t.Priority.ToString(), t.CreatedByUserId, t.CreatedByUser != null ? t.CreatedByUser.FullName : "Unknown", t.AssignedToUserId, t.AssignedToUser != null ? t.AssignedToUser.FullName : null, t.CreatedAtUtc)
            ).ToListAsync(cancellationtoken);
        }

        public async Task<TicketResponse?> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Tickets.AsNoTracking().Include(t => t.CreatedByUser).Include(t => t.AssignedToUser).Where(t => t.Id == id).Select(t => new TicketResponse(
                    t.Id,
                    t.Title,
                    t.Description,
                    t.Status.ToString(),
                    t.Priority.ToString(),
                    t.CreatedByUserId,
                    t.CreatedByUser != null ? t.CreatedByUser.FullName : "Unknown",
                    t.AssignedToUserId,
                    t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                    t.CreatedAtUtc
                )).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
