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
    public class CardAttachmentConfiguration : IEntityTypeConfiguration<CardAttachment>
    {
        public void Configure(EntityTypeBuilder<CardAttachment> builder)
        {
            builder.ToTable("card_attachments");
            builder.HasKey(ca => ca.Id);

            builder.Property(ca => ca.Id).HasColumnName("id");
            builder.Property(ca => ca.CardId).HasColumnName("card_id").IsRequired();
            builder.Property(ca => ca.FileName).HasColumnName("file_name").HasMaxLength(255).IsRequired();
            builder.Property(ca => ca.FileUrl).HasColumnName("file_url").HasMaxLength(1000).IsRequired();
            builder.Property(ca => ca.FileSize).HasColumnName("file_size");
            builder.Property(ca => ca.MimeType).HasColumnName("mime_type").HasMaxLength(100);
            builder.Property(ca => ca.ThumbnailUrl).HasColumnName("thumbnail_url").HasMaxLength(1000);
            builder.Property(ca => ca.UploadedById).HasColumnName("uploaded_by_id").IsRequired();
            builder.Property(ca => ca.UploadedAt).HasColumnName("uploaded_at").HasDefaultValueSql("NOW()");
            builder.Property(ca => ca.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(ca => ca.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            builder.HasOne(ca => ca.Card)
                .WithMany(c => c.Attachments)
                .HasForeignKey(ca => ca.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ca => ca.UploadedBy)
                .WithMany()
                .HasForeignKey(ca => ca.UploadedById)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(ca => ca.CardId).HasDatabaseName("idx_attachments_card");
            builder.HasIndex(ca => ca.UploadedById).HasDatabaseName("idx_attachments_user");
        }
    }

}
