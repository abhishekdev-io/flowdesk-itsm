using System.Net.Http.Json;
using FlowDesk.Application.Features.Workspaces.WorkspaceMember.AddMember;
using FlowDesk.Application.IntegrationTests.Common;
using FlowDesk.Application.IntegrationTests.Common.Helper;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FlowDesk.Application.IntegrationTests.Features.Workspaces;

public class AddMemberTest(IntegrationTestWebAppFactory factory)
 : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Handle_WithValidCommand_ShouldPassBehaviorsAndAddMemberToWorkspace()
    {
        //Arrange
        var workspace = await TestSeeder.WorkspaceAsync(name: "Test-Workspace");

        await TestSeeder.MemberAsync(
            userId: TestUserId,
            workspaceId: workspace.Id,
            role: WorkspaceRole.Admin
        );

        // new member details to be added
        var additionalUserSeeder = new TestUserDataSeeder(Factory.Services); 
        Guid newUserId = await additionalUserSeeder.CreateAdditionalUserAsync("newmember@itsm.com");

        var targetRole = WorkspaceRole.Member;

        var command = new AddMemberCommand(
            UserId: newUserId,
            WorkspaceId: workspace.Id,
            Role: targetRole
        );

        //Act
        var response = await HttpClient.PostAsJsonAsync($"/api/workspaces/{workspace.Id}/members", command);

        //Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var addedMember = await dbContext.WorkspaceMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == newUserId && m.WorkspaceId == workspace.Id);

        addedMember.Should().NotBeNull();
        addedMember!.Role.Should().Be(targetRole);
    }
}
