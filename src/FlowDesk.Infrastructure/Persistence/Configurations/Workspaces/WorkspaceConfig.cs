using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FlowDesk.Domain.Workspaces.Aggregates;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Workspaces;

public class WorkspaceConfig : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(t => t.Id)
               .ValueGeneratedNever();

        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(x => x.Value)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(w => w.CreatedAt)
                .IsRequired();               

        builder.HasMany(wm => wm.WorkspaceMembers)
                .WithOne(wm => wm.Workspace)
                .HasForeignKey(wm => wm.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade);
                
        builder.HasIndex(w => w.NormalizedName)
               .IsUnique();        
        
        builder.Property(w => w.IsDeleted)
               .HasDefaultValue(false)
               .IsRequired();
               
        builder.HasQueryFilter(w => !w.IsDeleted);                  
    
    }
}
