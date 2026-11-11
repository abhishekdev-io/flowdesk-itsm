using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlowDesk.Application.Features.Tickets.GetTicket;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class GetTicketByIdTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WhenUserIsCreator_ShouldReturnTicketDetails()
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
            o.CreatedByUserId = TestUserId;
            o.Title = "Printer not working";
        });

        // 2. Act
        var response = await HttpClient.GetAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());

        var body = await response.Content.ReadFromJsonAsync<TicketDetailsResponse>(options);

        body.Should().NotBeNull();
        body!.Id.Should().Be(ticket.Id);
        body.CreatedByUser.Should().NotBeNull();
        body.CreatedByUser.UserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task Handle_WhenUserIsPrivilegedButNotCreator_ShouldReturnTicketDetails()
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
            o.CreatedByUserId = Guid.NewGuid(); // Created by someone else
            o.Title = "Laptop won't boot";
        });

        // 2. Act
        var response = await HttpClient.GetAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // 1. Arrange 
        var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated Workspace");

        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid(); // Created by someone else
            o.Title = "Isolated ticket";
        });
        
        // 2. Act
        var response = await HttpClient.GetAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}");
        
        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Handle_WhenMemberViewsTicketTheyDidNotCreate_ShouldReturn403Forbidden()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        // Ticket created by someone else 
        var ticket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid(); // Created by someone else
            o.Title = "Someone else's ticket";
        });
        
        // 2. Act
        var response = await HttpClient.GetAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{ticket.Id}");
        
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

        // 2. Act
        var response = await HttpClient.GetAsync($"/api/v1/workspaces/{workspace.Id}/tickets/{nonExistentTicketId}");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
