using FlowDesk.Application.Interfaces;
using FlowDesk.Domain.Assets;
using FlowDesk.Domain.AssetCategories;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FlowDesk.Application.Common.ReadModel;
using FlowDesk.Domain.Notifications;
using FlowDesk.Domain.Authentication;

namespace FlowDesk.Infrastructure.Persistence.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
: IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options),IApplicationDbContext
{        
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>(); 
    public DbSet<TicketActivity> TicketActivities => Set<TicketActivity>();
    public DbSet<UserNotification> UserNotifications => Set<UserNotification>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    //ReadModel for DB view
    public DbSet<WorkspaceUserView> WorkspaceUsersView => Set<WorkspaceUserView>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}


