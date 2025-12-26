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
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("tags");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).HasColumnName("id");
            builder.Property(t => t.BoardId).HasColumnName("board_id").IsRequired();
            builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
            builder.Property(t => t.Color).HasColumnName("color").HasMaxLength(20).IsRequired();
            builder.Property(t => t.Description).HasColumnName("description");
            builder.Property(t => t.UsageCount).HasColumnName("usage_count").HasDefaultValue(0);
            builder.Property(t => t.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            builder.HasOne(t => t.Board)
                .WithMany(b => b.Tags)
                .HasForeignKey(t => t.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(t => t.BoardId).HasDatabaseName("idx_tags_board");
            builder.HasIndex(t => new { t.BoardId, t.Name }).IsUnique().HasDatabaseName("idx_tags_board_name_unique");
        }
    }

}
