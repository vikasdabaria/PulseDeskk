using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PulseDesk.Application.DTOs.Auth;
using PulseDesk.Application.Interfaces;

namespace PulseDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService _authService)
        {
            this._authService = _authService;
        }

        /// <summary>
        /// Onboard a new Company (Tenant) along with its first Admin user.
        /// </summary>
        [HttpPost("register-tenant")]
        public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RegisterTenantAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Register a new Agent or Client user under an existing Tenant.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RegisterUserAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Authenticate user via Email and Password, returns safe user data and JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.LoginAsync(request, cancellationToken);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint protected by JWT Bearer. Returns current user's token claims.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            var name = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var tenantId = User.FindFirstValue("TenantId");

            return Ok(new
            {
                UserId = userId,
                Email = email,
                FullName = name,
                Role = role,
                TenantId = tenantId
            });
        }
    }
}
