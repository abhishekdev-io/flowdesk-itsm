using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Tickets.CloseTicket;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class CloseTicketTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WhenUserIsMember_ShouldCloseTicketAndPersist()
    {
        // 1. Arrange 
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        var ticket = Ticket.Create(
            workspaceId: workspace.Id,
            title: "Printer not working",
            description: "The office printer on 2nd floor is jammed.",
            createdByUserId: Guid.NewGuid(),
            type: TicketType.Incident,
            priority: TicketPriority.Medium,
            assetId: null
        );
        ticket.Assign(TestUserId, TestUserId); //Only assigned tickets can be resolved
        ticket.Resolve(TestUserId); //Only resolved tickets can be closed

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync();

        var command = new CloseTicketCommand(
            TicketId: ticket.Id,
            WorkspaceId: workspace.Id
        );

        // 2. Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}/close", command);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        dbContext.ChangeTracker.Clear();

        var persistedTicket = await dbContext.Tickets.FirstAsync(t => t.Id == ticket.Id);
        persistedTicket.Status.Should().Be(TicketStatus.Closed);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // 1. Arrange 
        var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated-Workspace");

        var ticket = Ticket.Create(
            workspaceId: workspace.Id,
            title: "Isolated ticket",
            description: "Should never be closed.",
            createdByUserId: Guid.NewGuid(),
            type: TicketType.Incident,
            priority: TicketPriority.Low,
            assetId: null
        );
        ticket.Assign(TestUserId, TestUserId); //Only assigned tickets can be resolved
        ticket.Resolve(TestUserId); //Only resolved tickets can be closed
        
        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync();

        var command = new CloseTicketCommand(
            TicketId: ticket.Id,
            WorkspaceId: workspace.Id
        );

        // 2. Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}/close", command);
        
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
            role: WorkspaceRole.Member
        );

        var nonExistentTicketId = Guid.NewGuid();
        var command = new CloseTicketCommand(
            TicketId: nonExistentTicketId,
            WorkspaceId: workspace.Id
        );

        // 2. Act
        var response = await HttpClient.PatchAsJsonAsync(
            $"/api/v1/workspaces/{workspace.Id}/tickets/{nonExistentTicketId}/close", command);
        
        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
