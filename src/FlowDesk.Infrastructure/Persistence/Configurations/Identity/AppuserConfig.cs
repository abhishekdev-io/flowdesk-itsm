using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Identity;

public class AppuserConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasMany(u => u.WorkspaceMembers)
                .WithOne()
                .HasForeignKey(wm => wm.UserId)
                .OnDelete(DeleteBehavior.Restrict);
    }
}
