using FlowDesk.Domain.AssetCategories;
using FlowDesk.Domain.Exceptions;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.AssetCategories;

public class CategoryNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldReturnCategoryNameWithTrimmedValue()
    {
        // Act
        var result = CategoryName.Create("  My Category  ");

        // Assert
        result.Value.Should().Be("My Category");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowException(string? invalidValue)
    {
        // Act
        Action act = () => CategoryName.Create(invalidValue!);
        
        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Category name cannot be empty.");
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldThrowException()
    {
        // Arrange
        var tooLongName = new string('A', CategoryName.MaxLength + 1);

        // Act
        Action act = () => CategoryName.Create(tooLongName);
        
        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Category name cannot exceed {CategoryName.MaxLength} characters.");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Arrange & Act
        var name1 = CategoryName.Create("Laptops");
        var name2 = CategoryName.Create("Laptops");

        // Assert
        name1.Should().Be(name2);
    }
}
