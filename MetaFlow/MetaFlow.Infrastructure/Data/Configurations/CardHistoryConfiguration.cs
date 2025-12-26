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
    public class CardHistoryConfiguration : IEntityTypeConfiguration<CardHistory>
    {
        public void Configure(EntityTypeBuilder<CardHistory> builder)
        {
            builder.ToTable("card_history");
            builder.HasKey(ch => ch.Id);

            builder.Property(ch => ch.Id).HasColumnName("id");
            builder.Property(ch => ch.CardId).HasColumnName("card_id").IsRequired();
            builder.Property(ch => ch.UserId).HasColumnName("user_id");
            builder.Property(ch => ch.Action).HasColumnName("action").HasMaxLength(50).IsRequired();
            builder.Property(ch => ch.FromColumnId).HasColumnName("from_column_id");
            builder.Property(ch => ch.ToColumnId).HasColumnName("to_column_id");
            builder.Property(ch => ch.Changes).HasColumnName("changes").HasColumnType("jsonb");
            builder.Property(ch => ch.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");

            builder.HasOne(ch => ch.Card)
                .WithMany(c => c.History)
                .HasForeignKey(ch => ch.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ch => new { ch.CardId, ch.CreatedAt })
                .HasDatabaseName("idx_history_card_created")
                .IsDescending(false, true);

            builder.HasIndex(ch => new { ch.Action, ch.CreatedAt })
                .HasDatabaseName("idx_history_action");

            builder.HasIndex(ch => new { ch.FromColumnId, ch.ToColumnId })
                .HasFilter("action = 'moved'")
                .HasDatabaseName("idx_history_columns");
        }
    }

}
