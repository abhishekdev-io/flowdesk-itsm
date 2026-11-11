using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Tickets.UpdateTicket;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class UpdateTicketTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WhenUserIsCreator_ShouldPersistUpdate()
    {
        // 1. Arrange - workspace + membership (Member role, NOT privileged)
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        // Ticket created BY TestUserId - isCreator branch
        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = TestUserId;
            o.Title = "Printer not working";
            o.Description = "The office printer on 2nd floor is jammed.";
        });

        var command = new UpdateTicketCommand(
            Id: ticket.Id,
            WorkspaceId: workspace.Id,
            Title: "Printer still not working",
            Description: "Updated: still jammed after cycling power."
        );

        // 2. Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}", command);

        // 3. Assert - status code
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        dbContext.ChangeTracker.Clear();

        // 4. Assert - persisted data
        var persistedTicket = await dbContext.Tickets
            .FirstAsync(t => t.Id == ticket.Id);
        
        persistedTicket.Should().NotBeNull();
        persistedTicket!.Title.Value.Should().Be(command.Title);
        persistedTicket.Description.Value.Should().Be(command.Description);
    }

    [Fact]
    public async Task Handle_WhenUserIsPrivilegedButNotCreator_ShouldPersistUpdate()
    {
        // 1. Arrange - workspace + Admin membership for TestUserId
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        // Ticket created by a DIFFERENT user - isPrivileged branch
        var otherUserId = Guid.NewGuid();
        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = otherUserId;
            o.Title = "Laptop won't boot";
            o.Description = "Blue screen on startup.";
        });

        var command = new UpdateTicketCommand(
            Id: ticket.Id,
            WorkspaceId: workspace.Id,
            Title: "Laptop boot issue - escalated",
            Description: "Escalated to hardware team."
        );

        // 2. Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        dbContext.ChangeTracker.Clear();

        var persistedTicket = await dbContext.Tickets
            .FirstAsync(t => t.Id == ticket.Id);
        
        persistedTicket.Should().NotBeNull();
        persistedTicket!.Title.Value.Should().Be(command.Title);
        persistedTicket.Description.Value.Should().Be(command.Description);

    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // 1. Arrange - workspace with NO membership for TestUserId
        var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated-Workspace");

        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Isolated ticket";
        });

        var command = new UpdateTicketCommand(
            Id: ticket.Id,
            WorkspaceId: workspace.Id,
            Title: "Should not persist",
            Description: "Should not persist."
        );

        // 2. Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}", command);
        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenUserIsMemberButNotCreatorOrPrivileged_ShouldReturn403Forbidden()
    {
        // 1. Arrange - workspace + Member role (not privileged) for TestUserId
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        // Ticket created by a DIFFERENT user - fails both isCreator and isPrivileged
        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Someone else's ticket";
        });

        var command = new UpdateTicketCommand(
            Id: ticket.Id,
            WorkspaceId: workspace.Id,
            Title: "Should not persist",
            Description: "Should not persist."
        );

        // 2. Act
        var response = await HttpClient.PutAsJsonAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}", command);
        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenTicketDoesNotExist_ShouldReturn404NotFound()
    {
        // 1. Arrange - valid workspace + membership, but no ticket
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        var nonExistentTicketId = Guid.NewGuid();
        var command = new UpdateTicketCommand(
            Id: nonExistentTicketId,
            WorkspaceId: workspace.Id,
            Title: "Ghost ticket",
            Description: "This ticket does not exist."
        );

        // 2. Act
        var response = await HttpClient.PutAsJsonAsync(
            $"/api/workspaces/{workspace.Id}/tickets/{nonExistentTicketId}", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
