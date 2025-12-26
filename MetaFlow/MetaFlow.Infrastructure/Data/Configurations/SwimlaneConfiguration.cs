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
    public class SwimlaneConfiguration : IEntityTypeConfiguration<Swimlane>
    {
        public void Configure(EntityTypeBuilder<Swimlane> builder)
        {
            builder.ToTable("swimlanes");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.BoardId).HasColumnName("board_id").IsRequired();
            builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            builder.Property(s => s.Description).HasColumnName("description");
            builder.Property(s => s.Position).HasColumnName("position").IsRequired();
            builder.Property(s => s.SwimlaneType).HasColumnName("swimlane_type").HasMaxLength(50).HasDefaultValue("custom");
            builder.Property(s => s.Color).HasColumnName("color").HasMaxLength(20);
            builder.Property(s => s.IsCollapsed).HasColumnName("is_collapsed").HasDefaultValue(false);
            builder.Property(s => s.IsVisible).HasColumnName("is_visible").HasDefaultValue(true);
            builder.Property(s => s.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            builder.HasOne(s => s.Board)
                .WithMany(b => b.Swimlanes)
                .HasForeignKey(s => s.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(s => s.BoardId).HasDatabaseName("idx_swimlanes_board");
            builder.HasIndex(s => new { s.BoardId, s.Position }).HasDatabaseName("idx_swimlanes_board_position");
        }
    }

}
