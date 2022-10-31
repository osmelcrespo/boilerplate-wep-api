using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration
{
    internal class AspNetRoleClaimConfig : IEntityTypeConfiguration<AspNetRoleClaims>
    {
        public void Configure(EntityTypeBuilder<AspNetRoleClaims> builder)
        {
            builder.ToTable("AspNetRoleClaims");

            builder.Property(e => e.RoleId).IsRequired().HasMaxLength(450);

            builder.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasForeignKey(d => d.RoleId);
        }
    }
}
