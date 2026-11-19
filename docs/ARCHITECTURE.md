# FlowDesk System Architecture

This document describes the architectural topology, patterns, cross-cutting pipelines, and design decisions governing **FlowDesk**, a full-stack, multi-tenant helpdesk and IT asset management platform built on .NET 10, React 19, Clean Architecture, Domain-Driven Design (DDD), and CQRS.

---

## 1. System Topology

FlowDesk operates as a containerized, decoupled system composed of a React Single-Page Application (SPA), an Nginx reverse proxy, an ASP.NET Core Web API with SignalR real-time hubs, and a PostgreSQL relational database.

```mermaid
graph TD
    subgraph Client["Client Tier (Browser)"]
        SPA["React 19 SPA (Vite + TS)<br/>TanStack Query • Zustand • Tailwind CSS"]
    end

    subgraph Gateway["Edge / Reverse Proxy Tier"]
        Nginx["Nginx Reverse Proxy<br/>Gzip • Static Asset Cache • WebSocket Upgrade"]
    end

    subgraph Backend["Application & API Tier (.NET 10)"]
        API["FlowDesk.Api (Host & Controllers)"]
        Hub["NotificationHub (SignalR WebSockets)"]
        
        subgraph Core["Clean Architecture Core"]
            MediatR["5-Stage MediatR Pipeline"]
            Application["FlowDesk.Application (CQRS Commands & Queries)"]
            Domain["FlowDesk.Domain (Aggregates, Invariants, Events)"]
        end

        subgraph Infra["Infrastructure Layer"]
            EFCore["FlowDesk.Infrastructure (EF Core 10, Identity, Interceptors)"]
        end
    end

    subgraph Persistence["Persistence Tier"]
        Postgres[("PostgreSQL 17 Alpine<br/>Relational Tables • Views • Sequences")]
    end

    %% Network flows
    SPA -->|HTTP / REST Requests| Nginx
    SPA -->|WebSocket Connections| Nginx
    Nginx -->|Proxy /api/ & Static Fallback| API
    Nginx -->|Upgrade /api/v1/hubs/| Hub
    
    API --> MediatR
    Hub --> Application
    MediatR --> Application
    Application --> Domain
    Application --> Infra
    Infra --> Domain
    Infra --> Postgres

    %% Styling
    classDef client fill:#1976d2,stroke:#bbdefb,stroke-width:2px,color:#ffffff;
    classDef gateway fill:#00796b,stroke:#80cbc4,stroke-width:2px,color:#ffffff;
    classDef host fill:#4a148c,stroke:#ce93d8,stroke-width:2px,color:#ffffff;
    classDef app fill:#0d47a1,stroke:#90caf9,stroke-width:2px,color:#ffffff;
    classDef domain fill:#1b5e20,stroke:#a5d6a7,stroke-width:2px,color:#ffffff;
    classDef infra fill:#e65100,stroke:#ffcc80,stroke-width:2px,color:#ffffff;
    classDef db fill:#263238,stroke:#b0bec5,stroke-width:2px,color:#ffffff;

    class SPA client;
    class Nginx gateway;
    class API,Hub host;
    class MediatR,Application app;
    class Domain domain;
    class EFCore infra;
    class Postgres db;
```

---

## 2. Layer Separation & Responsibilities

The system follows strict **Clean Architecture** boundaries where dependencies flow exclusively inward. High-level policies never depend on low-level implementation details.

```mermaid
graph TD
    Api["FlowDesk.Api<br/>(Presentation Layer)"]
    Infrastructure["FlowDesk.Infrastructure<br/>(Infrastructure Layer)"]
    Application["FlowDesk.Application<br/>(Application Layer)"]
    Domain["FlowDesk.Domain<br/>(Domain Layer)"]

    Api --> Application
    Api --> Infrastructure
    Infrastructure --> Application
    Infrastructure --> Domain
    Application --> Domain

    classDef domain fill:#1b5e20,stroke:#81c784,stroke-width:2px,color:#ffffff;
    classDef application fill:#0d47a1,stroke:#64b5f6,stroke-width:2px,color:#ffffff;
    classDef infra fill:#e65100,stroke:#ffb74d,stroke-width:2px,color:#ffffff;
    classDef api fill:#4a148c,stroke:#ba68c8,stroke-width:2px,color:#ffffff;

    class Domain domain;
    class Application application;
    class Infrastructure infra;
    class Api api;
```

| Layer | Component | Core Responsibilities | Key Technologies |
| :--- | :--- | :--- | :--- |
| **Frontend** | `FlowDesk.Client` | Renders user interfaces, manages client routing, maintains cache state, and coordinates real-time socket events. | React 19, TypeScript, Vite, TanStack Query, Zustand, Tailwind CSS v4, Lucide |
| **Presentation** | `FlowDesk.Api` | Hosts HTTP endpoints, maps API versioning, configures SignalR hubs, terminates CORS, captures global exceptions, and publishes health checks. | ASP.NET Core 10, SignalR, Serilog, Swashbuckle / OpenAPI |
| **Application** | `FlowDesk.Application` | Orchestrates business use cases via CQRS Commands and Queries, executes pipeline validation, and evaluates multi-tenant authorization rules. | MediatR, FluentValidation |
| **Domain** | `FlowDesk.Domain` | Encapsulates enterprise logic, aggregate roots, domain invariants, value objects, state machine transitions, and domain events. Zero external dependencies. | Pure C# 13, MediatR (Marker Interfaces) |
| **Infrastructure** | `FlowDesk.Infrastructure` | Manages database persistence, EF Core mappings, PostgreSQL views, JWT token generation, cookie configurations, and event interceptors. | EF Core 10, Npgsql, ASP.NET Core Identity |

---

## 3. CQRS Implementation

FlowDesk segregates write operations (mutations) from read operations (queries) to optimize domain integrity and query performance independently.

```mermaid
flowchart LR
    subgraph ClientRequest["Incoming Request"]
        CmdReq["Command (Mutation)"]
        QryReq["Query (Read)"]
    end

    subgraph WritePipeline["Write Path (Commands)"]
        direction TB
        CmdHandler["Command Handler"]
        Repo["Domain Repository"]
        Aggregate["Aggregate Root (Invariants & Events)"]
        UoW["Unit of Work (SaveChangesAsync)"]
        Interceptor["PublishDomainEventsInterceptor"]
        
        CmdHandler --> Repo
        Repo --> Aggregate
        Aggregate --> UoW
        UoW --> Interceptor
    end

    subgraph ReadPipeline["Read Path (Queries)"]
        direction TB
        QryHandler["Query Handler"]
        DirectDb["IApplicationDbContext (.AsNoTracking())"]
        View["Database View (vw_WorkspaceUsers)"]
        DTO["Flat Response DTO"]

        QryHandler --> DirectDb
        DirectDb --> View
        View --> DTO
    end

    CmdReq --> WritePipeline
    QryReq --> ReadPipeline
```

### Write Path (Commands)
* **Aggregate Invariant Protection**: Commands fetch domain aggregates through domain repositories (`ITicketRepository`, `IWorkspaceRepository`). State mutations invoke expressive domain methods (e.g. `ticket.Assign(assigneeId, currentUserId)`), which enforce business rules and queue domain events.
* **Transactional Persistence**: Mutations are committed via `IApplicationDbContext.SaveChangesAsync()`.
* **Post-Commit Event Interception**: A custom EF Core interceptor collects raised domain events and publishes them through MediatR *only after* the transaction commits successfully.

### Read Path (Queries)
* **Repository Bypass**: Query handlers bypass domain repositories completely and query `IApplicationDbContext` directly using `.AsNoTracking()`.
* **Projection & Views**: Handlers project queries directly into flat response DTOs, leveraging the keyless database view `vw_WorkspaceUsers` to retrieve user information without joining sensitive identity tables.
* **Tenant Scoping**: All queries enforce strict tenant boundary filtering (`Where(x => x.WorkspaceId == request.WorkspaceId)`).

---

## 4. MediatR Request Pipeline Chain

Cross-cutting concerns are organized sequentially using open behaviors within the MediatR pipeline. Every incoming command or query passes through this 5-stage interceptor chain before executing its handler:

```mermaid
graph TD
    Request["Incoming MediatR Request"] --> B1["1. LoggingBehaviour<br/>• Measures duration with TimeProvider<br/>• Structured Serilog logging"]
    B1 --> B2["2. ValidationBehaviour<br/>• Executes FluentValidation rules<br/>• Throws ValidationException on error"]
    B2 --> B3["3. SystemAdminBehaviour<br/>• Validates ISystemAdminRequest<br/>• Verifies system admin claims"]
    B3 --> B4["4. AuthorizationBehaviour<br/>• Validates IWorkspaceRequest<br/>• Verifies tenant membership<br/>• Hydrates CurrentWorkspaceContext"]
    B4 --> B5["5. PermissionBehaviour<br/>• Evaluates RequirePermissionAttribute<br/>• Checks role capabilities"]
    B5 --> Handler["Target Command / Query Handler"]
```

### Ordering Rationale
1. **`LoggingBehaviour`** wraps the outermost boundary to ensure execution time is recorded and any downstream pipeline exception is captured and logged.
2. **`ValidationBehaviour`** rejects malformed payloads immediately, avoiding unnecessary database calls or security lookups.
3. **`SystemAdminBehaviour`** blocks non-superadmins from accessing global administrative commands.
4. **`AuthorizationBehaviour`** inspects the incoming request for `IWorkspaceRequest`, queries membership, and populates the scoped `CurrentWorkspaceContext` with the user's role.
5. **`PermissionBehaviour`** executes last before the handler, utilizing the already-hydrated `CurrentWorkspaceContext` to evaluate granular permissions without duplicate database round-trips.

---

## 5. Real-Time Notification Pipeline (SignalR)

FlowDesk incorporates real-time notifications for ticket assignments, priority changes, status updates, and workspace membership changes.

```mermaid
sequenceDiagram
    autonumber
    actor Agent as User A (Actor)
    participant API as Web API
    participant Interceptor as EF Core Interceptor
    participant MediatR as MediatR Dispatcher
    participant Handler as Notification Event Handler
    participant Svc as NotificationService
    participant Hub as SignalR NotificationHub
    actor Recipient as User B (Recipient Browser)

    Agent->>API: POST /api/v1/workspaces/{id}/tickets/{id}/assign
    API->>API: Ticket.Assign(...) -> raises TicketAssignDomainEvent
    API->>Interceptor: SaveChangesAsync()
    Note over Interceptor: Transaction committed to PostgreSQL
    Interceptor->>MediatR: Publish(TicketAssignDomainEvent)
    MediatR->>Handler: Handle(domainEvent)
    Handler->>Svc: CreateAsync(workspaceId, recipientId, ...)
    Svc->>Svc: Persist UserNotification entity to DB
    Svc->>Hub: RealtimePublisher.SendToUserAsync(...)
    Hub->>Recipient: WebSocket push: "NotificationReceived" { NotificationDto }
    Note over Recipient: React Client updates unread badge & displays toast
```

### Architectural Details:
1. **Decoupled Event Flow**: The command handler does not directly send notifications or know about SignalR. It simply modifies the aggregate.
2. **Post-Commit Guarantee**: `PublishDomainEventsInterceptor` dispatches domain events only after the database write succeeds. If the transaction rolls back, no notifications are pushed.
3. **Multi-Tenant Hub Security**:
   - Connection handshake intercepts the access token via query parameter `?access_token=` in `JwtBearerEvents.OnMessageReceived`.
   - `NotificationHub.OnConnectedAsync` queries `IMembershipChecker` to ensure the user is an authorized member of the requested workspace.
   - Connections are joined to an isolated group key: `workspace:{workspaceId}:user:{userId}`.
4. **Client-Side Integration**: The React client subscribes via `useNotificationHub`, dynamically invalidates TanStack Query caches, and renders real-time toast notifications via Sonner.

---

## 6. Authentication & Session Security Architecture

FlowDesk implements an enterprise-grade split-token session architecture:

```mermaid
flowchart TD
    subgraph AuthStorage["Client Token Segregation"]
        Mem["Application Memory (Zustand Store)<br/>Short-Lived Access Token (15 min)"]
        Cookie["HttpOnly / Secure / SameSite Cookie<br/>Long-Lived Refresh Token (7 days)<br/>Path: /api/v1/auth"]
    end

    subgraph AuthProtection["Security Safeguards"]
        XSS["XSS Immunity<br/>JavaScript cannot read document.cookie"]
        CSRF["Strict Path Scoping & SameSite Mode<br/>Cookie only sent to /api/v1/auth"]
        Hash["SHA-256 Hash Persistence<br/>Plaintext tokens never stored in DB"]
        Breach["Automatic Breach Detection<br/>Replay of revoked token revokes entire token family"]
    end

    Mem --> XSS
    Cookie --> CSRF
    Cookie --> Hash
    Cookie --> Breach
```

### Silent Refresh with Concurrency Queuing
In the frontend client (`apiClient.ts`), an Axios interceptor prevents token expiration race conditions when multiple concurrent requests trigger 401 Unauthorized responses:
* **In-Flight Lock**: When the first 401 error is received, a boolean lock `isRefreshing` is set to `true`.
* **Request Queuing**: Concurrent incoming 401 requests are pushed to a pending queue instead of triggering separate refresh calls.
* **Token Rotation**: The client calls `/api/v1/auth/refresh-token`. The server rotates the refresh token in the cookie, persists the new hash, and returns a new access token.
* **Queue Drain**: The client updates the in-memory access token, drains the pending queue with the new token, and replays all original requests seamlessly.

---

## 7. Containerization & Edge Architecture

In production, FlowDesk runs within a multi-container Docker environment orchestrated by Docker Compose:

```mermaid
graph LR
    subgraph Host["Docker Host (Bridge Network: flowdesk-net)"]
        NginxService["web (Nginx:alpine)<br/>Port 80:80"]
        ApiService["api (ASP.NET Core 10)<br/>Port 8080 (Internal)"]
        DbService["db (Postgres 17-alpine)<br/>Port 5432 (Internal)"]
        Volume[("Volume: postgres_data")]
    end

    Internet(("Public Traffic")) -->|Port 80| NginxService
    NginxService -->|/api/* & WebSockets| ApiService
    ApiService -->|Connection String| DbService
    DbService --- Volume
```

### Nginx Reverse Proxy Capabilities
* **Single Origin**: Frontend and API share the same origin, eliminating cross-origin preflight requests in production.
* **SPA Routing Fallback**: Non-file paths fallback to `/index.html` (`try_files $uri $uri/ /index.html;`) to support React Router client-side routes.
* **WebSocket Protocol Upgrades**: Dynamic `Upgrade` and `Connection` header proxying with unbuffered streams (`proxy_buffering off; proxy_read_timeout 3600s;`) ensures persistent SignalR connections.
* **Performance Optimizations**: Native Gzip compression and long-term immutable caching headers (`Cache-Control: public, immutable`) for static bundles (`.js`, `.css`, fonts, images).

---

## 8. Architectural Decisions & Trade-Offs

### ADR 1: HttpOnly Cookie Refresh Tokens vs. LocalStorage
* **Context**: Single-page applications frequently store both access and refresh tokens in `localStorage`, exposing long-lived tokens to cross-site scripting (XSS) extraction.
* **Decision**: Deliver refresh tokens strictly inside an `HttpOnly`, `Secure` cookie scoped to `Path = /api/v1/auth`, while keeping access tokens in JavaScript memory.
* **Gains**: Immune to XSS token theft; token cannot be read or exfiltrated by injected browser scripts.
* **Trade-offs**: Requires cookie handling configuration across CORS boundaries during local development.

### ADR 2: Post-Commit Domain Event Interception
* **Context**: Dispatching MediatR notifications inside command handlers or during `SaveChangesAsync` can trigger side effects (notifications, audit logs) even if the primary database transaction fails or is aborted.
* **Decision**: Implement `PublishDomainEventsInterceptor` to extract domain events from tracked aggregates before saving, but dispatch them only after `SavedChangesAsync` succeeds.
* **Gains**: Prevents orphaned side effects and guarantees consistency between database mutations and dispatched domain events.
* **Trade-offs**: Handlers receiving domain events cannot participate in the original database transaction without implementing a persistent Outbox pattern.

### ADR 3: Keyless Database View for Read-Model User Projections (`vw_WorkspaceUsers`)
* **Context**: Query handlers often need to project member names, emails, and roles without loading full ASP.NET Core Identity models or executing complex joins.
* **Decision**: Map an EF Core keyless entity to a PostgreSQL view (`vw_WorkspaceUsers`).
* **Gains**: High performance, zero risk of leaking security-sensitive fields (like password hashes), and clean CQRS read-model separation.
* **Trade-offs**: Database view definitions must be maintained through custom EF Core migration scripts.

### ADR 4: SignalR Tenant-Isolated Group Routing
* **Context**: Real-time notifications must be delivered only to the intended recipient within the active workspace. Broadcasting to broad channels wastes client bandwidth and leaks event metadata.
* **Decision**: SignalR connections join isolated groups formatted as `workspace:{workspaceId}:user:{userId}` after validating tenant membership.
* **Gains**: Absolute data isolation across tenants and minimal network overhead.
* **Trade-offs**: Multi-tab users generate multiple connections within the same user group.

### ADR 5: PostgreSQL Native Concurrency (`xmin`) & Identity Sequences
* **Context**: Support tickets require human-readable sequential identifiers (e.g. `TKT-00045`), and concurrent entity modifications require optimistic concurrency checks.
* **Decision**: Utilize PostgreSQL's system column `xmin` for optimistic concurrency tokens, and `UseIdentityAlwaysColumn()` for transactional ticket number generation.
* **Gains**: Thread-safe sequencing without locking application tables or writing custom counter tables.
* **Trade-offs**: Binds concurrency and sequencing logic specifically to PostgreSQL.
