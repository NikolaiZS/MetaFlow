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
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id");

            builder.Property(u => u.Email)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(u => u.Username)
                .HasColumnName("username")
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(u => u.FullName)
                .HasColumnName("full_name")
                .HasMaxLength(200);

            builder.Property(u => u.AvatarUrl)
                .HasColumnName("avatar_url")
                .HasMaxLength(500);

            builder.Property(u => u.PasswordHash)
                .HasColumnName("password_hash")
                .HasMaxLength(255);

            builder.Property(u => u.EmailVerified)
                .HasColumnName("email_verified")
                .HasDefaultValue(false);

            builder.Property(u => u.Preferences)
                .HasColumnName("preferences")
                .HasColumnType("jsonb")
                .HasDefaultValue("{}");

            builder.Property(u => u.CreatedAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("NOW()");

            builder.Property(u => u.UpdatedAt)
                .HasColumnName("updated_at")
                .HasDefaultValueSql("NOW()");

            builder.Property(u => u.LastLoginAt)
                .HasColumnName("last_login_at");

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("idx_users_email");

            builder.HasIndex(u => u.Username)
                .IsUnique()
                .HasDatabaseName("idx_users_username");

            builder.HasIndex(u => u.Preferences)
                .HasMethod("GIN")
                .HasDatabaseName("idx_users_preferences");
        }

    }
}
