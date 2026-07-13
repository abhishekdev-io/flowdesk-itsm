using System.Net;
using System.Net.Http.Json;
using FlowDesk.Application.Features.Tickets.TicketComments.CreateComment;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.Tickets;

public class CreateCommentTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WhenUserIsTicketCreator_ShouldAddCommentSuccessfully()
    {
        // Arrange
        var workspace = await TestSeeder.WorkspaceAsync();

        await TestSeeder.MemberAsync(TestUserId, workspace.Id, WorkspaceRole.Member);

        var ticket = await TestSeeder.TicketAsync(o => {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = TestUserId; 
        });

        var request = new CreateCommentCommand(
            TicketId: ticket.Id, 
            WorkspaceId: workspace.Id, 
            Description: "This is a comment from the creator."
            );
        
        // Act
        var response = await HttpClient.PostAsJsonAsync($"/api/workspaces/{workspace.Id}/tickets/{ticket.Id}/comments", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        dbContext.ChangeTracker.Clear();

        var comment = await dbContext.TicketComments
            .FirstOrDefaultAsync(c => c.TicketId == ticket.Id);

        comment.Should().NotBeNull();
        comment!.Description.Should().Be(request.Description);
        comment.CreatedBy.Should().Be(TestUserId);
    }

    [Fact]
    public async Task Handle_WhenUserIsRegularMemberAndNotCreator_ShouldReturn403Forbidden()
    {
        // 1. Arrange
        var workspace = await TestSeeder.WorkspaceAsync();
        
        await TestSeeder.MemberAsync(TestUserId, workspace.Id, WorkspaceRole.Member);

        // Ticket created by someone else
        var ticket = await TestSeeder.TicketAsync(o => {
            o.WorkspaceId = workspace.Id;
            o.CreatedByUserId = Guid.NewGuid(); 
        });

        var request = new CreateCommentCommand(
            TicketId: ticket.Id, 
            WorkspaceId: workspace.Id,
            Description: "Malicious comment attempt."
            );

        // 2. Act
        var response = await HttpClient.PostAsJsonAsync($"/api/workspaces/{workspace.Id}/tickets/{ticket.Id}/comments", request);

        // 3. Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
