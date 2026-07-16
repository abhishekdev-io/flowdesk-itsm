using FlowDesk.Application.Behaviours;
using FlowDesk.Application.Common.Exceptions;
using FlowDesk.Application.Common.Permission;
using FlowDesk.Application.Interfaces;
using FluentAssertions;
using MediatR;
using NSubstitute;

namespace FlowDesk.Application.UnitTests.Behaviours;

public class PermissionBehaviourTest
{
    private readonly ICurrentWorkspaceContext _workspaceContext = Substitute.For<ICurrentWorkspaceContext>();
    private static readonly Guid WorkspaceId = Guid.NewGuid();

    private PermissionBehaviour<TRequest, string> CreateSut<TRequest>() 
        where TRequest : IWorkspaceRequest 
    {
        var permissionService = new PermissionService(_workspaceContext);
        return new PermissionBehaviour<TRequest, string>(permissionService);
    }

    #region Test Requests (De-sugared Syntax)
    [RequirePermission(Permissions.CanManageAssets)]
    private sealed record ManageAssetsRequest : IWorkspaceRequest, IRequest<string>
    {
        public Guid WorkspaceId { get; init; }
        public ManageAssetsRequest(Guid workspaceId) => WorkspaceId = workspaceId;
    }
    #endregion

    [Fact]
    public async Task Handle_WhenPermissionIsGranted_CallsNext()
    {
        // Arrange
        _workspaceContext.IsAdmin.Returns(true); 

        var sut = CreateSut<ManageAssetsRequest>();
        var request = new ManageAssetsRequest(WorkspaceId);
        var nextCalled = false;
        Task<string> Next(CancellationToken ct) { nextCalled = true; return Task.FromResult("success"); }

        // Act
        var result = await sut.Handle(request, Next, CancellationToken.None);

        // Assert
        result.Should().Be("success");
        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenPermissionIsDenied_ThrowsForbiddenException()
    {
        // Arrange
        _workspaceContext.IsAdmin.Returns(false); 

        var sut = CreateSut<ManageAssetsRequest>();
        var request = new ManageAssetsRequest(WorkspaceId);
        var nextCalled = false;
        Task<string> Next(CancellationToken ct) { nextCalled = true; return Task.FromResult("ok"); }

        // Act
        var act = async () => await sut.Handle(request, Next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ForbiddenException>();
        nextCalled.Should().BeFalse();
    }
}
