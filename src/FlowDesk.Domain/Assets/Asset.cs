using FlowDesk.Domain.AssetCategories;

namespace FlowDesk.Domain.Assets;

public sealed class Asset : AggregateRoot<Guid>
{
    public AssetName Name { get; private set; } = null!;
    public string NormalizedName { get; private set; } = string.Empty;  
    public string ModelNumber  { get; private set; } = string.Empty;
    public string AssetTag { get; private set; } = string.Empty; 
    public string? SerialNumber { get; private set; } 
    public string Manufacturer { get; private set; } = string.Empty;
    public AssetStatus Status { get; private set; }
    public int CategoryId { get; private set; }
    public Guid WorkspaceId {get; private set;}
    public DateTime? DecommissionedAt { get; private set; }
    public DateTime? RestoredAt { get; private set; }

    public AssetCategory AssetCategory { get; private set; } = default!;

    private Asset() : base(Guid.Empty) { }
    
    //Create
    public static Asset Create(
        string name, 
        Guid workspaceId, 
        int categoryId,
        string modelNumber,
        string assetTag,
        string? serialNumber,
        string manufacturer)
    { 
        if (string.IsNullOrWhiteSpace(assetTag))
            throw new DomainException("Asset tag is required.");

        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new DomainException("Manufacturer is required.");

        if(string.IsNullOrWhiteSpace(modelNumber))
            throw new DomainException("Modelnumber is required");

        if(workspaceId == Guid.Empty)
            throw new DomainException("Invalid workspace.");

        if(categoryId <= 0)
            throw new DomainException("Invalid category.");

        var assetName = AssetName.Create(name);    

        return new Asset
        {
            Id = Guid.NewGuid(),
            Name = assetName,
            CategoryId = categoryId,
            ModelNumber = modelNumber,
            WorkspaceId = workspaceId,
            AssetTag = assetTag,
            Manufacturer = manufacturer,
            SerialNumber = serialNumber,
            CreatedAt = DateTime.UtcNow,
            NormalizedName = assetName.Value.Trim().ToUpperInvariant(),
            Status = AssetStatus.Active
        };
    }
    
    //Update
    public void Update(
        string name, 
        int categoryId,
        string modelNumber,
        string manufacturer,
        string? serialnumber)
    {
        if(string.IsNullOrWhiteSpace(modelNumber))
            throw new DomainException("Modelnumber is required");

        if (string.IsNullOrWhiteSpace(manufacturer))
            throw new DomainException("Manufacturer is required.");    

        if(categoryId <= 0)
            throw new DomainException("Invalid category.");
                    
        Name = AssetName.Create(name);
        CategoryId = categoryId;
        ModelNumber = modelNumber;
        UpdatedAt = DateTime.UtcNow;
        NormalizedName = Name.Value.Trim().ToUpperInvariant();
        Manufacturer = manufacturer;
        SerialNumber = serialnumber;
    }

    //Restore
    public void Restore()
    {
        if (Status != AssetStatus.Decommissioned)
            throw new DomainStateException("Only decommissioned assets can be restored");

        Status = AssetStatus.UnderMaintenance; // needs inspection before active
        DecommissionedAt = null;
        RestoredAt = DateTime.UtcNow;
    }

    //Status - Decommission
    public void Decommission()
    {
        if (Status != AssetStatus.Active && Status != AssetStatus.UnderMaintenance)
            throw new DomainStateException("Only active or under maintenance assets can be decommissioned");

        Status = AssetStatus.Decommissioned;
        DecommissionedAt = DateTime.UtcNow; // or a specific DecommissionedAt field
    }

    //Status - UnderMaintenance
    public void SendForMaintenance()
    {
        if (Status != AssetStatus.Active)
            throw new DomainStateException("Only active assets can be sent for maintenance");

        Status = AssetStatus.UnderMaintenance;
    }
    //Status - Active

    public void ReturnFromMaintenance()
    {
        if (Status != AssetStatus.UnderMaintenance)
            throw new DomainStateException("Asset is not under maintenance");

        Status = AssetStatus.Active;
    }
}

