using System.Reflection;
using FlowDesk.Domain.Exceptions;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Tickets.Events;
using FlowDesk.Domain.Workspaces.Enums;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.Tickets;

public class TicketTests
{
    #region Test Helpers
    private static readonly Guid WorkspaceId = Guid.NewGuid();
    private static readonly Guid CreatedByUserId = Guid.NewGuid();

    private static TicketType AnyTicketType() => Enum.GetValues<TicketType>().First();

    private static TicketType AnotherTicketType(TicketType exclude) =>
        Enum.GetValues<TicketType>().First(t => t != exclude);
    
    private static Ticket CreateOpenTicket(
        TicketPriority priority = TicketPriority.Medium,
        TicketType? type = null,
        DateTime? dueTime = null,
        Guid? workspaceId = null,
        Guid? createdByUserId = null)
    {
        return Ticket.Create(
            title: "Printer on 3rd floor is offline",
            description: "The office printer stopped responding to print jobs this morning.",
            workspaceId: workspaceId ?? WorkspaceId,
            type: type ?? AnyTicketType(),
            priority: priority,
            createdByUserId: createdByUserId ?? CreatedByUserId,
            assetId: null,
            dueTime: dueTime);
    }

    private static Ticket CreateAssignedTicket(out Guid assignee, TicketPriority priority = TicketPriority.Medium)
    {
        var ticket = CreateOpenTicket(priority);
        assignee = Guid.NewGuid();
        ticket.Assign(assignee, Guid.NewGuid());
        return ticket;
    }

    private static Ticket CreateResolvedTicket(TicketPriority priority = TicketPriority.Medium)
    {
        var ticket = CreateAssignedTicket(out _, priority);
        ticket.Resolve(Guid.NewGuid());
        return ticket;
    }

    private static Ticket CreateClosedTicket()
    {
        var ticket = CreateResolvedTicket();
        ticket.Close(Guid.NewGuid());
        return ticket;
    }

    private static void SetPrivateProperty<T>(object target, string propertyName, T value)
    {
        var property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
            ?? throw new InvalidOperationException($"Property '{propertyName}' not found on {target.GetType().Name}.");
 
        var setter = property.GetSetMethod(nonPublic: true)
            ?? throw new InvalidOperationException($"Property '{propertyName}' has no setter.");
 
        setter.Invoke(target, [value]);
    }

    #endregion

    #region Create Ticket

    [Fact]
    public void Create_WithValidData_ShouldInitializeTicketAsOpen()
    {
        var ticket = CreateOpenTicket();
 
        ticket.Status.Should().Be(TicketStatus.Open);
        ticket.IsArchived.Should().BeFalse();
        ticket.SlaDeadlineAt.Should().BeNull();
        ticket.IsSlaBreached.Should().BeFalse();
        ticket.AssignedToUserId.Should().BeNull();
        ticket.SequenceNumber.Should().Be(0);
    }

    [Fact]
    public void Create_ShouldRaiseTicketCreatedDomainEvent()
    {
        var ticket = CreateOpenTicket();
 
        var domainEvent = ticket.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TicketCreatedDomainEvent>().Subject;
 
        domainEvent.TicketId.Should().Be(ticket.Id);
        domainEvent.WorkspaceId.Should().Be(WorkspaceId);
        domainEvent.CreatedBy.Should().Be(CreatedByUserId);
    }

    [Fact]
    public void Create_WithEmptyWorkspaceId_ShouldThrowDomainException()
    {
        var act = () => CreateOpenTicket(workspaceId: Guid.Empty);
 
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithEmptyCreatedByUserId_ShouldThrowDomainException()
    {
        var act = () => CreateOpenTicket(createdByUserId: Guid.Empty);
 
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_WithDueTimeInThePast_ShouldThrowDomainException()
    {
        var act = () => CreateOpenTicket(dueTime: DateTime.UtcNow.AddDays(-1));
 
        act.Should().Throw<DomainException>();
    }

    #endregion

    #region  Update Ticket

    [Fact]
    public void Update_WhenEditable_ShouldChangeTitleAndDescriptionAndUpdatedAt()
    {
        var ticket = CreateOpenTicket();
        var newDueTime = DateTime.UtcNow.AddDays(3);
 
        ticket.Update("Printer needs a new toner cartridge", "Toner light is blinking red.", Guid.NewGuid(), newDueTime);
 
        ticket.Title.ToString().Should().Contain("toner cartridge");
        ticket.Description.ToString().Should().Contain("Toner light");
        ticket.DueTime.Should().Be(newDueTime);
        ticket.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Update_ShouldRaiseTicketUpdatedDomainEventWithOldAndNewValues()
    {
        var ticket = CreateOpenTicket();
        var oldTitle = ticket.Title;
        var updatedBy = Guid.NewGuid();
 
        ticket.Update("New title text here", "New description text here.", updatedBy);
 
        var domainEvent = ticket.DomainEvents.OfType<TicketUpdatedDomainEvent>().Should().ContainSingle().Subject;
        domainEvent.OldTitle.Should().Be(oldTitle);
        domainEvent.NewTitle.Should().Be(ticket.Title);
        domainEvent.UpdatedBy.Should().Be(updatedBy);
    }

    #endregion

    #region Assign Ticket

    [Fact]
    public void Assign_WhenOpen_ShouldMoveToInProgressAndStartSlaClock()
    {
        var ticket = CreateOpenTicket(TicketPriority.High);
        var assignee = Guid.NewGuid();
 
        ticket.Assign(assignee, Guid.NewGuid());
 
        ticket.Status.Should().Be(TicketStatus.InProgress);
        ticket.AssignedToUserId.Should().Be(assignee);
        ticket.AssignedAt.Should().NotBeNull();
        ticket.IsSlaBreached.Should().BeFalse();
    }

    [Theory]
    [InlineData(TicketPriority.Critical, 2)]
    [InlineData(TicketPriority.High, 8)]
    [InlineData(TicketPriority.Medium, 24)]
    [InlineData(TicketPriority.Low, 72)]
    public void Assign_ShouldCalculateSlaDeadlineBasedOnPriority(TicketPriority priority, int expectedHours)
    {
        var ticket = CreateOpenTicket(priority);
 
        ticket.Assign(Guid.NewGuid(), Guid.NewGuid());
 
        ticket.SlaDeadlineAt.Should().Be(ticket.AssignedAt!.Value.AddHours(expectedHours));
    }

    [Fact]
    public void Assign_ShouldRaiseTicketAssignDomainEvent()
    {
        var ticket = CreateOpenTicket();
        var assignee = Guid.NewGuid();
        var assigner = Guid.NewGuid();
 
        ticket.Assign(assignee, assigner);
 
        var domainEvent = ticket.DomainEvents.OfType<TicketAssignDomainEvent>().Should().ContainSingle().Subject;
        domainEvent.AssignedToId.Should().Be(assignee);
        domainEvent.PerformedBy.Should().Be(assigner);
        domainEvent.OldStatus.Should().Be(TicketStatus.Open);
        domainEvent.NewStatus.Should().Be(TicketStatus.InProgress);
    }

    [Fact]
    public void Assign_WithEmptyAssigneeId_ShouldThrowDomainStateException()
    {
        var ticket = CreateOpenTicket();
 
        var act = () => ticket.Assign(Guid.Empty, Guid.NewGuid());
 
        act.Should().Throw<DomainStateException>();
    }

    #endregion

    #region UnAssign Ticket

    [Fact]
    public void UnAssign_WhenInProgress_ShouldResetToOpenAndClearSla()
    {
        var ticket = CreateAssignedTicket(out _);
 
        ticket.UnAssign(Guid.NewGuid());
 
        ticket.Status.Should().Be(TicketStatus.Open);
        ticket.AssignedToUserId.Should().BeNull();
        ticket.AssignedAt.Should().BeNull();
        ticket.SlaDeadlineAt.Should().BeNull();
        ticket.IsSlaBreached.Should().BeFalse();
    }

    [Fact]
    public void UnAssign_ShouldRaiseTicketUnAssignDomainEventWithPreviousAssignee()
    {
        var ticket = CreateAssignedTicket(out var assignee);
        var performedBy = Guid.NewGuid();
 
        ticket.UnAssign(performedBy);
 
        var domainEvent = ticket.DomainEvents.OfType<TicketUnAssignDomainEvent>().Should().ContainSingle().Subject;
        domainEvent.WasAssignedTo.Should().Be(assignee);
        domainEvent.PerformedBy.Should().Be(performedBy);
        domainEvent.NewStatus.Should().Be(TicketStatus.Open);
    }

    #endregion

    #region Archive / UnArchive Ticket

    [Fact]
    public void Archive_WhenClosed_ShouldSetArchivedTrueWithTimestamp()
    {
        var ticket = CreateClosedTicket();
 
        ticket.Archive(Guid.NewGuid());
 
        ticket.IsArchived.Should().BeTrue();
        ticket.ArchivedAt.Should().NotBeNull();
    }

    [Fact]
    public void Archive_ShouldRaiseTicketArchivedDomainEventWithIsArchivedTrue()
    {
        var ticket = CreateClosedTicket();
        var performedBy = Guid.NewGuid();
 
        ticket.Archive(performedBy);
 
        var domainEvent = ticket.DomainEvents.OfType<TicketArchivedDomainEvent>().Should().ContainSingle().Subject;
        domainEvent.IsArchived.Should().BeTrue();
        domainEvent.PerformedBy.Should().Be(performedBy);
    }

    [Fact]
    public void UnArchive_WhenArchived_ShouldClearArchivedFlagAndTimestamp()
    {
        var ticket = CreateClosedTicket();
        ticket.Archive(Guid.NewGuid());
 
        ticket.UnArchive(Guid.NewGuid());
 
        ticket.IsArchived.Should().BeFalse();
        ticket.ArchivedAt.Should().BeNull();
    }

    #endregion

    #region Resolve Ticket

    [Fact]
    public void Resolve_WhenInProgress_ShouldSetResolvedStatusAndClearSlaDeadline()
    {
        var ticket = CreateAssignedTicket(out _);
 
        ticket.Resolve(Guid.NewGuid());
 
        ticket.Status.Should().Be(TicketStatus.Resolved);
        ticket.ResolvedAt.Should().NotBeNull();
        ticket.SlaDeadlineAt.Should().BeNull();
    }

    [Theory]
    [InlineData(true)]  // deadline already passed -> breached
    [InlineData(false)] // deadline still in the future -> not breached
    public void Resolve_ShouldSetSlaBreachedFlagBasedOnWhetherDeadlineHasPassed(bool deadlineHasPassed)
    {
        var ticket = CreateAssignedTicket(out _, TicketPriority.Low); // 72h window, plenty of headroom by default
        if (deadlineHasPassed)
            SetPrivateProperty(ticket, nameof(Ticket.SlaDeadlineAt), DateTime.UtcNow.AddHours(-1));
 
        ticket.Resolve(Guid.NewGuid());
 
        ticket.IsSlaBreached.Should().Be(deadlineHasPassed);
    }

    [Fact]
    public void Resolve_ShouldRaiseTicketStatusChangedDomainEvent()
    {
        var ticket = CreateAssignedTicket(out _);
        var changedBy = Guid.NewGuid();
 
        ticket.Resolve(changedBy);
 
        var domainEvent = ticket.DomainEvents.OfType<TicketStatusChangedDomainEvent>().Should().ContainSingle().Subject;
        domainEvent.OldStatus.Should().Be(TicketStatus.InProgress);
        domainEvent.NewStatus.Should().Be(TicketStatus.Resolved);
        domainEvent.ChangedBy.Should().Be(changedBy);
    }

    #endregion

    #region Close Ticket

    [Fact]
    public void Close_ShouldRaiseTicketStatusChangedDomainEventWithReason()
    {
        var ticket = CreateResolvedTicket();
        var changedBy = Guid.NewGuid();
 
        ticket.Close(changedBy);
 
        var domainEvent = ticket.DomainEvents.OfType<TicketStatusChangedDomainEvent>()
            .Should().ContainSingle(e => e.NewStatus == TicketStatus.Closed).Subject;
 
        domainEvent.OldStatus.Should().Be(TicketStatus.Resolved);
        domainEvent.ChangedBy.Should().Be(changedBy);
        domainEvent.Reason.Should().Be("Confirmed fixed by requester.");
    }

    #endregion

    #region Reopen Ticket

    [Fact]
    public void Reopen_WhenResolvedAndStillAssigned_ShouldReturnToInProgressAndRecalculateSla()
    {
        var ticket = CreateResolvedTicket();
 
        ticket.Reopen(Guid.NewGuid());
 
        ticket.Status.Should().Be(TicketStatus.InProgress);
        ticket.ResolvedAt.Should().BeNull();
        ticket.ClosedAt.Should().BeNull();
        ticket.SlaDeadlineAt.Should().NotBeNull();
        ticket.IsSlaBreached.Should().BeFalse();
    }

    [Fact]
    public void Reopen_WhenResolvedAndUnassigned_ShouldReturnToOpen()
    {
        var ticket = CreateAssignedTicket(out _);
        ticket.Resolve(Guid.NewGuid());
        SetPrivateProperty(ticket, nameof(Ticket.AssignedToUserId), (Guid?)null);
 
        ticket.Reopen(Guid.NewGuid());
 
        ticket.Status.Should().Be(TicketStatus.Open);
    }

    #endregion

    #region Change Ticket Priority

    [Fact]
    public void ChangePriority_WithDifferentValue_ShouldUpdatePriorityAndRaiseEvent()
    {
        var ticket = CreateOpenTicket(TicketPriority.Low);
        var changedBy = Guid.NewGuid();
 
        ticket.ChangePriority(TicketPriority.Critical, changedBy);
 
        ticket.Priority.Should().Be(TicketPriority.Critical);
        var domainEvent = ticket.DomainEvents.OfType<TicketPriorityChangedDomainEvent>().Should().ContainSingle().Subject;
        domainEvent.OldPriority.Should().Be(TicketPriority.Low);
        domainEvent.NewPriority.Should().Be(TicketPriority.Critical);
        domainEvent.ChangedBy.Should().Be(changedBy);
    }

    [Fact]
    public void ChangePriority_WhenInProgress_ShouldRecalculateSlaDeadlineFromOriginalAssignedAt()
    {
        var ticket = CreateAssignedTicket(out _, TicketPriority.Low);
        var assignedAt = ticket.AssignedAt!.Value;
 
        ticket.ChangePriority(TicketPriority.Critical, Guid.NewGuid());
 
        ticket.SlaDeadlineAt.Should().Be(assignedAt.AddHours(2));
    }

    #endregion

    #region Comments

    [Fact]
    public void AddComment_WithValidData_ShouldAddCommentAndRaiseEvent()
    {
        var ticket = CreateOpenTicket();
        var author = Guid.NewGuid();
 
        ticket.AddComment("Rebooted the printer, waiting to see if it holds.", author);
 
        var comment = ticket.Comments.Should().ContainSingle().Subject;
        comment.CreatedBy.Should().Be(author);
 
        var domainEvent = ticket.DomainEvents.OfType<TicketCommentAddedDomainEvent>().Should().ContainSingle().Subject;
        domainEvent.CommentId.Should().Be(comment.Id);
        domainEvent.CreatedBy.Should().Be(author);
    }

    [Theory]
    [InlineData(true, WorkspaceRole.Member, false)]  // author, non-admin -> allowed
    [InlineData(false, WorkspaceRole.Admin, false)]  // non-author, admin  -> allowed
    [InlineData(false, WorkspaceRole.Member, true)]  // non-author, non-admin -> denied
    public void DeleteComment_ShouldBeAllowedOnlyForAuthorOrAdmin(bool isAuthor, WorkspaceRole role, bool expectThrow)
    {
        var ticket = CreateOpenTicket();
        var author = Guid.NewGuid();
        ticket.AddComment("Original comment text.", author);
        var commentId = ticket.Comments.Single().Id;
        var actingUser = isAuthor ? author : Guid.NewGuid();
 
        var act = () => ticket.DeleteComment(commentId, actingUser, role);
 
        if (expectThrow)
            act.Should().Throw<DomainException>();
        else
            act.Should().NotThrow();
    }

    [Theory]
    [InlineData(true, false)]  // author edits -> allowed
    [InlineData(false, true)] // non-author edits -> denied
    public void EditComment_ShouldOnlyBeAllowedByTheAuthor(bool isAuthor, bool expectThrow)
    {
        var ticket = CreateOpenTicket();
        var author = Guid.NewGuid();
        ticket.AddComment("Original comment text.", author);
        var commentId = ticket.Comments.Single().Id;
        var actingUser = isAuthor ? author : Guid.NewGuid();
 
        var act = () => ticket.EditComment(commentId, actingUser, "Edited comment text.");
 
        if (expectThrow)
            act.Should().Throw<DomainException>();
        else
            act.Should().NotThrow();
    }

    [Fact]
    public void RestoreComment_WhenCommentExists_ShouldSucceed()
    {
        var ticket = CreateOpenTicket();
        var author = Guid.NewGuid();
        ticket.AddComment("Original comment text.", author);
        var commentId = ticket.Comments.Single().Id;
        ticket.DeleteComment(commentId, author, WorkspaceRole.Member);
 
        var act = () => ticket.RestoreComment(commentId);
 
        act.Should().NotThrow();
    }

    #endregion

    #region Theory Test Data
 
    public static TheoryData<string, Func<Ticket>, Action<Ticket>> GuardedStateTransitions()
    {
        var data = new TheoryData<string, Func<Ticket>, Action<Ticket>>
        {
            {
                "Archive_WhenNotClosed",
                () => CreateOpenTicket(),
                t => t.Archive(Guid.NewGuid())
            },
            {
                "Assign_WhenNotOpen",
                () => CreateAssignedTicket(out _),
                t => t.Assign(Guid.NewGuid(), Guid.NewGuid())
            },
            {
                "ChangePriority_WhenClosed",
                CreateClosedTicket,
                t => t.ChangePriority(TicketPriority.Critical, Guid.NewGuid())
            },
            {
                "Close_WhenNotResolved",
                () => CreateOpenTicket(),
                t => t.Close(Guid.NewGuid())
            },
            {
                "Reopen_WhenNotResolved",
                () => CreateOpenTicket(),
                t => t.Reopen(Guid.NewGuid())
            },
            {
                "Resolve_WhenNotInProgress",
                () => CreateOpenTicket(),
                t => t.Resolve(Guid.NewGuid())
            },
            {
                "UnArchive_WhenNotArchived",
                CreateClosedTicket,
                t => t.UnArchive(Guid.NewGuid())
            },
            {
                "UnAssign_WhenNotInProgress",
                () => CreateOpenTicket(),
                t => t.UnAssign(Guid.NewGuid())
            },
            {
                "Update_WhenClosed",
                CreateClosedTicket,
                t => t.Update("New title text here", "New description text here.", Guid.NewGuid())
            }
        };

        return data;
    }

    [Theory]
    [MemberData(nameof(GuardedStateTransitions))]
    public void GuardedTransition_WhenCalledInWrongState_ShouldThrowDomainStateException(
        string scenario, Func<Ticket> arrange, Action<Ticket> act)
    {
        var ticket = arrange();

        var call = () => act(ticket);

        call.Should().Throw<DomainStateException>($"scenario '{scenario}' should be blocked");
    }
 
    public static TheoryData<string, Func<Ticket>, Action<Ticket>> NoOpStateTransitions()
    {
        var data = new TheoryData<string, Func<Ticket>, Action<Ticket>>
        {
            {
                "Archive_AlreadyArchived",
                () =>
                {
                    var t = CreateClosedTicket();
                    t.Archive(Guid.NewGuid());
                    return t;
                },
                t => t.Archive(Guid.NewGuid())
            },
            {
                "Assign_SameAssignee",
                () => CreateAssignedTicket(out _),
                t => t.Assign(t.AssignedToUserId!.Value, Guid.NewGuid())
            },
            {
                "ChangePriority_SameValue",
                () => CreateOpenTicket(TicketPriority.Medium),
                t => t.ChangePriority(TicketPriority.Medium, Guid.NewGuid())
            },
            {
                "ChangeType_SameValue",
                () => CreateOpenTicket(type: AnyTicketType()),
                t => t.ChangeType(t.Type, Guid.NewGuid())
            }
        };

        return data;
    }

    [Theory]
    [MemberData(nameof(NoOpStateTransitions))]
    public void NoOpTransition_WhenCalledWithUnchangedValue_ShouldNotRaiseAdditionalDomainEvents(
        string scenario, Func<Ticket> arrange, Action<Ticket> repeatAction)
    {
        var ticket = arrange();
        var eventCountBefore = ticket.DomainEvents.Count;

        repeatAction(ticket);

        ticket.DomainEvents.Should().HaveCount(eventCountBefore, $"scenario '{scenario}' should be a no-op");
    }
    #endregion
}

