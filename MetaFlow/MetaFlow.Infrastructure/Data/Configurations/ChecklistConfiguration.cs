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
    public class ChecklistConfiguration : IEntityTypeConfiguration<Checklist>
    {
        public void Configure(EntityTypeBuilder<Checklist> builder)
        {
            builder.ToTable("checklists");
            builder.HasKey(cl => cl.Id);

            builder.Property(cl => cl.Id).HasColumnName("id");
            builder.Property(cl => cl.CardId).HasColumnName("card_id").IsRequired();
            builder.Property(cl => cl.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            builder.Property(cl => cl.Position).HasColumnName("position").IsRequired();
            builder.Property(cl => cl.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("NOW()");
            builder.Property(cl => cl.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("NOW()");

            builder.HasOne(cl => cl.Card)
                .WithMany(c => c.Checklists)
                .HasForeignKey(cl => cl.CardId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(cl => cl.CardId).HasDatabaseName("idx_checklists_card");
        }
    }

}
