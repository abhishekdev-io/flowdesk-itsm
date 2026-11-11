using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Workspaces.CreateWorkspace;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.Workspaces;

public class CreateWorkspaceTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldPersistWorkspaceAndCreateAdminMembership()
    {
        // 1. Arrange
        var command = new CreateWorkspaceCommand(
            Name: "Engineering",
            Description: "Engineering team workspace"
        );

        // 2. Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/workspaces", command);

        // 3. Assert - response 
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // 4. Assert - persistence
        var workspace = await dbContext.Workspaces
            .SingleAsync(w => w.Name.Value == "Engineering");
        
        workspace.Should().NotBeNull();
        workspace!.Name.Value.Should().Be("Engineering");

        // 5. Assert - creator was added as Admin member
        var membership = await dbContext.WorkspaceMembers
            .SingleAsync(m => m.WorkspaceId == workspace.Id && m.UserId == TestUserId);

        membership!.Role.Should().Be(WorkspaceRole.Admin);
    }

    [Fact]
    public async Task Handle_WhenNameAlreadyExists_ShouldReturnConflict()
    {
        // 1. Arrange - seed an existing workspace with the same name
        var existing = Workspace.Create(
            name: "Duplicate-Workspace",
            description: "Already exists"
        );
        dbContext.Workspaces.Add(existing);
        await dbContext.SaveChangesAsync();

        var command = new CreateWorkspaceCommand(
            Name: "Duplicate-Workspace",
            Description: "Attempting to create a duplicate"
        );

        // 2. Act
        var response = await HttpClient.PostAsJsonAsync("/api/v1/workspaces", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
