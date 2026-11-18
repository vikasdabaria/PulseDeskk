using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseDesk.Application.Interfaces
{
    public interface ICurrentTenantService
    {
        int? TenantId { get; }
        int? UserId { get; }
        string? Role { get; }
    }
}
