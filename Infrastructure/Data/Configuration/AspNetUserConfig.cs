using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration
{
    internal class AspNetUserConfig : IEntityTypeConfiguration<AspNetUser>
    {
        public void Configure(EntityTypeBuilder<AspNetUser> builder)
        {
            builder.ToTable("AspNetUsers");

            builder.Property(e => e.Email).HasMaxLength(256);

            builder.Property(e => e.NormalizedEmail).HasMaxLength(256);

            builder.Property(e => e.NormalizedUserName).HasMaxLength(256);

            builder.Property(e => e.UserName).HasMaxLength(256);

            builder.Ignore(x=>x.Password);
        }
    }
}
