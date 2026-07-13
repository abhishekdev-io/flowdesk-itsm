using FlowDesk.Domain.AssetCategories;
using FlowDesk.Domain.Assets;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Application.IntegrationTests.Common.Helper;

public class TestDataBuilder(ApplicationDbContext dbContext)
{
    // Workspace Builder 
    public async Task<Workspace> WorkspaceAsync(string name = "Default Workspace")
    {
        var workspace = Workspace.Create(name, name);
        dbContext.Workspaces.Add(workspace);
        await dbContext.SaveChangesAsync();
        return workspace;
    }

    // Member Builder
    public async Task<WorkspaceMember> MemberAsync(Guid userId, Guid workspaceId, WorkspaceRole role = WorkspaceRole.Member)
    {
        var membership = WorkspaceMember.Create(userId, workspaceId, role);
        dbContext.WorkspaceMembers.Add(membership);
        await dbContext.SaveChangesAsync();
        return membership;
    }

    // AssetCategory Builder
    public async Task<AssetCategory> AssetCategoryAsync(string name, Guid workspaceId, string prefix = "PFX")
    {
        var category = AssetCategory.Create(name, workspaceId, prefix);
        dbContext.AssetCategories.Add(category);
        await dbContext.SaveChangesAsync();
        return category;
    }

    // Asset Builder
    public async Task<Asset> AssetAsync(Action<AssetBuilderOptions> configure)
    {
        var options = new AssetBuilderOptions();
        configure(options);

        var asset = Asset.Create(
            name: options.Name,
            workspaceId: options.WorkspaceId,
            categoryId: options.CategoryId,
            modelNumber: options.ModelNumber,
            assetTag: options.AssetTag,
            serialNumber: options.SerialNumber,
            manufacturer: options.Manufacturer
        );

        dbContext.Assets.Add(asset);
        await dbContext.SaveChangesAsync();
        return asset;
    }


    // Ticket Builder 
    public async Task<Ticket> TicketAsync(Action<TicketBuilderOptions> configure)
    {
        var options = new TicketBuilderOptions();
        configure(options);

        if (options.WorkspaceId == Guid.Empty) 
            throw new ArgumentException("WorkspaceId is required to seed a ticket.");

        var ticket = Ticket.Create(
            workspaceId: options.WorkspaceId,
            title: options.Title,
            description: options.Description,
            createdByUserId: options.CreatedByUserId,
            type: options.Type,
            priority: options.Priority,
            assetId: null
        );

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync();
        return ticket;
    }
}

public class TicketBuilderOptions
{
    public Guid WorkspaceId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string Title { get; set; } = "Default Ticket Title";
    public string Description { get; set; } = "Default Ticket Description";
    public TicketType Type { get; set; } = TicketType.Incident;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
}

public class AssetBuilderOptions
{
    public Guid WorkspaceId { get; set; }
    public int CategoryId { get; set; }
    public string Name { get; set; } = "Default Asset Name";
    public string ModelNumber { get; set; } = "Default Model Number";
    public string AssetTag { get; set; } = "Default Asset Tag";
    public string SerialNumber { get; set; } = "Default Serial Number";
    public string Manufacturer { get; set; } = "Default Manufacturer";
}