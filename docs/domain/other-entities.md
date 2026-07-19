# Supporting Domain Entities

This document covers supporting domain entities and simple aggregates that contain business logic but do not meet the complexity thresholds of full aggregate roots.

---

## 1. Asset Category (`AssetCategory`)

`AssetCategory` is a workspace-level cataloging entity. It inherits from `Entity<int>` and handles prefix-based asset tagging.

### Invariants & Rules
*   **Prefix Requirement**: A prefix is mandatory (e.g. `HW`, `SW`) and is automatically normalized to uppercase and trimmed.
*   **Unique Scope**: Within a single workspace, category names must be unique (enforced via database constraints on `WorkspaceId` + `NormalizedName`).
*   **Tag Generation**: Automatically formats and increments tag identifiers sequence using the format `{Prefix}-{NextAssetNumber:D4}` (e.g. `IT-0001`).

### State Transitions
*   **`Create`**: Instantiates category under a workspace with `IsActive = true` and `NextAssetNumber = 1`. Throws `DomainException` if prefix is empty or workspace ID is `Guid.Empty`.
*   **`Update`**: Re-evaluates prefix and name value object constraints.
*   **`Deactivate`**: Shifts `IsActive` to `false` and captures `DeactivatedAt` timestamp.
*   **`Restore`**: Shifts `IsActive` to `true`, captures `RestoredAt` timestamp, and clears `DeactivatedAt`.

---

## 2. User Notification (`UserNotification`)

`UserNotification` tracks notifications dispatched to specific workspace members. It inherits from `Entity<Guid>`.

### Invariants & Rules
*   **Mandatory Fields**: Throws `DomainException` if `WorkspaceId` or `RecipientUserId` is empty, or if `Title` or `Body` is blank.
*   **Reference Links**: Can optionally carry a `RelatedTicketId` to allow deep-linking within client applications.

### State Transitions
*   **`Create`**: Initializes notification with `IsRead = false` and `CreatedAt = DateTime.UtcNow`.
*   **`MarkAsRead`**: If not already read, sets `IsRead = true` and captures `ReadAt` timestamp.

---

## 3. Refresh Token (`RefreshToken`)

`RefreshToken` manages session persistence and revocation for identity services. It inherits from `Entity<Guid>`.

### Invariants & Rules
*   **Expiration Check**: Evaluates if the token is expired by comparing `ExpiresAt` to `DateTime.UtcNow`.
*   **Revocation Check**: Once `RevokedAt` is populated, the token is considered inactive.
*   **Session Linkage**: Can track the successor token using `ReplacedByTokenId` to trace the refresh token rotation chain.

### State Transitions
*   **`Create`**: Generates a new refresh token with a token hash, user ID, and expiration date.
*   **`Revoke`**: Marks the token as revoked by setting `RevokedAt = DateTime.UtcNow`, recording the `RevokedReason`, and mapping any replacement token IDs.
 