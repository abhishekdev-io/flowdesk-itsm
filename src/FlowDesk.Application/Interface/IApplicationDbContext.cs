using FlowDesk.Domain.Assets;
using FlowDesk.Domain.AssetCategories;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Application.Common.ReadModel;
using FlowDesk.Domain.Notifications;

namespace FlowDesk.Application.Interface;

public interface IApplicationDbContext
{
    DbSet<AssetCategory> AssetCategories {get;}
    DbSet<Asset> Assets {get;}
    DbSet<Ticket> Tickets {get;}
    DbSet<Workspace> Workspaces {get;}
    DbSet<WorkspaceMember> WorkspaceMembers {get;}
    DbSet<TicketComment> TicketComments {get;}
    DbSet<TicketActivity> TicketActivities {get;}
    DbSet<UserNotification> UserNotifications {get;}
    
    //DBView
    DbSet<WorkspaceUserView> WorkspaceUsersView { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
