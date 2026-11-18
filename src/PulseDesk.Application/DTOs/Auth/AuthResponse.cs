using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseDesk.Application.DTOs.Auth
{
    public sealed record AuthResponse
    (int UserId,
    string Email,
    string FullName,
    string Role,
    int TenantId,
    string Token);
}
