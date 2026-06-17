using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Infrastructure.Identity.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Tickets;

public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.HasKey(tc => tc.Id);

        builder.Property(t => t.Id)
               .ValueGeneratedNever();

        builder.Property(tc => tc.Description)
                .IsRequired()
                .HasMaxLength(900);

        builder.HasIndex(tc => tc.TicketId);

        builder.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(tc => tc.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);       

        builder.Property(Tc => Tc.IsDeleted)
               .HasDefaultValue(false)
               .IsRequired();

        builder.HasOne(tc => tc.Ticket)
               .WithMany(t => t.Comments)
               .HasForeignKey(tc => tc.TicketId)
               .OnDelete(DeleteBehavior.Cascade);       

        builder.HasQueryFilter(tc => !tc.IsDeleted);       
    }
}
