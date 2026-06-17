using FlowDesk.Domain.Assets;
using FlowDesk.Domain.Tickets.Aggregates;
using FlowDesk.Domain.Tickets.Enums;
using FlowDesk.Domain.Workspaces.Aggregates;
using FlowDesk.Infrastructure.Identity.Entity;
using FlowDesk.Infrastructure.Persistence.Data;

namespace FlowDesk.Infrastructure.Persistence.Seeders;

public static class TicketSystemSeeder
{
    public static async Task SeedTicketAsync(
        ApplicationDbContext dbContext, 
        Workspace workspace, 
        Dictionary<string, Asset> assets,
        AppUser admin, 
        AppUser agent1, 
        AppUser agent2,
        AppUser member1,
        AppUser member2,
        AppUser member3)
    {
        var dellLaptop = assets["DellLaptop"];
        var macbook = assets["Macbook"];
        var ciscoSwitch = assets["Switch"];
        var dellServer = assets["DellServer"];
        var hpServer = assets["HpServer"];
        var cannonPrinter = assets["CannonPrinter"];

        //tickets

        //Healthy SLA (In Progress)
        var ticket1 = Ticket.Create(
            "Software Deployment Failure", 
            "Docker desktop throwing hypervisor engine errors on startup.", 
            workspace.Id, TicketType.Incident, TicketPriority.Medium, member1.Id, dellLaptop.Id);
        
        ticket1.Assign(agent1.Id, admin.Id);

        //comments
        ticket1.AddComment("Hey, could you check if your WSL2 kernel component is updated to the latest version?", agent1.Id);
        ticket1.AddComment("Just checked, running WSL version 2.3.24. Should I downgrade?", member1.Id);
        
        await dbContext.Tickets.AddAsync(ticket1);
        await dbContext.SaveChangesAsync(); 

        //Healthy SLA (In Progress)
        var ticket2 = Ticket.Create(
            "IDE Environment Out of Memory", 
            "Local web containers throwing out of memory exceptions on runtime builds.", 
            workspace.Id, TicketType.Incident, TicketPriority.Medium, member2.Id, dellLaptop.Id);
        
        ticket2.Assign(agent2.Id, admin.Id); 

        //comments
        ticket2.AddComment("I bumped your allocated Docker runtime memory limits up to 8GB. Let me know if the build script passes now.", agent2.Id);

        await dbContext.Tickets.AddAsync(ticket2);
        await dbContext.SaveChangesAsync();

        //Near SLA Breach (Warning State - 15 Mins Left)
        var ticket3 = Ticket.Create(
            "Executive Device Issues", 
            "CEO's MacBook trackpad is completely unresponsive.", 
            workspace.Id, TicketType.Incident, TicketPriority.Critical, member2.Id, macbook.Id);
        
        ticket3.Assign(agent1.Id, admin.Id);

        //comments
        ticket3.AddComment("Running an external hardware diagnostic loop via the centralized device portal.", agent1.Id);
        ticket3.AddComment("Please expedite, the executive team meeting starts in 20 minutes.", member2.Id);

        await dbContext.Tickets.AddAsync(ticket3);
        await dbContext.SaveChangesAsync(); 

        // Backdate to simulate 1 hour and 45 minutes passing on a 2-hour SLA window
        var entry3 = dbContext.Entry(ticket3);
        entry3.Property(t => t.AssignedAt).CurrentValue = DateTime.UtcNow.AddMinutes(-105);
        entry3.Property(t => t.SlaDeadlineAt).CurrentValue = DateTime.UtcNow.AddMinutes(15); 
        await dbContext.SaveChangesAsync();

        //Actively Breached SLA (Overdue Crisis)
        var ticket4 = Ticket.Create(
            "Core Switch Downtime", 
            "Packet loss critical on core production interface VLAN-10.", 
            workspace.Id, TicketType.Incident, TicketPriority.Critical, member3.Id, ciscoSwitch.Id);

        ticket4.Assign(agent1.Id, admin.Id);

        //comments
        ticket4.AddComment("Primary fiber transceiver module is unresponsive. Running automated failover configurations.", agent1.Id);
        ticket4.AddComment("Failover loops are also timing out. The entire staging cluster is offline.", member3.Id);

        await dbContext.Tickets.AddAsync(ticket4);
        await dbContext.SaveChangesAsync();

        // Force breach rules by moving assigned window 5 hours into history
        var entry4 = dbContext.Entry(ticket4);
        entry4.Property(t => t.AssignedAt).CurrentValue = DateTime.UtcNow.AddHours(-5);
        entry4.Property(t => t.SlaDeadlineAt).CurrentValue = DateTime.UtcNow.AddHours(-3);
        entry4.Property(t => t.IsSlaBreached).CurrentValue = true; 
        await dbContext.SaveChangesAsync();

        //Met SLA / Cleanly Resolved (Historical)
        var ticket5 = Ticket.Create(
            "VPN Account Provisioning", 
            "Requesting corporate network connection profile configuration access.", 
            workspace.Id, TicketType.ServiceRequest, TicketPriority.Low, member3.Id, hpServer.Id);

        ticket5.Assign(agent2.Id, admin.Id);

        //comments
        ticket5.AddComment("Profile credentials configured and linked to your corporate authenticator token.", agent2.Id);
        ticket5.AddComment("Confirmed! Logged in successfully. Thank you for the quick turnaround.", member3.Id);
        
        await dbContext.Tickets.AddAsync(ticket5);
        await dbContext.SaveChangesAsync();

        ticket5.Resolve(admin.Id); 
        await dbContext.SaveChangesAsync();
        
        //Unassigned Queue / Open Intake
        var ticket6 = Ticket.Create(
            "Network Switch Port Dropouts", 
            "Ports 12 through 16 are dropping packets intermittently during peak operation hours.", 
            workspace.Id, TicketType.Incident, TicketPriority.High, member1.Id, ciscoSwitch.Id);

        //comments
        ticket6.AddComment("Attached network diagnostic dump file. Notice the CRC error spike at 11:00 AM.", member1.Id);    
        
        await dbContext.Tickets.AddAsync(ticket6);
        await dbContext.SaveChangesAsync();

        //Cold Archived Record History
        var ticket7 = Ticket.Create(
            "Replacement Printer Toner Cartridge", 
            "Department printer is running low on black ink. Requesting standard high-yield replacement.", 
            workspace.Id, TicketType.ServiceRequest, TicketPriority.Low, member1.Id, cannonPrinter.Id);

        ticket7.Assign(agent2.Id, admin.Id);

        //comments
        ticket7.AddComment("Toner replacement cartridge dispatched via internal courier delivery.", agent2.Id);
        ticket7.AddComment("Received and physically installed. Print tests passing perfectly.", member1.Id);

        ticket7.Resolve(admin.Id);
        ticket7.Close(member1.Id,"Successfully resolved the issue");
        ticket7.Archive(admin.Id); 
        
        await dbContext.Tickets.AddAsync(ticket7);
        await dbContext.SaveChangesAsync();   

    }    
}
