using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MetaFlow.Domain.Entities;

namespace MetaFlow.Infrastructure.Data.Configurations
{
    public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
    {
        public void Configure(EntityTypeBuilder<ChecklistItem> builder)
        {
            builder.ToTable("checklist_items");
            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id).HasColumnName("id");
            builder.Property(ci => ci.ChecklistId).HasColumnName("checklist_id").IsRequired();
            builder.Property(ci => ci.Content).HasColumnName("content").HasMaxLength(500).IsRequired();
            builder.Property(ci => ci.Position).HasColumnName("position").IsRequired();
            builder.Property(ci => ci.IsCompleted).HasColumnName("is_completed").HasDefaultValue(false);
            builder.Property(ci => ci.CompletedAt).HasColumnName("completed_at");
            builder.Property(ci => ci.CompletedById).HasColumnName("completed_by_id");
            builder.Property(ci => ci.AssignedToId).HasColumnName("assigned_to_id");
            builder.Property(ci => ci.DueDate).HasColumnName("due_date");
            builder.Property(ci => ci.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(ci => ci.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            builder.HasOne(ci => ci.Checklist)
                .WithMany(cl => cl.Items)
                .HasForeignKey(ci => ci.ChecklistId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ci => new { ci.ChecklistId, ci.Position })
                .HasDatabaseName("idx_checklist_items_checklist_position");
        }
    }

}
