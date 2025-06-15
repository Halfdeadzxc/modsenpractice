using DAL.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations
{
    public class RepostConfiguration : IEntityTypeConfiguration<Repost>
    {
        public void Configure(EntityTypeBuilder<Repost> builder)
        {
            builder.HasKey(r => new { r.UserId, r.PostId });

            builder.Property(r => r.Comment)
                .HasMaxLength(280);

            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.HasOne(r => r.Post)
                .WithMany()
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
