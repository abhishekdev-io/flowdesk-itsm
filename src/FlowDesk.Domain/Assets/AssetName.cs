using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;

namespace FlowDesk.Domain.Assets;

public sealed class AssetName : ValueObject
{
    private const int MaxLength = 100;
    public string Value { get; }

    private AssetName(string value)
        => Value = value;

    public static AssetName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Asset name cannot be empty.");

        string trimmedValue = value.Trim();

        if (trimmedValue.Length > MaxLength)
            throw new DomainException("Asset name cannot exceed 100 characters.");

        return new AssetName(trimmedValue);
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
