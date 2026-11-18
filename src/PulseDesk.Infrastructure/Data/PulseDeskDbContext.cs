using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PulseDesk.Application.Interfaces;
using PulseDesk.Domain.Common;
using PulseDesk.Domain.Entities;

namespace PulseDesk.infrastructure.Data
{
    public sealed class PulseDeskDbContext : DbContext
    {
        private readonly ICurrentTenantService _currentTenantService;
        public PulseDeskDbContext(DbContextOptions<PulseDeskDbContext> options, ICurrentTenantService currentTenantService) : base(options)
        {
            _currentTenantService = currentTenantService;
        }
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketComment> TicketComments => Set<TicketComment>();
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAtUtc = DateTime.UtcNow;

                    if (entry.Entity is ITenantEntity tenantEntity && tenantEntity.TenantId == 0 && _currentTenantService.TenantId.HasValue)
                    {
                        tenantEntity.TenantId = _currentTenantService.TenantId.Value;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>().HasQueryFilter(t => t.TenantId == _currentTenantService.TenantId);
            modelBuilder.Entity<TicketComment>().HasQueryFilter(c => c.TenantId == _currentTenantService.TenantId);



            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasOne(t => t.Tenant)
                    .WithMany()
                    .HasForeignKey(t => t.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.CreatedByUser)
                    .WithMany()
                    .HasForeignKey(t => t.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.AssignedToUser)
                    .WithMany()
                    .HasForeignKey(t => t.AssignedToUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TicketComment>(entity =>
            {
                entity.HasOne(c => c.Tenant)
                    .WithMany()
                    .HasForeignKey(c => c.TenantId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User)
                    .WithMany()
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
