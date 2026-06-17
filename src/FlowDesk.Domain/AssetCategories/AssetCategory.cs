using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;

namespace FlowDesk.Domain.AssetCategories;

public sealed class AssetCategory : Entity<int>
{
    public CategoryName Name { get; private set; } = null!;
    public Guid WorkspaceId { get; private set; }
    public bool IsActive { get; private set; }
    public string NormalizedName { get; private set; } = string.Empty;
    public DateTime? DeactivatedAt { get; private set; }
    public DateTime? RestoredAt { get; private set; }
    public string Prefix { get; private set; } = string.Empty;
    public int NextAssetNumber { get; private set; } = 1;

    private AssetCategory() : base(0) { }

    //generate asset tag
    public string GenerateAssetTag()
    {
        var tag = $"{Prefix}-{NextAssetNumber:D4}";
        NextAssetNumber++;
        return tag;
    }

    //Create
    public static AssetCategory Create(
        string name, 
        Guid workspaceId,
        string prefix)
    { 
        var categoryName = CategoryName.Create(name);

        if (workspaceId == Guid.Empty)
            throw new DomainException("Invalid workspace.");

        if (string.IsNullOrWhiteSpace(prefix))
        throw new DomainException("Prefix is required.");    

        return new AssetCategory
        {
            Name = categoryName,
            WorkspaceId = workspaceId,
            NormalizedName = categoryName.Value.Trim().ToUpperInvariant(),
            IsActive = true,
            Prefix = prefix.Trim().ToUpperInvariant()
        };
    }
    
    //Update
    public void Update(
        string name,
        string prefix)
    {   
        if (string.IsNullOrWhiteSpace(prefix))
        throw new DomainException("Prefix is required."); 

        var newName = CategoryName.Create(name);    
        Name = newName;
        NormalizedName = newName.Value.Trim().ToUpperInvariant();
        Prefix = prefix.Trim().ToUpperInvariant();
    }

    //Deactivate
    public void Deactivate()
    {
        if (!IsActive) return;

        IsActive = false;
        DeactivatedAt = DateTime.UtcNow;
    }
    
    //Restore
    public void Restore()
    {
        if(IsActive) return;
        IsActive = true;
        RestoredAt = DateTime.UtcNow;
        DeactivatedAt = null;
    }
}

