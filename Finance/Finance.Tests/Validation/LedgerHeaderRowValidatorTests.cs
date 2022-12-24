using System.Collections.Generic;
using System.Linq;
using Finance.Domain.GoogleSheets.Models;
using FluentAssertions;
using Xunit;

namespace Finance.Tests.Validation;

public class LedgerHeaderRowValidatorTests: TestFixture<LedgerHeaderRowValidator>
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Validate_ReturnsInvalidResult_IfShort(int size)
    {
        // Arrange
        var input = ValidLedgerDataRow.Take(size).ToList();
        
        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Header row invalid");
    }

    [Fact]    
    public void Validate_ReturnsInvalidResult_IfInvalid()
    {
        // Arrange
        var input = CreateMany<string>(6).Cast<object>().ToList();
        
        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Header row invalid");
    }

    [Theory]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void Validate_ReturnsValidResult_IfExtraHeaders(int size)
    {
        // Arrange
        var input = Enumerable
            .Range(0, size)
            .Select(i => i < LedgerInputRow.HeaderRow.Length ? LedgerInputRow.HeaderRow[i] : Create<string>())
            .Cast<object>()
            .ToList();
        
        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ReturnsInvalidResult_IfDateNullOrEmpty()
    {
        // Arrange
        var input = ValidLedgerDataRow;
        input[0] = " ";

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Header row invalid");
    }

    private IList<object> ValidLedgerDataRow => LedgerValidatorTest.ValidLedgerDataRow;
}