using PulseDesk.Domain.Common;
using PulseDesk.Domain.Enums;

namespace PulseDesk.Domain.Entities
{
    public sealed class Ticket : BaseEntity, ITenantEntity
    {
        public int TenantId { get; set; }
        public Tenant? Tenant { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketStatus Status { get; set; } = TicketStatus.Open;
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
        // Relations (Foreign Keys)
        public int CreatedByUserId { get; set; }
        public User? CreatedByUser { get; set; }
        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }
        public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    }
}
