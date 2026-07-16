using FlowDesk.Domain.Authentication;
using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Authentication;

public class RefreshTokenConfig : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder) 
    {
        builder.ToTable("RefreshTokens");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TokenHash)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.HasOne<AppUser>()
            .WithMany() 
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(r => r.TokenHash).IsUnique();

        builder.HasIndex(t => new { t.UserId, t.RevokedAt })
            .HasDatabaseName("IX_RefreshTokens_UserId_Active");       
    }
}
  