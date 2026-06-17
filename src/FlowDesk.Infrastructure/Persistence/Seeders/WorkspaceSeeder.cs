using FlowDesk.Domain.AssetCategories;
using FlowDesk.Domain.Assets;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FlowDesk.Infrastructure.Identity.Entity;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Infrastructure.Persistence.Seeders;

public static class WorkspaceSeeder
{
    public static async Task<(Workspace Workspace, Dictionary<string, Asset> SeededAssets)> SeedWorkspaceContextAsync(
        ApplicationDbContext dbContext,
        AppUser localAdmin,
        AppUser agent1,
        AppUser agent2,
        AppUser member1,
        AppUser member2,
        AppUser member3)
    {
        //Creating Workspace.
        var workspace = Workspace.Create(
            "Acme Technologies",
            "Enterprise technology solutions provider managing IT infrastructure, end-user support, networking, cybersecurity, and business-critical systems.");


        //adding workspace-member.
        workspace.AddMember(localAdmin.Id, WorkspaceRole.Admin);
        workspace.AddMember(agent1.Id, WorkspaceRole.Agent);
        workspace.AddMember(agent2.Id, WorkspaceRole.Agent);
        workspace.AddMember(member1.Id, WorkspaceRole.Member);
        workspace.AddMember(member2.Id, WorkspaceRole.Member);
        workspace.AddMember(member3.Id, WorkspaceRole.Member);

        await dbContext.Workspaces.AddAsync(workspace);
        await dbContext.SaveChangesAsync();

        //Asset Category
        var laptops = AssetCategory.Create("Laptops", workspace.Id,"LAP");
        var servers = AssetCategory.Create("Servers", workspace.Id, "SVR");
        var networking = AssetCategory.Create("Networking", workspace.Id, "NET");
        var printers = AssetCategory.Create("Printers", workspace.Id, "PRT");        

        await dbContext.AssetCategories.AddRangeAsync(
            laptops,
            servers,
            networking,
            printers
        );
        await dbContext.SaveChangesAsync();

        //Assets
        var dellLaptop = Asset.Create(
            "Dell Latitude 5440",
            workspace.Id,
            laptops.Id,
            "Latitude 5440",
            laptops.GenerateAssetTag(),
            "DL5440001",
            "Dell");

        var macbook = Asset.Create(
            "MacBook Pro M4",
            workspace.Id,
            laptops.Id,
            "MacBook Pro M4",
            laptops.GenerateAssetTag(),
            "MBP4001",
            "Apple");   

        var ciscoSwitch = Asset.Create(
            "Cisco Catalyst 9200",
            workspace.Id,
            networking.Id,
            "Catalyst 9200",
            networking.GenerateAssetTag(),
            "CSC9200001",
            "Cisco");
            
        var fortinet = Asset.Create(
            "Fortinet FG-100F",
            workspace.Id,
            networking.Id,
            "FG-100F",
            networking.GenerateAssetTag(),
            "FT100F001",
            "Fortinet");

        var dellServer = Asset.Create(
            "Dell PowerEdge R760",
            workspace.Id,
            servers.Id,
            "PowerEdge R760",
            servers.GenerateAssetTag(),
            "R7600001",
            "Dell");

        var hpServer = Asset.Create(
            "HP ProLiant DL380",
            workspace.Id,
            servers.Id,
            "ProLiant DL380",
            servers.GenerateAssetTag(),
            "DL3800001",
            "HP");

        var hpPrinter = Asset.Create(
            "HP LaserJet Pro",
            workspace.Id,
            printers.Id,
            "LaserJet Pro",
            printers.GenerateAssetTag(),
            "HPLJ0001",
            "HP");

        var cannonPrinter = Asset.Create(
            "Canon ImageRunner",
            workspace.Id,
            printers.Id,
            "ImageRunner",
            printers.GenerateAssetTag(),
            "CIR0001",
            "Canon"); 

        dbContext.AssetCategories.UpdateRange(
            laptops, 
            servers, 
            networking, 
            printers);          

        await dbContext.Assets.AddRangeAsync(
            dellLaptop,
            macbook,
            ciscoSwitch,
            fortinet,
            dellServer,
            hpServer,
            hpPrinter,
            cannonPrinter);   

        await dbContext.SaveChangesAsync();  

        var assetLookup = new Dictionary<string, Asset>
        {
            { "DellLaptop", dellLaptop },
            { "Macbook", macbook },
            { "Switch", ciscoSwitch },
            { "DellServer", dellServer },
            { "HpServer", hpServer },
            { "CannonPrinter", cannonPrinter}
        };

        return (workspace, assetLookup);           
        
    }
}
