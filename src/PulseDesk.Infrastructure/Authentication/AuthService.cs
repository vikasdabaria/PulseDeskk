using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulseDesk.Application.DTOs.Auth;
using PulseDesk.Application.Interfaces;
using PulseDesk.Domain.Entities;
using PulseDesk.Domain.Enums;
using PulseDesk.infrastructure.Data;

namespace PulseDesk.infrastructure.Authentication
{
    public sealed class AuthService : IAuthService
    {
        private readonly PulseDeskDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(
            PulseDeskDbContext context,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<AuthResponse> RegisterTenantAsync(RegisterTenantRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Validate Company Code uniqueness
            var codeExists = await _context.Tenants
                .AnyAsync(t => t.Code == request.CompanyCode.ToUpperInvariant(), cancellationToken);

            if (codeExists)
            {
                throw new InvalidOperationException($"Company code '{request.CompanyCode}' is already registered.");
            }

            // 2. Validate Admin Email uniqueness
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.AdminEmail.ToLowerInvariant(), cancellationToken);

            if (emailExists)
            {
                throw new InvalidOperationException($"Email '{request.AdminEmail}' is already registered.");
            }

            // 3. Create & Save Tenant
            var tenant = new Tenant
            {
                Name = request.CompanyName,
                Code = request.CompanyCode.ToUpperInvariant(),
                IsActive = true
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Create & Save Admin User for this Tenant
            var adminUser = new User
            {
                TenantId = tenant.Id,
                FullName = request.AdminFullName,
                Email = request.AdminEmail.ToLowerInvariant(),
                PasswordHash = _passwordHasher.Hash(request.AdminPassword),
                Role = UserRole.Admin,
                IsActive = true
            };

            _context.Users.Add(adminUser);
            await _context.SaveChangesAsync(cancellationToken);

            // 5. Issue JWT Token & Return Safe Response
            var token = _jwtTokenGenerator.GenerateToken(adminUser);

            return new AuthResponse(
                adminUser.Id,
                adminUser.Email,
                adminUser.FullName,
                adminUser.Role.ToString(),
                adminUser.TenantId,
                token
            );
        }

        public async Task<AuthResponse> RegisterUserAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Verify target Tenant exists and is active
            var tenantExists = await _context.Tenants
                .AnyAsync(t => t.Id == request.TenantId && t.IsActive, cancellationToken);

            if (!tenantExists)
            {
                throw new InvalidOperationException("Invalid or inactive tenant.");
            }

            // 2. Verify Email uniqueness
            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

            if (emailExists)
            {
                throw new InvalidOperationException($"Email '{request.Email}' is already registered.");
            }

            // 3. Create & Save New User
            var user = new User
            {
                TenantId = request.TenantId,
                FullName = request.FullName,
                Email = request.Email.ToLowerInvariant(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                Role = request.Role,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            // 4. Issue JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponse(
                user.Id,
                user.Email,
                user.FullName,
                user.Role.ToString(),
                user.TenantId,
                token
            );
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            // 1. Find User by Email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

            if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new InvalidOperationException("Invalid email or password.");
            }

            // 2. Check if User is active
            if (!user.IsActive)
            {
                throw new InvalidOperationException("User account is inactive.");
            }

            // 3. Verify Tenant is active
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Id == user.TenantId, cancellationToken);

            if (tenant == null || !tenant.IsActive)
            {
                throw new InvalidOperationException("Tenant organization is inactive or not found.");
            }

            // 4. Issue JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponse(
                user.Id,
                user.Email,
                user.FullName,
                user.Role.ToString(),
                user.TenantId,
                token
            );
        }
    }
}
