using FlowDesk.Domain.Notifications;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Notifications;

public sealed class UserNotificationConfig : IEntityTypeConfiguration<UserNotification>
{
    public void Configure(EntityTypeBuilder<UserNotification> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Id)
               .ValueGeneratedNever();

        builder.Property(n => n.WorkspaceId)
               .IsRequired();

        builder.Property(n => n.RecipientUserId)
               .IsRequired();

        builder.Property(n => n.Title)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(n => n.Body)
               .HasMaxLength(1000)
               .IsRequired();

        builder.Property(n => n.EventType)
               .HasConversion<string>()
               .HasMaxLength(50)
               .IsRequired();

        builder.Property(n => n.RelatedTicketId);

        builder.Property(n => n.IsRead)
               .IsRequired()
               .HasDefaultValue(false);

        builder.Property(n => n.ReadAt);

        builder.Property(n => n.CreatedAt)
               .IsRequired();

        builder.HasIndex(n => new { n.RecipientUserId, n.IsRead });

        builder.HasIndex(n => new { n.WorkspaceId, n.RecipientUserId, n.CreatedAt });

        builder.HasIndex(n => n.RelatedTicketId);
    }
}