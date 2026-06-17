using FlowDesk.Application.Common.ReadModel;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.ReadModel;

//DB View setup
public class WorkspaceUserViewConfig : IEntityTypeConfiguration<WorkspaceUserView>
{
    public void Configure(EntityTypeBuilder<WorkspaceUserView> builder)
    {
        builder.HasNoKey();
        builder.ToView("vw_WorkspaceUsers");

        builder.Property(x => x.UserId).HasColumnName("UserId");
        builder.Property(x => x.WorkspaceId).HasColumnName("WorkspaceId");
        builder.Property(x => x.UserName).HasColumnName("UserName");
        builder.Property(x => x.Email).HasColumnName("Email");
        builder.Property(x => x.AvatarUrl).HasColumnName("AvatarUrl");
        builder.Property(x => x.Role).HasColumnName("Role");
    }
}
