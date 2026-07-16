using FlowDesk.Application.Behaviours;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Interfaces;
using FlowDesk.Application.UnitTests.Common;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace FlowDesk.Application.UnitTests.Behaviours;

public class AuthorizationBehaviourTest
{
    private readonly ICurrentUserContext _currentUserContext = Substitute.For<ICurrentUserContext>();
    private readonly ICurrentWorkspaceContext _workspaceContext = Substitute.For<ICurrentWorkspaceContext>();
    private readonly IApplicationDbContext _context = DbContextMockFactory.Create();

    private static readonly Guid WorkspaceId = Guid.NewGuid();
    private static readonly Guid UserId = Guid.NewGuid();

    #region Test Requests (De-sugared Syntax)
    private sealed record TestRequest : IWorkspaceRequest, IRequest<string> //Create a test request that implements IWorkspaceRequest and IRequest<string>
    {
        public Guid WorkspaceId { get; init; }
        public TestRequest(Guid workspaceId) => WorkspaceId = workspaceId;
    }
    #endregion

    private AuthorizationBehaviour<TestRequest, string> CreateSut() =>  //Create a method to create the system under test (SUT) for the AuthorizationBehaviour with TestRequest and string as type parameters
        new(_currentUserContext, _workspaceContext, _context);
    
    [Fact]
    public async Task Handle_WhenUserNotAuthenticated_ThrowsUnauthorizedAccessException()
    {
        _currentUserContext.IsAuthenticated.Returns(false);
        var sut = CreateSut();
        var request = new TestRequest(WorkspaceId);
        var nextCalled = false; 
        Task<string> Next(CancellationToken ct) { nextCalled = true; return Task.FromResult("ok"); } 

        var act = async () => await sut.Handle(request, Next, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        nextCalled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenUserNotAWorkspaceMember_ThrowsForbiddenException()
    {
        _currentUserContext.IsAuthenticated.Returns(true);
        _currentUserContext.UserId.Returns(UserId);

        var mockMembers = DbContextMockFactory.MockDbSet(new List<WorkspaceMember>());
            _context.WorkspaceMembers.Returns(mockMembers);

        var sut = CreateSut();
        var request = new TestRequest(WorkspaceId);
        var nextCalled = false;
        Task<string> Next(CancellationToken ct) { nextCalled = true; return Task.FromResult("ok"); }

        var act = async () => await sut.Handle(request, Next, CancellationToken.None);

        await act.Should().ThrowAsync<ForbiddenException>();
        nextCalled.Should().BeFalse();
        _workspaceContext.DidNotReceive().SetWorkspaceContext(Arg.Any<Guid>(), Arg.Any<WorkspaceRole>());
    }

    [Fact]
    public async Task Handle_WhenUserIsWorkspaceMember_SetsWorkspaceContextAndCallsNext()
    {
        _currentUserContext.IsAuthenticated.Returns(true);
        _currentUserContext.UserId.Returns(UserId);

        var membership = WorkspaceMember.Create(
            UserId,
            WorkspaceId,            
            WorkspaceRole.Agent
        );
        var mockMembers = DbContextMockFactory.MockDbSet(new List<WorkspaceMember> { membership });
            _context.WorkspaceMembers.Returns(mockMembers);

        var sut = CreateSut();
        var request = new TestRequest(WorkspaceId);
        static Task<string> Next(CancellationToken ct) => Task.FromResult("handled");

        var result = await sut.Handle(request, Next, CancellationToken.None);

        result.Should().Be("handled");
        _workspaceContext.Received(1).SetWorkspaceContext(WorkspaceId, WorkspaceRole.Agent);
    }
}

