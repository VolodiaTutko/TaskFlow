using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskFlow.Infrastructure.Identity;

namespace TaskFlow.Infrastructure.Persistence.Configurations;

public sealed class ApplicationUserConfiguration
    : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.DisplayName)
            .HasMaxLength(100);

        builder.HasIndex(u => u.NormalizedEmail)
            .IsUnique();
    }
}