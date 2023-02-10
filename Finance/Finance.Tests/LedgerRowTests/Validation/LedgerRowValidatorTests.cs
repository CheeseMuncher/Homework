using System.Collections.Generic;
using System.Linq;
using Finance.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Finance.Tests.LedgerRowTests.Validation;

public class LedgerRowValidatorTests : TestFixture<LedgerRowValidator>
{
    private IList<object> ValidLedgerRowRaw =>
        new List<object> { "Something 1", "GBP", "ISA",  "GSK", "Money", "Stuff" };

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
        var input = ValidLedgerRowRaw;

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
        var input = ValidLedgerRowRaw;
        input[0] = date;

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Date is missing");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Validate_ReturnsInvalidResult_IfCurrencyNullOrEmpty(string currency)
    {
        // Arrange
        var input = ValidLedgerRowRaw;
        input[1] = currency;

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Currency is missing");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Validate_ReturnsInvalidResult_IfPortfolioNullOrEmpty(string portfolio)
    {
        // Arrange
        var input = ValidLedgerRowRaw;
        input[2] = portfolio;

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Portfolio is missing");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Validate_ReturnsInvalidResult_IfCodeNullOrEmpty(string code)
    {
        // Arrange
        var input = ValidLedgerRowRaw;
        input[3] = code;

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Code is missing");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Validate_ReturnsInvalidResult_IfConsiderationNullOrEmpty(string consideration)
    {
        // Arrange
        var input = ValidLedgerRowRaw;
        input[4] = consideration;

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Consideration is missing");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Validate_HandlesUndersizedInput(int size)
    {
        // Arrange
        var input = ValidLedgerRowRaw.Take(size).ToList();        

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be(size == 0 ? "Input is empty" : $"{LedgerInputRow.HeaderRow[size]} is missing");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Validate_HandlesEmptyUnits(string units)
    {
        // Arrange
        var input = ValidLedgerRowRaw.Take(5).Concat(new [] { units }).ToList();  

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }
}