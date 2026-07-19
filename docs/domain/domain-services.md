# Domain Services

This document details the domain services within FlowDesk that encapsulate logic that does not naturally belong to a single entity or aggregate.

---

## Service: SLA Service (`SlaService`)

The `SlaService` coordinates Service Level Agreement (SLA) status assessments across tickets, relying on a mockable `TimeProvider` for timezone and time mockability in tests.

### Why is this a Domain Service?
SLA evaluation requires access to the system clock (`TimeProvider`), which would compromise aggregate purity and unit testability if injected directly into the `Ticket` entity. Additionally, scheduling, queue ordering, and calculating bulk compliance percentages across groups of tickets are concerns that span collections, rather than a single `Ticket` aggregate.

---

## Service Features

### 1. Risk Level Evaluation (`SlaRiskLevel`)
The service determines a ticket's SLA risk state based on the percentage of time remaining out of the total duration allowed:

$$\text{Percentage Remaining} = \left( \frac{\text{Remaining Time}}{\text{Total SLA Allowed Time}} \right) \times 100$$

| Risk Level (`SlaRiskLevel`) | Trigger Condition |
| :--- | :--- |
| **`NoSla`** | The ticket does not have an active SLA deadline (`SlaDeadlineAt` is null). |
| **`Breached`** | The SLA deadline has passed (`SlaDeadlineAt` < Current Time) or `IsSlaBreached` is true. |
| ****Critical** (Red)** | Less than or equal to 20% of the total SLA duration remains. |
| ****AtRisk** (Yellow)** | Between 20% and 50% of the total SLA duration remains. |
| ****OnTrack** (Green)** | Greater than 50% of the total SLA duration remains. |

### 2. Remaining Time Display Formatter
Provides formatted text displays for time remaining:
*   `>= 1 day`: `"Xd Xh"` (e.g., `2d 4h`)
*   `< 1 day but >= 1 hour`: `"Xh Xm"` (e.g., `5h 12m`)
*   `< 1 hour`: `"Xm"` (e.g., `45m`)
*   `No SLA`: `"No SLA"`

### 3. SLA-Priority Sorting
The service orders collections of tickets by scheduling urgency using the method:
```csharp
public IEnumerable<Ticket> OrderBySlaPriority(IEnumerable<Ticket> tickets)
```
*   **Priority 1**: Risk Level descending (Breached/Critical first).
*   **Priority 2**: Remaining time ascending (least remaining time first).

### 4. SLA Bulk Summary (`SlaSummary`)
Compiles statistics across collections of tickets, including a compliance percentage calculation:
$$\text{Compliance} = \left( \frac{\text{Resolved Tickets Not Breached}}{\text{Total Resolved Tickets}} \right) \times 100$$
*(Defaults to 100% if no tickets have been resolved yet).*
