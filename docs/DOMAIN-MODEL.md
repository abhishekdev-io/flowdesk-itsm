# FlowDesk Domain Model Documentation

Welcome to the technical domain model documentation for **FlowDesk**, a .NET 10 helpdesk and asset management application built following Domain-Driven Design (DDD) principles and Clean Architecture.

This documentation serves as a guide for technical reviewers to evaluate the design decisions, invariants, state-machine lifecycles, and event-driven patterns implemented in the Domain layer of the codebase.

## Documentation Index

### [1. DDD Primitives & Architecture Overview](domain/00-overview.md)
Provides a foundation on the base DDD abstractions (`Entity<TId>`, `AggregateRoot<TId>`, `ValueObject`, and `IDomainEvent`) used across the application. It details coding conventions, encapsulation practices, factory pattern designs, and features a high-level Bounded Context map showing relationships and ID-only references between aggregates.

### [2. Ticket Aggregate Model](domain/ticket.md)
Documents the core, highly complex `Ticket` aggregate. This section details its boundaries, owned child entities (`TicketComment`, `TicketActivity`), value objects, custom business rules, SLA deadline computations, and a state-transition matrix governing the ticket lifecycle from creation to resolution and archiving.

### [3. Workspace Aggregate Model](domain/workspace.md)
Covers the `Workspace` aggregate model, which provides multi-tenant logical boundaries within the system. It defines the workspace lifecycle, member onboarding processes, and access roles (`WorkspaceRole`), alongside domain invariants preventing duplicate memberships or invalid role assignments.

### [4. Asset Aggregate Model](domain/asset.md)
Explains the lifecycle and state transitions of the `Asset` aggregate. This includes the management of physical/digital assets, validation constraints on tags and serial numbers, and the formal state machine handling maintenance schedules, active statuses, and decommissioning routes.

### [5. Supporting Domain Entities](domain/other-entities.md)
Summarizes the simpler, support-level entities and aggregates that assist with core features: `AssetCategory` (workspace-level categorization), `UserNotification` (recipient notifications), and `RefreshToken` (identity authentication persistence).

### [6. Domain Services](domain/domain-services.md)
Details the stateless business logic encapsulated within domain services. In particular, it focuses on the `SlaService` and its responsibility for calculating real-time SLA risk levels, breach statuses, compliance percentages, and sorting queues.

### [7. Events Catalog & Database Schema Mapping](domain/events-and-schema.md)
A comprehensive index of all Domain Events alongside their triggers and Application-layer handlers. It also details the Entity Framework Core persistence mapping, including non-obvious configurations such as PostgreSQL-specific concurrency tokens (`xmin`), identity sequences, and database view mappings (`WorkspaceUserView`).
