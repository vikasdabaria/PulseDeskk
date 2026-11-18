using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseDesk.Domain.Enums;

namespace PulseDesk.Application.DTOs.Auth
{
    public sealed record RegisterRequest
    (string FullName,string Email,string Password,int TenantId,UserRole Role);
}
