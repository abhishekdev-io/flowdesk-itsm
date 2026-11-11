using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Assets.GetAsset;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;

namespace FlowDesk.Application.IntegrationTests.Features.Assets;

public class GetAssetByIdTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnAsset()
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
            o.Name = "Developer MacBook Pro";
            o.WorkspaceId = workspace.Id;
            o.CategoryId = category.Id;
            o.ModelNumber = "M3-Max";
            o.SerialNumber = "SN-12345678";
            o.Manufacturer = "Apple";
        });

        // Act
        var response = await HttpClient.GetAsync($"api/v1/workspaces/{workspace.Id}/assets/{asset.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<AssetResponse>();

        result.Should().NotBeNull();
        result!.Id.Should().Be(asset.Id);
        result.Name.Should().Be("Developer MacBook Pro");
        result.AssetTag.Should().Be(asset.AssetTag);
        result.Manufacturer.Should().Be("Apple");
        result.Modelnumber.Should().Be("M3-Max");
        result.Serialnumber.Should().Be("SN-12345678");
        result.WorkspaceId.Should().Be(workspace.Id);

    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

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
        var response = await HttpClient.GetAsync($"api/v1/workspaces/{workspace.Id}/assets/{asset.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
