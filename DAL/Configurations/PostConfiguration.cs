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
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Content)
                .IsRequired()
                .HasMaxLength(280);

            builder.Property(p => p.Hashtags)
             .HasMaxLength(500);

            builder.Property(p => p.CreatedAt)
            .IsRequired();

            builder.Property(p => p.UpdatedAt)
             .IsRequired();
        }
    }

}
