using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseDesk.Domain.Common;
using PulseDesk.Domain.Enums;

namespace PulseDesk.Domain.Entities
{
    public sealed class User : BaseEntity, ITenantEntity
    {
        public int TenantId { get; set; }
        public Tenant? Tenant { get; set; }

        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Client;
        public bool IsActive { get; set; } = true;
    }
}
