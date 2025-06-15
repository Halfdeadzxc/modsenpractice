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
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.HasKey(s => new { s.FollowerId, s.FollowingId });

            builder.Property(s => s.Status)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.FollowerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(s => s.FollowingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
