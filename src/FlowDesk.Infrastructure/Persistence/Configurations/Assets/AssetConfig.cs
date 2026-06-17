using FlowDesk.Domain.Assets;
using FlowDesk.Domain.AssetCategories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FlowDesk.Domain.Workspaces.Aggregates;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Assets;

public class AssetConfig : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(t => t.Id)
               .ValueGeneratedNever();

        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(x => x.Value)
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsRequired();
        });

        builder.Property(a => a.WorkspaceId).IsRequired();

        builder.HasOne(a => a.AssetCategory)
               .WithMany()
               .HasForeignKey(t => t.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.Property(a => a.Status)
               .HasConversion<string>()
               .IsRequired();                

        builder.HasIndex(a => a.WorkspaceId);
        builder.HasIndex(a => a.CategoryId);
        builder.HasIndex(a => new { a.WorkspaceId, a.CategoryId });
        builder.HasIndex(c => new
        {
            c.WorkspaceId,
            c.NormalizedName
        }).IsUnique();    

        builder.HasIndex(x => new
        {
            x.WorkspaceId,
            x.AssetTag
        }).IsUnique();   
    }
}
