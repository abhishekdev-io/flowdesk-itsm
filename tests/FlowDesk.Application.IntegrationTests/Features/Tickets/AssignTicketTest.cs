using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Tickets.AssignTicket;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class AssignTicketTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WhenUserIsAdmin_ShouldAssignTicketAndPersist()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Printer not working";
            o.Priority = TicketPriority.Medium;
        });

        var assigneeId = Guid.NewGuid();
        var command = new AssignTicketCommand(
            TicketId: ticket.Id,
            AssigneeId: assigneeId,
            WorkspaceId: workspace.Id
        );

        // 2. Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}/assign", command);

        // 3. Assert - status code
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        dbContext.ChangeTracker.Clear();

        // 4. Assert - persisted data
        var persistedTicket = await dbContext.Tickets.FirstAsync(t => t.Id == ticket.Id);
        persistedTicket.AssignedToUserId.Should().Be(assigneeId);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // 1. Arrange 
         var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated-Workspace");

        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Isolated ticket";
        });

        var command = new AssignTicketCommand(
            TicketId: ticket.Id,
            AssigneeId: Guid.NewGuid(),
            WorkspaceId: workspace.Id
        );

        // 2. Act 
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}/assign", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenUserLacksPermission_ShouldReturn403Forbidden()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Some ticket";
            o.Priority = TicketPriority.Medium;
        });

        var command = new AssignTicketCommand(
            TicketId: ticket.Id,
            AssigneeId: Guid.NewGuid(),
            WorkspaceId: workspace.Id
        );

        // 2. Act 
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}/assign", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenTicketDoesNotExist_ShouldReturn404NotFound()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        var nonExistentTicketId = Guid.NewGuid();
        var command = new AssignTicketCommand(
            TicketId: nonExistentTicketId,
            AssigneeId: Guid.NewGuid(),
            WorkspaceId: workspace.Id
        );

        // 2. Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1/workspaces/{workspace.Id}/tickets/{nonExistentTicketId}/assign", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
