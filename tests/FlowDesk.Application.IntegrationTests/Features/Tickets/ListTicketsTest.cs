using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using FlowDesk.Application.Common.Pagination;
using FlowDesk.Application.Features.Tickets.ListTickets;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class ListTicketsTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WhenUserIsAdmin_ShouldReturnAllWorkspaceTickets()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        // Two tickets created by two DIFFERENT users - Admin should see both,
        // proving the switch falls through to the unfiltered branch.

        var ticketOne = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Ticket by user A";
        });
        var ticketTwo = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Ticket by user B";
        });

        // 2. Act
        var response = await HttpClient.GetAsync($"/api/workspaces/{workspace.Id}/tickets");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());

        var result = await response.Content.ReadFromJsonAsync<PagedResult<TicketSummaryResponse>>(options);

        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(2);
        result.Items.Should().Contain(t => t.Id == ticketOne.Id);
        result.Items.Should().Contain(t => t.Id == ticketTwo.Id);
    }

    [Fact]
    public async Task Handle_WhenUserIsMember_ShouldReturnOnlyTicketsTheyCreated()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        // TestUserId's own ticket - should appear
        var ownTicket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = TestUserId;
            o.Title = "My own ticket";
        });

        // Someone else's ticket in the SAME workspace - should be filtered out
        var othersTicket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Someone else's ticket";
        });

        // 2. Act
        var response = await HttpClient.GetAsync($"/api/workspaces/{workspace.Id}/tickets");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());

        var result = await response.Content.ReadFromJsonAsync<PagedResult<TicketSummaryResponse>>(options);
        result!.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(t => t.Id == ownTicket.Id);
        result.Items.Should().NotContain(t => t.Id == othersTicket.Id);
    }

    [Fact]
    public async Task Handle_ShouldExcludeArchivedTickets()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        var activeTicket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = TestUserId;
            o.Title = "Active ticket";
        });

        var archivedTicket = Ticket.Create(
            workspaceId: workspace.Id,
            title: "Archived ticket",
            description: "Should be excluded from the default list.",
            createdByUserId: TestUserId,
            type: TicketType.Incident,
            priority: TicketPriority.Low,
            assetId: null
        );
        archivedTicket.Assign(assignedToUserId: Guid.NewGuid(), assignedByUserId: TestUserId); //Only assigned tickets can be resolved
        archivedTicket.Resolve(TestUserId); //Only resolved tickets can be closed
        archivedTicket.Close(TestUserId,"close-ticket"); //Only closed tickets can be archived
        archivedTicket.Archive(TestUserId);

        dbContext.Tickets.Add(archivedTicket);
        await dbContext.SaveChangesAsync();

        // 2. Act
        var response = await HttpClient.GetAsync($"/api/workspaces/{workspace.Id}/tickets");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());
            
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TicketSummaryResponse>>(options);
        result!.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(t => t.Id == activeTicket.Id);
    }

    [Fact]
    public async Task Handle_WithStatusFilter_ShouldReturnOnlyMatchingTickets()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Member
        );

        var openTicket = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = TestUserId;
            o.Title = "Open ticket";
        });

        var closedTicket = Ticket.Create(
            workspaceId: workspace.Id,
            title: "Closed ticket",
            description: "Already resolved.",
            createdByUserId: TestUserId,
            type: TicketType.Incident,
            priority: TicketPriority.Medium,
            assetId: null
        );
        closedTicket.Assign(assignedToUserId: Guid.NewGuid(), assignedByUserId: TestUserId); //Only assigned tickets can be resolved
        closedTicket.Resolve(TestUserId); //Only resolved tickets can be closed
        closedTicket.Close(TestUserId, "Resolved.");

        dbContext.Tickets.Add(closedTicket);
        await dbContext.SaveChangesAsync();

        // 2. Act
        var response = await HttpClient.GetAsync(
            $"/api/workspaces/{workspace.Id}/tickets?Status={TicketStatus.Closed}");

        // 3. Assert
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());
            
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TicketSummaryResponse>>(options);
        result!.TotalCount.Should().Be(1);
        result.Items.Should().ContainSingle(t => t.Id == closedTicket.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectPage_WhenPaginated()
    {
        // 1. Arrange - 3 tickets, page size 2 -> page 1 has 2, page 2 has 1
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        for (var i = 1; i <= 3; i++)
        {
            await TestSeeder.TicketAsync(o =>
            {
                o.WorkspaceId = workspace.Id;
                o.CreatedByUserId = TestUserId;
                o.Title = $"Ticket {i}";
                o.Description = "Pagination test.";
            });
        }

        // 2. Act
        var response = await HttpClient.GetAsync(
            $"/api/workspaces/{workspace.Id}/tickets?Page=2&PageSize=2");

        // 3. Assert
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new JsonStringEnumConverter());
            
        var result = await response.Content.ReadFromJsonAsync<PagedResult<TicketSummaryResponse>>(options);

        result!.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(1);
        result.Page.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // 1. Arrange 
        var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated Workspace");

        // 2. Act 
        var response = await HttpClient.GetAsync($"/api/workspaces/{workspace.Id}/tickets");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
 