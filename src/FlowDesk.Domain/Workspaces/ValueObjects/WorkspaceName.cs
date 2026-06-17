using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;

namespace FlowDesk.Domain.Workspaces.ValueObjects;

public sealed class WorkspaceName : ValueObject
{
    public const int MaxLength = 100;

    private WorkspaceName(string value)
    {
        this.Value = value;
    }

    public static WorkspaceName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Workspace name cannot be empty.");

        string trimmedValue = value.Trim();    

        if (trimmedValue.Length > MaxLength)
            throw new DomainException($"Workspace name cannot exceed {MaxLength} characters.");

        return new WorkspaceName(trimmedValue);
    }
    public string Value { get; }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
