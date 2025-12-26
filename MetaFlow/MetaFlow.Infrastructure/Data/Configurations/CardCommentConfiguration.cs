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
    public class CardCommentConfiguration : IEntityTypeConfiguration<CardComment>
    {
        public void Configure(EntityTypeBuilder<CardComment> builder)
        {
            builder.ToTable("card_comments");
            builder.HasKey(cc => cc.Id);

            builder.Property(cc => cc.Id).HasColumnName("id");
            builder.Property(cc => cc.CardId).HasColumnName("card_id").IsRequired();
            builder.Property(cc => cc.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(cc => cc.ParentCommentId).HasColumnName("parent_comment_id");
            builder.Property(cc => cc.Content).HasColumnName("content").IsRequired();
            builder.Property(cc => cc.Metadata).HasColumnName("metadata").HasColumnType("jsonb").HasDefaultValue("{}");
            builder.Property(cc => cc.IsEdited).HasColumnName("is_edited").HasDefaultValue(false);
            builder.Property(cc => cc.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);
            builder.Property(cc => cc.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(cc => cc.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");
            builder.Property(cc => cc.DeletedAt).HasColumnName("deleted_at");

            builder.HasOne(cc => cc.Card)
                .WithMany(c => c.Comments)
                .HasForeignKey(cc => cc.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cc => cc.User)
                .WithMany()
                .HasForeignKey(cc => cc.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(cc => cc.ParentComment)
                .WithMany()
                .HasForeignKey(cc => cc.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(cc => new { cc.CardId, cc.CreatedAt })
                .HasDatabaseName("idx_comments_card_created");
            builder.HasIndex(cc => cc.UserId).HasDatabaseName("idx_comments_user");
            builder.HasIndex(cc => cc.ParentCommentId)
                .HasFilter("parent_comment_id IS NOT NULL")
                .HasDatabaseName("idx_comments_parent");
        }
    }

}
