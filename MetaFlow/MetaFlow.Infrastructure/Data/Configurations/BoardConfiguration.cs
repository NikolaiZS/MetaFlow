using MetaFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaFlow.Infrastructure.Data.Configurations
{
    public class BoardConfiguration : IEntityTypeConfiguration<Board>
    {
        public void Configure(EntityTypeBuilder<Board> builder)
        {
            builder.ToTable("boards");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id).HasColumnName("id");
            builder.Property(b => b.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            builder.Property(b => b.Description).HasColumnName("description");
            builder.Property(b => b.MethodologyPresetId).HasColumnName("methodology_preset_id").IsRequired();
            builder.Property(b => b.CustomConfig).HasColumnName("custom_config").HasColumnType("jsonb");
            builder.Property(b => b.OwnerId).HasColumnName("owner_id").IsRequired();
            builder.Property(b => b.IsPublic).HasColumnName("is_public").HasDefaultValue(false);
            builder.Property(b => b.IsTemplate).HasColumnName("is_template").HasDefaultValue(false);
            builder.Property(b => b.IsArchived).HasColumnName("is_archived").HasDefaultValue(false);
            builder.Property(b => b.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(b => b.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
            builder.Property(b => b.ArchivedAt).HasColumnName("archived_at");

            builder.HasOne(b => b.Owner)
                .WithMany(u => u.OwnedBoards)
                .HasForeignKey(b => b.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(b => b.MethodologyPreset)
                .WithMany(m => m.Boards)
                .HasForeignKey(b => b.MethodologyPresetId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(b => b.OwnerId).HasDatabaseName("idx_boards_owner");
            builder.HasIndex(b => b.MethodologyPresetId).HasDatabaseName("idx_boards_methodology");
            builder.HasIndex(b => b.IsArchived)
                .HasFilter("is_archived = false")
                .HasDatabaseName("idx_boards_archived");
            builder.HasIndex(b => b.CustomConfig).HasMethod("GIN").HasDatabaseName("idx_boards_custom_config");
        }
    }

}
