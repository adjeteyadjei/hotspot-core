using hotvenues.Models;
using Hotvenues.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Hotvenues.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<AppSetting> AppSettings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageOutward> MessageOutwards { get; set; }
        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<LiveStatus> LiveStatuses { get; set; }
        public DbSet<LikeStatus> StatusLikes { get; set; }
        public DbSet<CommentStatus> StatusComments { get; set; }
        public DbSet<UpcomingEvent> UpcomingEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added)
                .Select(x => x.Entity).OfType<IAuditable>())
            {
                entry.CreatedAt = DateTime.UtcNow;
                entry.ModifiedAt = DateTime.UtcNow;
            }

            foreach (var entry in ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Modified)
                .Select(x => x.Entity)
                .OfType<IAuditable>())
            { entry.ModifiedAt = DateTime.UtcNow; }

            #region Audit Trail
            /*var modifiedEntities = ChangeTracker.Entries().ToList()
                .Where(p => p.Entity.GetType().Name != "AuditTrail")
                .Where(p => p.State == EntityState.Modified || p.State == EntityState.Deleted)
                .ToList();


            foreach (var change in modifiedEntities)
            {
                var eType = change.Entity.GetType();
                var audit = new AuditTrail
                {
                    Timestamp = DateTime.UtcNow,
                    Model = eType.Name,
                    User = change.Entity.GetType().GetProperty("ModifiedBy")?.GetValue(change.Entity, null)?.ToString() ?? "",
                    ReferenceId = change.Entity.GetType().GetProperty("Id")?.GetValue(change.Entity, null)?.ToString() ?? ""
                };

                switch (change.State)
                {
                    case EntityState.Detached:
                    case EntityState.Unchanged:
                    case EntityState.Added:
                        break;
                    case EntityState.Deleted:
                        audit.Action = AuditAction.Delete;
                        audit.OldObject = JsonSerializer.Serialize(change.Entity);
                        break;
                    case EntityState.Modified:
                        audit.Action = AuditAction.Update;
                        audit.NewObject = JsonSerializer.Serialize(change.CurrentValues.ToObject());
                        audit.OldObject = JsonSerializer.Serialize(Entry(change.Entity).GetDatabaseValues().ToObject());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                AuditTrails.Add(audit);
            }*/
            #endregion

            return base.SaveChanges();
        }
    }
}
