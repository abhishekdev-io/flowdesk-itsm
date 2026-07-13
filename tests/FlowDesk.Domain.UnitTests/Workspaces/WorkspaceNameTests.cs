using FlowDesk.Domain.Exceptions;
using FlowDesk.Domain.Workspaces.ValueObjects;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.Workspaces;

public class WorkspaceNameTests
{
    [Fact]
    public void Create_WithValidName_ShouldReturnWorkspaceNameWithTrimmedValue()
    {
        // Act
        var result = WorkspaceName.Create("   Engineering Workspace   ");

        // Assert
        result.Value.Should().Be("Engineering Workspace");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowDomainException(string? invalidValue)
    {
        // Act
        Action act = () => WorkspaceName.Create(invalidValue!);
        
        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Workspace name cannot be empty.");
    }

    [Fact]
    public void Create_WithValueExactlyAtMaxLength_ShouldSucceed()
    {
        // Arrange
        var validMaxLengthName = new string('A', WorkspaceName.MaxLength);

        // Act
        var result = WorkspaceName.Create(validMaxLengthName);
        
        // Assert
        result.Value.Should().HaveLength(WorkspaceName.MaxLength);
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldThrowDomainException()
    {
        // Arrange
        var tooLongName = new string('A', WorkspaceName.MaxLength + 1);

        // Act
        Action act = () => WorkspaceName.Create(tooLongName);
        
        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage($"Workspace name cannot exceed {WorkspaceName.MaxLength} characters.");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Act
        var name1 = WorkspaceName.Create("Marketing Team");
        var name2 = WorkspaceName.Create("Marketing Team");

        // Assert
        name1.Should().Be(name2);
    }
}
