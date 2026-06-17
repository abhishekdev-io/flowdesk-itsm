using FlowDesk.Domain.Primitives;
using FlowDesk.Domain.Exceptions;

namespace FlowDesk.Domain.AssetCategories;

public sealed class CategoryName : ValueObject
{
    public const int MaxLength = 100;
    private CategoryName(string value)
    {
        Value = value;
    }

    public static CategoryName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        throw new DomainException("Category name cannot be empty.");

            var trimedValue = value.Trim();

        if (trimedValue.Length > MaxLength)
            throw new DomainException($"Category name cannot exceed {MaxLength} characters.");

        return new CategoryName(trimedValue);
    }

    public string Value { get; private set; }
    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
