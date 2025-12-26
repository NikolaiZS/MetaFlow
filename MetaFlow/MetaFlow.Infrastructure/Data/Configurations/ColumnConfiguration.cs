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
    public class ColumnConfiguration : IEntityTypeConfiguration<Column>
    {
        public void Configure(EntityTypeBuilder<Column> builder)
        {
            builder.ToTable("columns");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).HasColumnName("id");
            builder.Property(c => c.BoardId).HasColumnName("board_id").IsRequired();
            builder.Property(c => c.Name).HasColumnName("name").HasMaxLength(100).IsRequired();
            builder.Property(c => c.Description).HasColumnName("description");
            builder.Property(c => c.Position).HasColumnName("position").IsRequired();
            builder.Property(c => c.ColumnType).HasColumnName("column_type").HasMaxLength(50).HasDefaultValue("standard");
            builder.Property(c => c.WipLimit).HasColumnName("wip_limit");
            builder.Property(c => c.Color).HasColumnName("color").HasMaxLength(20).HasDefaultValue("#e0e0e0");
            builder.Property(c => c.Settings).HasColumnName("settings").HasColumnType("jsonb");
            builder.Property(c => c.IsVisible).HasColumnName("is_visible").HasDefaultValue(true);
            builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            builder.HasOne(c => c.Board)
                .WithMany(b => b.Columns)
                .HasForeignKey(c => c.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(c => c.BoardId).HasDatabaseName("idx_columns_board");
            builder.HasIndex(c => new { c.BoardId, c.Position }).HasDatabaseName("idx_columns_board_position");
            builder.HasIndex(c => new { c.BoardId, c.Name }).IsUnique().HasDatabaseName("idx_columns_board_name");
        }
    }

}
