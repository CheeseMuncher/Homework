using System.Collections.Generic;
using System.Linq;
using Finance.Domain.GoogleSheets.Models;
using FluentAssertions;
using Xunit;

namespace Finance.Tests.Validation;

public class LedgerDataRowValidatorTests : TestFixture<LedgerDataRowValidator>
{
    public static IList<object> ValidLedgerDataRow =>
        LedgerValidatorTest.ValidLedgerDataRow;

    [Fact]    
    public void Validate_ReturnsInvalidResult_IfInputEmpty()
    {
        // Arrange
        // Act
        var result = Sut.Validate(new List<object>());

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Input is empty");
    }

    [Fact]    
    public void Validate_ReturnsValidResult_WhenValidRow()
    {
        // Arrange
        var input = ValidLedgerDataRow;

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Validate_ReturnsInvalidResult_IfDateNullOrEmpty(string date)
    {
        // Arrange
        var input = ValidLedgerDataRow;
        input[0] = date;

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Date is missing");
    }

    [Fact]
    public void Validate_ReturnsInvalidResult_IfDateInvalid()
    {
        // Arrange
        var input = ValidLedgerDataRow;
        input[0] = "Cheese";

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Date value '{input[0]}' is invalid");
    }

    [Fact]
    public void Validate_HandlesEmptyConsiderationData()
    {
        // Arrange
        var input = ValidLedgerDataRow.Take(4).ToList();

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Consideration is missing");
    }

    [Fact]
    public void Validate_ReturnsInvalidResult_IfConsiderationInvalid()
    {
        // Arrange
        var input = ValidLedgerDataRow;
        input[4] = "Cheese";

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Consideration value '{input[4]}' is invalid");
    }

    [Fact]
    public void Validate_ReturnsInvalidResult_IfUnitsInvalid()
    {
        // Arrange
        var input = ValidLedgerDataRow;
        input[5] = "1.5";

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Units value '{input[5]}' is invalid");
    }

    [Fact]
    public void Validate_HandlesEmptyUnitsData()
    {
        // Arrange
        var input = ValidLedgerDataRow.Take(5).ToList();

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }
}