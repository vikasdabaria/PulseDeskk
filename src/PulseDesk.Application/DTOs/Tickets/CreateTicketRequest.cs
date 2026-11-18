using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseDesk.Domain.Enums;

namespace PulseDesk.Application.DTOs.Tickets
{
    public sealed record CreateTicketRequest
    (string Title,
        string Description,
        TicketPriority Priority);
}
