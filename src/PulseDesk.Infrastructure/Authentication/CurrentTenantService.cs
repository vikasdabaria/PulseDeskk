using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PulseDesk.Application.Interfaces;

namespace PulseDesk.infrastructure.Authentication
{
    public sealed class CurrentTenantService : ICurrentTenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? TenantId
        {
            get
            {
                var tenantClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("TenantId")?.Value;
                return int.TryParse(tenantClaim, out var id) ? id : null;
            }
        }

        public int? UserId
        {
            get
            {
                var userClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userClaim, out var id) ? id : null;
            }
        }
        public string? Role
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
            }
        }
    }
}
