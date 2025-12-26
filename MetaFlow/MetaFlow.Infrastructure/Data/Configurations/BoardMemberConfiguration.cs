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
    public class BoardMemberConfiguration : IEntityTypeConfiguration<BoardMember>
    {
        public void Configure(EntityTypeBuilder<BoardMember> builder)
        {
            builder.ToTable("board_members");
            builder.HasKey(bm => bm.Id);

            builder.Property(bm => bm.Id).HasColumnName("id");
            builder.Property(bm => bm.BoardId).HasColumnName("board_id").IsRequired();
            builder.Property(bm => bm.UserId).HasColumnName("user_id").IsRequired();
            builder.Property(bm => bm.Role).HasColumnName("role").HasMaxLength(50).HasDefaultValue("member");
            builder.Property(bm => bm.Permissions).HasColumnName("permissions").HasColumnType("jsonb");
            builder.Property(bm => bm.JoinedAt).HasColumnName("joined_at").HasDefaultValueSql("NOW()");
            builder.Property(bm => bm.InvitedBy).HasColumnName("invited_by");

            builder.HasOne(bm => bm.Board)
                .WithMany(b => b.Members)
                .HasForeignKey(bm => bm.BoardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bm => bm.User)
                .WithMany(u => u.BoardMemberships)
                .HasForeignKey(bm => bm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(bm => bm.BoardId).HasDatabaseName("idx_board_members_board");
            builder.HasIndex(bm => bm.UserId).HasDatabaseName("idx_board_members_user");
            builder.HasIndex(bm => new { bm.BoardId, bm.UserId }).IsUnique().HasDatabaseName("idx_board_members_unique");
        }
    }

}
