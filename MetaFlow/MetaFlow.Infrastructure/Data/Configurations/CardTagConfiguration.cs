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
    public class CardTagConfiguration : IEntityTypeConfiguration<CardTag>
    {
        public void Configure(EntityTypeBuilder<CardTag> builder)
        {
            builder.ToTable("card_tags");
            builder.HasKey(ct => new { ct.CardId, ct.TagId });

            builder.Property(ct => ct.CardId).HasColumnName("card_id");
            builder.Property(ct => ct.TagId).HasColumnName("tag_id");
            builder.Property(ct => ct.AddedAt).HasColumnName("added_at").HasDefaultValueSql("NOW()");
            builder.Property(ct => ct.AddedBy).HasColumnName("added_by");

            builder.HasOne(ct => ct.Card)
                .WithMany(c => c.CardTags)
                .HasForeignKey(ct => ct.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ct => ct.Tag)
                .WithMany(t => t.CardTags)
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ct => ct.CardId).HasDatabaseName("idx_card_tags_card");
            builder.HasIndex(ct => ct.TagId).HasDatabaseName("idx_card_tags_tag");
        }
    }

}
