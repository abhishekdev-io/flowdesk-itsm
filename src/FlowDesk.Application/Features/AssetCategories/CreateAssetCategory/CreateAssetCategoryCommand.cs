using FlowDesk.Application.Interface;
using FlowDesk.Application.Common.Permission;
using MediatR;

namespace FlowDesk.Application.Features.AssetCategories.CreateAssetCategory;

[RequirePermission(Permissions.CanManageCategories)]
public sealed record CreateAssetCategoryCommand(
    string Name,
    string Prefix,
    Guid WorkspaceId
) : IRequest<int>, IWorkspaceRequest;



