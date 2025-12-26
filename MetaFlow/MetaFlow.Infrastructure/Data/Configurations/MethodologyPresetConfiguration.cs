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
    public class MethodologyPresetConfiguration : IEntityTypeConfiguration<MethodologyPreset>
    {
        public void Configure(EntityTypeBuilder<MethodologyPreset> builder)
        {
            builder.ToTable("methodology_presets");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id).HasColumnName("id");
            builder.Property(m => m.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            builder.Property(m => m.DisplayName).HasColumnName("display_name").HasMaxLength(100).IsRequired();
            builder.Property(m => m.Description).HasColumnName("description");
            builder.Property(m => m.Icon).HasColumnName("icon").HasMaxLength(50);
            builder.Property(m => m.Category).HasColumnName("category").HasMaxLength(50);
            builder.Property(m => m.Config).HasColumnName("config").HasColumnType("jsonb").IsRequired();
            builder.Property(m => m.IsSystem).HasColumnName("is_system").HasDefaultValue(false);
            builder.Property(m => m.IsActive).HasColumnName("is_active").HasDefaultValue(true);
            builder.Property(m => m.CreatedBy).HasColumnName("created_by");
            builder.Property(m => m.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(m => m.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            builder.HasIndex(m => m.Name).IsUnique().HasDatabaseName("idx_methodology_name");
            builder.HasIndex(m => m.Category).HasDatabaseName("idx_methodology_category");
            builder.HasIndex(m => m.Config).HasMethod("GIN").HasDatabaseName("idx_methodology_config");
        }
    }

}
