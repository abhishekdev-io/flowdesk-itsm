using FlowDesk.Domain.AssetCategories;
using FlowDesk.Domain.Workspaces.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.AssetCategories;

public class AssetCategoryConfig : IEntityTypeConfiguration<AssetCategory>
{
    public void Configure(EntityTypeBuilder<AssetCategory> builder)
    {
        builder.HasKey(c => c.Id);
               
        // This is the database-agnostic way to tell EF to map Postgres's system column
        builder.Property<uint>("xmin")
            .HasColumnType("xid")
            .ValueGeneratedOnAddOrUpdate()
            .IsConcurrencyToken();       

        builder.OwnsOne(x => x.Name, name =>
        {
            name.Property(x => x.Value)
                .HasColumnName("Name")
                .HasMaxLength(50)
                .IsRequired();    
        });

        builder.Property(c => c.WorkspaceId).IsRequired();

        builder.Property(c => c.Prefix)
            .HasColumnName("Prefix")
            .IsRequired();

        builder.Property(c => c.NextAssetNumber)
            .HasColumnName("NextAssetNumber")
            .IsRequired();

        builder.HasIndex(c => c.WorkspaceId);

        builder.HasIndex(c => new
        {
            c.WorkspaceId,
            c.NormalizedName
        }).IsUnique();

        builder.Property(c => c.IsActive)
               .IsRequired()
               .HasDefaultValue(true);        
    }
}
