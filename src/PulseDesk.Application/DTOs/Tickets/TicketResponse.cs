using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseDesk.Application.DTOs.Tickets
{
    public sealed record TicketResponse
    (int Id,
        string Title,
        string Description,
        string Status,
        string Priority,
        int CreatedByUserId,
        string CreatedByUserName,
        int? AssignedToUserId,
        string? AssignedToUserName,
        DateTime CreatedAtUtc
    );
}
