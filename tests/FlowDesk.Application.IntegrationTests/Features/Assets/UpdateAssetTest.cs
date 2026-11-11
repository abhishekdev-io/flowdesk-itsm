using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Assets.UpdateAsset;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Assets;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;

namespace FlowDesk.Application.IntegrationTests.Features.Assets;

public class UpdateAssetTest(IntegrationTestWebAppFactory factory)
    : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WithValidUpdates_ShouldModifyAssetInDatabase()
    {
        // Arrange
       var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        var category = await TestSeeder.AssetCategoryAsync(
            name: "Laptop",
            workspaceId: workspace.Id,
            prefix: "LAP"
        );

        var asset = await TestSeeder.AssetAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CategoryId = category.Id;
        });

        var command = new UpdateAssetCommand(
            Name: "Upgraded MacBook Pro",
            AssetId: asset.Id,
            CategoryId: category.Id,
            Modelnumber: "M3-Max",
            Manufacturer: "Apple",
            Serialnumber: "SN-NEW",
            WorkspaceId: workspace.Id
        );

        // Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/assets/{asset.Id}", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        dbContext.ChangeTracker.Clear();
        var updatedAsset = await dbContext.Assets.FindAsync(asset.Id);
        
        updatedAsset!.ModelNumber.Should().Be("M3-Max");
        updatedAsset.SerialNumber.Should().Be("SN-NEW");
    }

    [Fact]
    public async Task Decommission_WhenAssetIsActive_ShouldTransitionStateSuccessfully()
    {
        // Arrange - Seed prerequisite context and active asset
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        var category = await TestSeeder.AssetCategoryAsync(
            name: "Laptop",
            workspaceId: workspace.Id,
            prefix: "LAP"
        );

        var asset = await TestSeeder.AssetAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CategoryId = category.Id;
        });

        // Act
        var response = await HttpClient.PatchAsync($"/api/v1/workspaces/{workspace.Id}/assets/{asset.Id}/decommission", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        dbContext.ChangeTracker.Clear();
        var decommissionedAsset = await dbContext.Assets.FindAsync(asset.Id);
        
        decommissionedAsset!.Status.Should().Be(AssetStatus.Decommissioned);
        decommissionedAsset.DecommissionedAt.Should().NotBeNull();
    }
}
