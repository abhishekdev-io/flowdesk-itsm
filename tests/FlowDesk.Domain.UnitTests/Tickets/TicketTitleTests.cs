using FlowDesk.Domain.Exceptions;
using FlowDesk.Domain.Tickets.ValueObjects;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.Tickets;

public class TicketTitleTests
{
    [Fact]
    public void Create_WithValidTitle_ShouldReturnTicketTitleWithTrimmedValue()
    {
        // Act
        var result = TicketTitle.Create("   Server Crash Alert   ");

        // Assert
        result.Value.Should().Be("Server Crash Alert");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowDomainException(string? invalidValue)
    {
        // Act
        Action act = () => TicketTitle.Create(invalidValue!);
        
        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Ticket title cannot be empty.");
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldThrowDomainException()
    {
        // Arrange
        var tooLongTitle = new string('A', 101);

        // Act
        Action act = () => TicketTitle.Create(tooLongTitle);
        
        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Ticket title cannot exceed 100 characters.");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Act
        var title1 = TicketTitle.Create("Database Migration");
        var title2 = TicketTitle.Create("Database Migration");

        // Assert
        title1.Should().Be(title2);
    }
}
