using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Workspaces.Aggregates;

namespace FlowDesk.Infrastructure.Persistence.Configurations.Tickets;

public class TicketConfig : IEntityTypeConfiguration<Ticket>
{
       public void Configure(EntityTypeBuilder<Ticket> builder)
       {
              builder.HasKey(t => t.Id);

              builder.Property(t => t.Id)
               .ValueGeneratedNever();

              builder.Property(t => t.SequenceNumber)
                     .UseIdentityAlwaysColumn();

              builder.HasIndex(t => t.SequenceNumber)
                     .IsUnique();       

              builder.Property(a => a.WorkspaceId).IsRequired();  

              builder.HasOne(t => t.Asset)
                     .WithMany()
                     .HasForeignKey(t => t.AssetId)
                     .OnDelete(DeleteBehavior.Restrict);            

              builder.OwnsOne(x => x.Title, title =>
              {
              title.Property(x => x.Value)
                     .HasColumnName("Title")
                     .HasMaxLength(100)
                     .IsRequired();
              });

              builder.OwnsOne(x => x.Description, description =>
              {  
              description.Property(x => x.Value)
                            .HasColumnName("Description")
                            .HasMaxLength(500)
                            .IsRequired();
              });

              builder.Property(t => t.Status)
                     .HasConversion<string>();

              builder.Property(t => t.Priority)
                     .HasConversion<string>();

              builder.Property(t => t.Type)
                     .HasConversion<string>();

              builder.HasIndex(t => t.WorkspaceId);
              builder.HasIndex(t => t.Status);
              builder.HasIndex(t => t.Priority);
              builder.HasIndex(t => new { t.WorkspaceId, t.Status });
       }
}
