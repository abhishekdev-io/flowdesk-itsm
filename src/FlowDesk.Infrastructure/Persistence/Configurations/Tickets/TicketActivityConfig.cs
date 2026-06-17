using FlowDesk.Domain.Tickets.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Tickets;

public class TicketActivityConfig : IEntityTypeConfiguration<TicketActivity>
{
       public void Configure(EntityTypeBuilder<TicketActivity> builder)
       {
              builder.HasKey(ta => ta.Id);

              builder.Property(t => t.Id)
               .ValueGeneratedNever();

              builder.Property(ta => ta.Description)
                     .IsRequired()
                     .HasMaxLength(900);

              builder.Property(ta => ta.Action)
                     .HasConversion<string>();

              builder.Property(ta => ta.OccurredAt)
                     .IsRequired();

              builder.Property(ta => ta.PerformedBy)
                     .IsRequired();                

              builder.HasIndex(ta => ta.TicketId);

              builder.HasIndex(ta => ta.OccurredAt);

              builder.HasOne(ta => ta.Ticket)
               .WithMany(t => t.Activities)
               .HasForeignKey(ta => ta.TicketId)
               .OnDelete(DeleteBehavior.Cascade);

       }
}
