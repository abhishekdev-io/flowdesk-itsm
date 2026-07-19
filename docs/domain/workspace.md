# Workspace Aggregate

The `Workspace` aggregate represents a logical workspace (or tenant context) in FlowDesk. It governs workspace-level metadata, soft-deletion status, and membership access control.

---

## Boundary

The `Workspace` aggregate encapsulates:
*   **Root Entity**: `Workspace` (identified by a `Guid`)
*   **Owned Child Entities**: `WorkspaceMember` (plain entity mapping users to roles within the workspace)
*   **Value Objects**: `WorkspaceName`

### Cross-Aggregate References
*   **By ID Only**:
    *   `UserId` (within `WorkspaceMember`): Links the membership to the user account in the identity sub-system.
*   **Why**: Memberships are entirely dependent on the workspace's existence and lifecycle. A membership role change or addition requires verification against the workspace's existing members list to avoid duplicates. Conversely, the identity/user system has its own lifecycles, and keeping users referenced by ID prevents lock escalation.

---

## Invariants

The `Workspace` aggregate enforces the following business rules:

| Area | Invariant Statement |
| :--- | :--- |
| **Unique Membership** | A user cannot be added as a member of a workspace if they are already an active member of that workspace. |
| **Member Existence** | Removing a member or changing a member's role requires that the member is currently part of the workspace. |
| **Identifier Integrity** | Members cannot be created or added with an empty `UserId` or `WorkspaceId`. |
| **Name Length** | Workspace names cannot be empty and cannot exceed 100 characters. |

---

## Creation

*   **Factory Method**: `Workspace.Create(...)`
*   **Parameters**: `string name`, `string description`
*   **Validations**:
    *   Instantiates `WorkspaceName` value object, triggering its empty checks and length constraints (max 100 characters).
*   **Event Raised**: None (creation is tracked inside the transaction, but no domain event is dispatched).

---

## State Transitions

The following state machine governs the lifecycle of a `Workspace` instance:

| Method | From → To State | Guards | Event Raised |
| :--- | :--- | :--- | :--- |
| **Create** | `[None] → Active` (`IsDeleted = false`) | None | None |
| **Update** | `Active → Active` | None | None |
| **Delete** | `Active → Deleted` (`IsDeleted = true`) | `!IsDeleted` | None |
| **Restore** | `Deleted → Active` (`IsDeleted = false`) | `IsDeleted == true` | None |
| **AddMember** | `Active → Active` | `userId != Guid.Empty`, member not already in workspace | None |
| **RemoveMember** | `Active → Active` | Member must exist in workspace | `WorkspaceMemberRemovedDomainEvent` |
| **ChangeMemberRole**| `Active → Active` | Member must exist in workspace | `WorkspaceRoleChangedDomainEvent` |

---

## Owned Child Entities & Value Objects Used

*   **`WorkspaceMember`**: A child entity mapping a user to a specific `WorkspaceRole` (e.g. `Admin`, `Member`). Key identity is composite (`UserId` + `WorkspaceId`).
*   **`WorkspaceName`**: A value object wrapping workspace name string, ensuring it is trimmed, not empty, and <= 100 characters.
