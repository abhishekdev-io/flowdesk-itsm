using FlowDesk.Domain.Assets;
using FlowDesk.Domain.Exceptions;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.Assets;

public class AssetNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldReturnAssetNameWithTrimmedValue()
    {
        //Act
        var result = AssetName.Create("  My Asset  ");

        //Assert
        result.Value.Should().Be("My Asset");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowException(string? invalidValue)
    {
        //Act
        Action act = () => AssetName.Create(invalidValue!);
        
        //Assert
        act.Should().Throw<DomainException>().WithMessage("Asset name cannot be empty.");
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldThrowException()
    {
        //Act
        var tooLongName = new string('A', 101);

        Action act = () => AssetName.Create(tooLongName);
        
        //Assert
        act.Should().Throw<DomainException>().WithMessage("Asset name cannot exceed 100 characters.");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        //Act
        var name1 = AssetName.Create("Laptop");
        var name2 = AssetName.Create("Laptop");

        //Assert
        name1.Should().Be(name2);
    }
}
