using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlowDesk.Domain.Exceptions;
using FlowDesk.Domain.Tickets.ValueObjects;
using FluentAssertions;

namespace FlowDesk.Domain.UnitTests.Tickets;

public class TicketDescriptionTests
{
    [Fact]
    public void Create_WithValidDescription_ShouldReturnTicketDescriptionWithTrimmedValue()
    {
        // Act
        var result = TicketDescription.Create("   The application pool crashes immediately after startup.   ");

        // Assert
        result.Value.Should().Be("The application pool crashes immediately after startup.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyValue_ShouldThrowDomainException(string? invalidValue)
    {
        // Act
        Action act = () => TicketDescription.Create(invalidValue!);
        
        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Ticket description cannot be empty.");
    }

    [Fact]
    public void Create_WithValueExceedingMaxLength_ShouldThrowDomainException()
    {
        // Arrange
        var tooLongDesc = new string('A', 501);

        // Act
        Action act = () => TicketDescription.Create(tooLongDesc);
        
        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Ticket description cannot exceed 500 characters.");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Act
        var desc1 = TicketDescription.Create("API returns a 500 Internal Server Error.");
        var desc2 = TicketDescription.Create("API returns a 500 Internal Server Error.");

        // Assert
        desc1.Should().Be(desc2);
    }
}
