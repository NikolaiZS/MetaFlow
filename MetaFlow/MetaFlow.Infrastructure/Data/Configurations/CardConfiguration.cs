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
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.ToTable("cards");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.BoardId).HasColumnName("board_id").IsRequired();
            builder.Property(c => c.ColumnId).HasColumnName("column_id").IsRequired();
            builder.Property(c => c.SwimlaneId).HasColumnName("swimlane_id");
            builder.Property(c => c.Title).HasColumnName("title").HasMaxLength(500).IsRequired();
            builder.Property(c => c.Description).HasColumnName("description");
            builder.Property(c => c.Position).HasColumnName("position").IsRequired();
            builder.Property(c => c.Priority).HasColumnName("priority").HasMaxLength(20).HasDefaultValue("medium");
            builder.Property(c => c.Status).HasColumnName("status").HasMaxLength(50).HasDefaultValue("active");
            builder.Property(c => c.DueDate).HasColumnName("due_date");
            builder.Property(c => c.StartDate).HasColumnName("start_date");
            builder.Property(c => c.CompletedAt).HasColumnName("completed_at");
            builder.Property(c => c.CreatedById).HasColumnName("created_by_id").IsRequired();
            builder.Property(c => c.AssignedToId).HasColumnName("assigned_to_id");
            builder.Property(c => c.SprintId).HasColumnName("sprint_id");
            builder.Property(c => c.CustomFields).HasColumnName("custom_fields").HasColumnType("jsonb");
            builder.Property(c => c.Metadata).HasColumnName("metadata").HasColumnType("jsonb");
            builder.Property(c => c.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            builder.HasOne(c => c.Board)
                .WithMany(b => b.Cards)
                .HasForeignKey(c => c.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Column)
                .WithMany(col => col.Cards)
                .HasForeignKey(c => c.ColumnId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Swimlane)
                .WithMany(s => s.Cards)
                .HasForeignKey(c => c.SwimlaneId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.CreatedBy)
                .WithMany()
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.AssignedTo)
                .WithMany()
                .HasForeignKey(c => c.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(c => c.BoardId).HasDatabaseName("idx_cards_board");
            builder.HasIndex(c => c.ColumnId).HasDatabaseName("idx_cards_column");
            builder.HasIndex(c => new { c.ColumnId, c.Position }).HasDatabaseName("idx_cards_column_position");
            builder.HasIndex(c => c.AssignedToId)
                .HasFilter("assigned_to_id IS NOT NULL")
                .HasDatabaseName("idx_cards_assigned");
            builder.HasIndex(c => c.DueDate)
                .HasFilter("due_date IS NOT NULL AND is_archived = false")
                .HasDatabaseName("idx_cards_due_date");
            builder.HasIndex(c => c.CustomFields).HasMethod("GIN").HasDatabaseName("idx_cards_custom_fields");
            builder.HasIndex(c => new { c.BoardId, c.Priority }).HasDatabaseName("idx_cards_priority");
            builder.HasIndex(c => c.Status)
                .HasFilter("status != 'archived'")
                .HasDatabaseName("idx_cards_status");
        }
    }

}
