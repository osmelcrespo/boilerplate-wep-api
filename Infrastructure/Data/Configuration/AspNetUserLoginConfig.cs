using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration
{
    internal class AspNetUserLoginConfig : IEntityTypeConfiguration<AspNetUserLogins>
    {
        public void Configure(EntityTypeBuilder<AspNetUserLogins> builder)
        {
            builder.HasKey(e => new { e.LoginProvider, e.ProviderKey });

            builder.Property(e => e.LoginProvider).HasMaxLength(128);

            builder.Property(e => e.ProviderKey).HasMaxLength(128);

            builder.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.HasOne(d => d.User)
                .WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId);
        }
    }
}
