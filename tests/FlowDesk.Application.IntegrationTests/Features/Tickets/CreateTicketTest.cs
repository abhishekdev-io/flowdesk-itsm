using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Tickets.CreateTicket;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class CreateTicketTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldPassBehaviorsAndPersistTicket()
    {
        // Arrange - Parent aggregate root must be persisted first
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        var command = new CreateTicketCommand(
            WorkspaceId: workspace.Id,
            Title: "Printer not working",
            Description: "The office printer on 2nd floor is jammed and won't print.",
            AssetId: null,
            Type: TicketType.Incident,
            Priority: TicketPriority.Medium
        );

        //  Act
        var response = await HttpClient.PostAsJsonAsync($"/api/workspaces/{workspace.Id}/tickets", command);

        //  Assert - status code
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        //  Assert - actual persisted data
        var persistedTicket = await dbContext.Tickets
            .FirstOrDefaultAsync(t => t.WorkspaceId == workspace.Id);
        
        persistedTicket.Should().NotBeNull();
        persistedTicket!.Title.Value.Should().Be(command.Title);
        persistedTicket.Description.Value.Should().Be(command.Description);
        persistedTicket.CreatedByUserId.Should().Be(TestUserId);
        persistedTicket.Type.Should().Be(command.Type);
        persistedTicket.Priority.Should().Be(command.Priority);
        persistedTicket.AssetId.Should().BeNull();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        //  Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated-Workspace");

        var command = new CreateTicketCommand(
            WorkspaceId: workspace.Id,
            Title: "Isolated ticket",
            Description: "Should never be created.",
            AssetId: null,
            Type: TicketType.Incident,
            Priority: TicketPriority.Low
        );

        //  Act
        var response = await HttpClient.PostAsJsonAsync($"/api/workspaces/{workspace.Id}/tickets", command);

        //  Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Handle_CreateTicket_ShouldPersistTicketAndTriggerSideEffects()
    {
        //Arrange
        var workspace = await TestSeeder.WorkspaceAsync();

        await TestSeeder.MemberAsync(
        userId: TestUserId,
        workspaceId: workspace.Id,
        role: WorkspaceRole.Admin 
        );

        var command = new CreateTicketCommand(
            WorkspaceId: workspace.Id,
            Title: "Network outage",
            Description: "The office network is down.",
            AssetId: null,
            Type: TicketType.Incident,
            Priority: TicketPriority.High
        );

        //Act
        var response = await HttpClient.PostAsJsonAsync($"/api/workspaces/{workspace.Id}/tickets", command);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var result = await response.Content.ReadFromJsonAsync<CreateTicketResponse>();
        result.Should().NotBeNull();
        Guid newTicketId = result!.Id;

        dbContext.ChangeTracker.Clear();

        //Assert Side-Effect: Was a TicketActivity logged?
        var activity = await dbContext.TicketActivities
            .FirstOrDefaultAsync(a => a.TicketId == newTicketId);

        activity.Should().NotBeNull();

        // Assert Side-Effect: Was a Notification dispatched/saved?
        var notification = await dbContext.UserNotifications
            .FirstOrDefaultAsync(n => n.RelatedTicketId == newTicketId && n.WorkspaceId == workspace.Id); 
        
        notification.Should().NotBeNull();
    }
}


