using MetaFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace MetaFlow.Infrastructure.Data
{
    public class MetaFlowDbContext : DbContext
    {
        public MetaFlowDbContext(DbContextOptions<MetaFlowDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<Column> Columns => Set<Column>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<MethodologyPreset> MethodologyPresets => Set<MethodologyPreset>();
        public DbSet<BoardMember> BoardMembers => Set<BoardMember>();
        public DbSet<Swimlane> Swimlanes => Set<Swimlane>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<CardTag> CardTags => Set<CardTag>();
        public DbSet<CardComment> CardComments => Set<CardComment>();
        public DbSet<CardAttachment> CardAttachments => Set<CardAttachment>();
        public DbSet<Checklist> Checklists => Set<Checklist>();
        public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();
        public DbSet<CardHistory> CardHistories => Set<CardHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MetaFlowDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is Domain.Common.BaseEntity &&
                           (e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                ((Domain.Common.BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}