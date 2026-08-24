using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;
using FlowDesk.Domain.Tickets.ValueObjects;
using FlowDesk.Domain.Workspaces.Enums;
using FlowDesk.Domain.Assets;

namespace FlowDesk.Domain.Tickets.Aggregates;
 
public sealed class Ticket : AggregateRoot<Guid>
{
    public TicketTitle Title { get; private set; } = null!;
    public int SequenceNumber { get; private set; }
    public string TicketNumber => $"TKT-{SequenceNumber:D5}";
    public TicketDescription Description { get; private set; } = null!;

    public bool IsArchived { get; private set; }
    public DateTime? ArchivedAt { get; private set; }

    public Guid WorkspaceId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid? AssignedToUserId { get; private set; }
    public Guid? AssetId { get; private set; }

    public TicketType Type { get; private set; }
    public TicketStatus Status { get; private set; }
    public TicketPriority Priority { get; private set; }

    public DateTime? AssignedAt { get; private set; }
    public DateTime? DueTime { get; private set; }
    public DateTime? ResolvedAt { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    private readonly List<TicketComment> _comments = [];
    public IReadOnlyCollection<TicketComment> Comments => _comments;

    private readonly List<TicketActivity> _activities = [];
    public IReadOnlyCollection<TicketActivity> Activities => _activities;
    
    public Asset? Asset {get; private set; }

    //SLA 
    public DateTime? SlaDeadlineAt { get; private set; }
    public bool IsSlaBreached { get; private set; }

    private Ticket() : base(Guid.Empty) { }

    private static DateTime CalculateSlaDeadline(DateTime assignedAt, TicketPriority priority)
    {
        int hoursToResolve = priority switch
        {
            TicketPriority.Critical => 2,
            TicketPriority.High => 8,
            TicketPriority.Medium => 24,
            TicketPriority.Low => 72,

            _ => throw new DomainStateException("Invalid ticket priority.")
        };

        return assignedAt.AddHours(hoursToResolve);
    }

    private void EnsureTicketIsEditable()
    {
        if (Status == TicketStatus.Closed)
            throw new DomainStateException("Closed tickets cannot be updated.");
    }

    public static Ticket Create(
        string title,
        string description,
        Guid workspaceId,
        TicketType type,
        TicketPriority priority, 
        Guid createdByUserId,
        Guid? assetId = null,
        DateTime? dueTime = null)
    {
        if (workspaceId == Guid.Empty)
            throw new DomainException("Invalid workspace.");

        if (createdByUserId == Guid.Empty)
            throw new DomainException("Invalid user.");

        if (dueTime.HasValue && dueTime.Value < DateTime.UtcNow)
            throw new DomainException("Due time cannot be in the past.");

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = TicketTitle.Create(title),
            Description = TicketDescription.Create(description),
            WorkspaceId = workspaceId,
            AssetId = assetId,
            Type = type,
            Status = TicketStatus.Open,
            Priority = priority,
            DueTime = dueTime,
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            SlaDeadlineAt = null,
            IsSlaBreached = false,
            IsArchived = false
        };

        ticket.RaiseDomainEvent(new TicketCreatedDomainEvent(
        WorkspaceId: workspaceId,
        TicketId: ticket.Id,
        CreatedBy: ticket.CreatedByUserId,
        Title: ticket.Title,               
        Description: ticket.Description,   
        Priority: ticket.Priority,
        Type: ticket.Type
        ));

        return ticket;
    }
 
    public void Update(
        string title,
        string description,        
        Guid updatedByUserId,
        DateTime? dueTime = null
        )
    {
        EnsureTicketIsEditable();

        var oldTitle = Title;
        var oldDescription = Description;

        Title = TicketTitle.Create(title);
        Description = TicketDescription.Create(description);
        UpdatedAt = DateTime.UtcNow;
        DueTime = dueTime;

        RaiseDomainEvent(new TicketUpdatedDomainEvent(
        TicketId: Id,
        UpdatedBy: updatedByUserId,
        OldTitle: oldTitle,
        NewTitle: Title,
        OldDescription: oldDescription,
        NewDescription: Description
    ));
    }

    // Assign Ticket
    public void Assign(Guid assignedToUserId, Guid assignedByUserId)
    {
        if (AssignedToUserId == assignedToUserId)
            return;

        if (Status != TicketStatus.Open)
            throw new DomainStateException("Only open tickets can be assigned");

        if (assignedToUserId == Guid.Empty)
            throw new DomainStateException("Invalid user.");

        var oldStatus = Status;   

        AssignedToUserId = assignedToUserId;
        AssignedAt = DateTime.UtcNow;
        Status = TicketStatus.InProgress;

        // Start the automated SLA clock based on priority hours
        SlaDeadlineAt = CalculateSlaDeadline(AssignedAt.Value, Priority);
        IsSlaBreached = false;

        RaiseDomainEvent(new TicketAssignDomainEvent(
            WorkspaceId: WorkspaceId,   
            TicketId: Id,
            PerformedBy: assignedByUserId,
            AssignedToId: assignedToUserId,
            Title: Title,
            OldStatus: oldStatus,
            NewStatus: Status
            ));
    }

    // Unassign Ticket
    public void UnAssign(Guid unassignedByUserId)
    {
        if (Status != TicketStatus.InProgress)
            throw new DomainStateException("Only in-progress tickets can be unassigned");

        var oldStatus = Status;    

        var wasAssignedTo = AssignedToUserId;

        AssignedToUserId = null;
        AssignedAt = null;
        Status = TicketStatus.Open;
        UpdatedAt = DateTime.UtcNow;
        SlaDeadlineAt = null;
        IsSlaBreached = false;

        RaiseDomainEvent(new TicketUnAssignDomainEvent(
            WorkspaceId: WorkspaceId, 
            TicketId: Id,
            PerformedBy: unassignedByUserId,
            WasAssignedTo: wasAssignedTo,
            Title: Title,
            OldStatus: oldStatus,
            NewStatus: Status
            ));
    }

    //Archive
    public void Archive(Guid archivedByUserId)
    {
        if(Status is not TicketStatus.Closed)
           throw new DomainStateException("Only Closed ticket can be archived.");

        if(IsArchived) return;

        IsArchived = true;
        ArchivedAt = DateTime.UtcNow;

        RaiseDomainEvent(new TicketArchivedDomainEvent(
            TicketId: Id, 
            PerformedBy: archivedByUserId, 
            IsArchived: true));

    }

    //UnArchive
    public void UnArchive(Guid unarchivedByUserId)
    {
        if(!IsArchived) throw new DomainStateException("Only Archived ticket can be UnArchived.");
        IsArchived = false;
        ArchivedAt = null;

        RaiseDomainEvent(new TicketArchivedDomainEvent(
            TicketId: Id, 
            PerformedBy: unarchivedByUserId, 
            IsArchived: false));
    }

    // Status to Resolved
    public void Resolve(Guid userId)
    {
        if (Status != TicketStatus.InProgress)
            throw new DomainStateException("Only in-progress tickets can be resolved");
        
        var oldStatus = Status; 

        Status = TicketStatus.Resolved;
        ResolvedAt = DateTime.UtcNow;

        if (SlaDeadlineAt.HasValue && ResolvedAt > SlaDeadlineAt)
            IsSlaBreached = true;

        SlaDeadlineAt = null;  

        RaiseDomainEvent(new TicketStatusChangedDomainEvent(
            WorkspaceId: WorkspaceId,
            TicketCreatedBy: CreatedByUserId,
            TicketId: Id,
            ChangedBy: userId,
            TicketNumber: TicketNumber,
            OldStatus: oldStatus,
            NewStatus: Status
        ));  
    }

    // Status to closed
    public void Close(Guid userId)
    {
        if (Status != TicketStatus.Resolved)
            throw new DomainStateException("Only resolved tickets can be closed");

        var oldStatus = Status;    

        Status = TicketStatus.Closed;
        ClosedAt = DateTime.UtcNow;

        //Raise domain event
        RaiseDomainEvent(new TicketStatusChangedDomainEvent(
            WorkspaceId: WorkspaceId,
            TicketCreatedBy: CreatedByUserId,
            TicketId: Id,
            ChangedBy: userId,
            TicketNumber: TicketNumber,
            OldStatus: oldStatus,
            NewStatus: Status            
        ));
    }

    // Status to Reopen
    public void Reopen(Guid reopenedByUserId)
    {
        // Allow either Resolved OR Closed tickets to be reopened
        if (Status != TicketStatus.Resolved && Status != TicketStatus.Closed)
        {
            throw new DomainStateException("Only resolved or closed tickets can be reopened");
        }

        var oldStatus = Status;    

        ResolvedAt = null;
        ClosedAt = null;
        UpdatedAt = DateTime.UtcNow;

        if (AssignedToUserId.HasValue)
        {
            Status = TicketStatus.InProgress;
            AssignedAt = DateTime.UtcNow;
            SlaDeadlineAt = CalculateSlaDeadline(AssignedAt.Value, Priority);
            IsSlaBreached = false;
        }
            
        else
        {
            Status = TicketStatus.Open;
        }

        RaiseDomainEvent(new TicketStatusChangedDomainEvent(
            WorkspaceId: WorkspaceId,
            TicketCreatedBy: CreatedByUserId,
            TicketId: Id,
            ChangedBy: reopenedByUserId,
            TicketNumber: TicketNumber,
            OldStatus: oldStatus,
            NewStatus: Status
            ));
    }

    // Priority Change
    public void ChangePriority(TicketPriority priority, Guid changedByUserId)
    {
        EnsureTicketIsEditable();

        if (Priority == priority) return;

        var oldPriority = Priority;

        Priority = priority;
        UpdatedAt = DateTime.UtcNow;

        if (Status == TicketStatus.InProgress && AssignedAt.HasValue)
        {
            SlaDeadlineAt = CalculateSlaDeadline(AssignedAt.Value, Priority);
        }

        RaiseDomainEvent(new TicketPriorityChangedDomainEvent(
            WorkspaceId: WorkspaceId,
            TicketCreatedBy: CreatedByUserId,
            TicketId: Id, 
            ChangedBy: changedByUserId, 
            TicketNumber: TicketNumber,
            OldPriority: oldPriority,
            NewPriority: Priority));
    }
 
    // Ticket Type Change
    public void ChangeType(TicketType type, Guid changedByUserId)
    {
        EnsureTicketIsEditable();

        if (Type == type) return;

        var oldType = Type;

        Type = type;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new TicketTypeChangedDomainEvent(
            TicketId: Id, 
            ChangedBy: changedByUserId, 
            OldType: oldType, 
            NewType: Type));
    }


    //TicketComment Management
    private TicketComment FindComment(Guid commentId)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId)
            ?? throw new DomainException("Comment not found.");

        return comment;
    }
    public void AddComment(string description, Guid createdBy)
    {
        if (createdBy == Guid.Empty)
            throw new DomainException("Invalid user Id.");

        var comment = TicketComment.Create(Id, description, createdBy);
        _comments.Add(comment);        

        RaiseDomainEvent(new TicketCommentAddedDomainEvent(
            TicketId: Id, 
            CommentId: comment.Id, 
            CreatedBy: createdBy
            ));
    }
    public void EditComment(Guid commentId, Guid userId, string description)
    {
        var comment = FindComment(commentId);

        if(comment.IsDeleted)
            throw new DomainException("Cannot edit deleted comment.");
            
        if (comment.CreatedBy != userId)
            throw new DomainException("Only the comment creator can edit the comment.");
        comment.Edit(description);
    }
    public void DeleteComment(Guid commentId, Guid userId, WorkspaceRole userRole)
    {
        var comment = FindComment(commentId);

        bool isAuthor = comment.CreatedBy == userId;
        bool isPrivileged = userRole is WorkspaceRole.Admin;

        if (!isAuthor && !isPrivileged)
            throw new DomainException("You do not have permission to delete this comment.");
        comment.Delete();
    }
    public void RestoreComment(Guid commentId)
    {
        var comment = FindComment(commentId);
        comment.Restore();
    }
}
