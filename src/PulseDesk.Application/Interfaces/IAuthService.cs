using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PulseDesk.Application.DTOs.Auth;

namespace PulseDesk.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterTenantAsync(RegisterTenantRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    }
}
