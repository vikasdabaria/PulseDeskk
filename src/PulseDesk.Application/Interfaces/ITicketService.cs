using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseDesk.Application.DTOs.Tickets;

namespace PulseDesk.Application.Interfaces
{
    public interface ITicketService
    {
        Task<TicketResponse> CreateTicketAsync(CreateTicketRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TicketResponse>> GetTicketsAsync(CancellationToken cancellationToken = default);
        Task<TicketResponse?> GetTicketByIdAsync(int id, CancellationToken cancellationToken = default);
    }
}
