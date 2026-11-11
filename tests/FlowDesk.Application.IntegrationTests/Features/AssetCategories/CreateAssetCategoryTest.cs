using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.AssetCategories.CreateAssetCategory;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.AssetCategories;

public class CreateAssetCategoryTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldPassBehaviorsAndPersistCategory()
    {
        // Arrange 
        var workspace = await TestSeeder.WorkspaceAsync(name: "IT-Operations");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        var command = new CreateAssetCategoryCommand(
            Name: "Laptops",
            Prefix: "LAP",
            WorkspaceId: workspace.Id
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/categories", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        result.Should().NotBeNull();
        int newCategoryId = result!.Id;

        // Verify Database State
        dbContext.ChangeTracker.Clear();

        var category = await dbContext.AssetCategories
            .FirstOrDefaultAsync(c => c.Id == newCategoryId);

        category.Should().NotBeNull();
        category!.Name.Value.Should().Be(command.Name);
        category.Prefix.Should().Be(command.Prefix);
        category.WorkspaceId.Should().Be(workspace.Id);
    }
}
