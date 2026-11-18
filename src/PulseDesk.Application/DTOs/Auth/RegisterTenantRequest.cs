using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseDesk.Application.DTOs.Auth
{
    public sealed record RegisterTenantRequest
    (
string CompanyName,
string CompanyCode,
string AdminFullName,
string AdminEmail,
string AdminPassword
);
}
