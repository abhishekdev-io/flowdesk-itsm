using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Workspaces;

public class WorkspaceMemberConfig : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.HasKey(wm => new { wm.UserId, wm.WorkspaceId });

        builder.Property(wm => wm.Role)
                .HasConversion<string>()
                .IsRequired();
                
        builder.HasOne(wm => wm.Workspace)
                .WithMany(w => w.WorkspaceMembers)
                .HasForeignKey(wm => wm.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(wm => wm.UserId)
                .OnDelete(DeleteBehavior.Restrict);  
                
        builder.HasQueryFilter(wm => !wm.Workspace.IsDeleted);               

        builder.HasIndex(wm => wm.WorkspaceId);
        builder.HasIndex(wm => wm.UserId);              
    }
}




