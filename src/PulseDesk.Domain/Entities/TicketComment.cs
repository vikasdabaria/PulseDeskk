using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseDesk.Domain.Common;

namespace PulseDesk.Domain.Entities
{
    public sealed class TicketComment : BaseEntity, ITenantEntity
    {
        public int TenantId { get; set; }
        public Tenant? Tenant { get; set; }
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsInternalNote { get; set; } = false;
    }
}
