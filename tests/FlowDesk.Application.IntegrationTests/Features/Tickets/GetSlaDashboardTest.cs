using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Tickets.GetSlaDashboard;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class GetSlaDashboardTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WhenUserIsAdmin_ShouldReturnDashboardReflectingWorkspaceTickets()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        // Two tickets in this workspace
        var ticketOne = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Ticket One";
            o.Priority = TicketPriority.Medium;
        });

        var ticketTwo = await TestSeeder.TicketAsync(o =>
        {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid();
            o.Title = "Ticket Two";
            o.Priority = TicketPriority.High;
        });

        // 2. Act
        var response = await HttpClient.GetAsync($"/api/workspaces/{workspace.Id}/tickets/sla-dashboard");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dashboard = await response.Content.ReadFromJsonAsync<SlaDashboardDto>();
        dashboard.Should().NotBeNull();
        dashboard!.Total.Should().Be(2);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotMemberOfWorkspace_ShouldReturn403Forbidden()
    {
        // 1. Arrange 
        var workspace = await TestSeeder.WorkspaceAsync(name: "Unrelated Workspace");

        // 2. Act 
        var response = await HttpClient.GetAsync($"/api/workspaces/{workspace.Id}/tickets/sla-dashboard");

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

        // 2. Act 
        var response = await HttpClient.GetAsync($"/api/workspaces/{workspace.Id}/tickets/sla-dashboard");

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
