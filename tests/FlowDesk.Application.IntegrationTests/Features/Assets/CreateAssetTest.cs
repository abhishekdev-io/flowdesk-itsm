using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Assets.CreateAsset;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;

namespace FlowDesk.Application.IntegrationTests.Features.Assets;

public class CreateAssetTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{

    [Fact]
    public async Task Handle_WithValidCommand_ShouldPassBehaviorsAndPersistAsset()
    {
        // 1. Arrange 
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

        var command = new CreateAssetCommand(
            Name: "Developer MacBook Pro",
            CategoryId: category.Id,
            Modelnumber: "M3-Max",
            Serialnumber: "SN-12345678",
            Manufacturer: "Apple",
            WorkspaceId: workspace.Id
        );

        // 3. Act
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/assets", command);

        // 4. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }
    
    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // 1. Arrange 
        var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated-Workspace");
        
        var category = await TestSeeder.AssetCategoryAsync(
            name: "Generic Equipment",
            workspaceId: workspace.Id,
            prefix: "GNE"
        );

        var command = new CreateAssetCommand(
            Name: "Isolated Machine",
            CategoryId: category.Id,
            Modelnumber: "Unknown",
            Serialnumber: "None",
            Manufacturer: "Generic",
            WorkspaceId: workspace.Id
        );

        // 2. Act
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/assets", command);

        // 3. Assert 
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
