using PulseDesk.Domain.Common;

namespace PulseDesk.Domain.Entities
{
    public sealed class Tenant : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
