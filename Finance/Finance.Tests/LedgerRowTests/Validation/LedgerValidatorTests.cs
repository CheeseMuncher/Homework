using System.Collections.Generic;
using System.Linq;
using Finance.Domain.Models;
using FluentAssertions;
using Xunit;

namespace Finance.Tests.LedgerRowTests.Validation;

public class LedgerValidatorTest : TestFixture<LedgerValidator>
{
    [Fact]    
    public void Validate_ReturnsInvalidResult_IfInputNull()
    {
        // Arrange
        var input = new LedgerCandidate
        {
            HeaderRow = null,
            DataRows = null
        };

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Ledger is empty");
    }

   [Fact]    
    public void Validate_ReturnsInvalidResult_IfInputEmpty()
    {
        // Arrange
        var input = new LedgerCandidate
        {
            HeaderRow = new List<object>(),
            DataRows = new List<IList<object>>()
        };

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be($"Ledger is empty");
    }

    [Fact]
    public void Validate_ReturnsValidResult_WhenValidLedger()
    {
        // Arrange
        var input = new LedgerCandidate
        {
            HeaderRow = ValidHeaderRow,
            DataRows = new List<IList<object>>() 
            {
                ValidLedgerDataRow,
                ValidLedgerDataRow
            }
        };        

        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ReturnsInvalidResult_IfHeaderRowInvalid()
    {
        // Arrange
        var input = new LedgerCandidate
        {
            HeaderRow = SomeStrings(3),
            DataRows = new List<IList<object>>() 
            {
                ValidLedgerDataRow,
                ValidLedgerDataRow
            }
        };
        
        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().ContainSingle()
            .Which.ErrorMessage.Should().Be("Header row invalid");
    }

    [Fact]
    public void Validate_ReturnsInvalidResult_FullErrors()
    {
        // Arrange
        var input = new LedgerCandidate
        {
            HeaderRow = ValidHeaderRow,
            DataRows = new List<IList<object>>() 
            {
                SomeStrings(7),
                SomeStrings(6),
                SomeStrings(7)
            }
        };
        
        // Act
        var result = Sut.Validate(input);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Where(e => e.ErrorMessage.Contains("DataRows[0]")).Should().HaveCount(3);
        result.Errors.Where(e => e.ErrorMessage.Contains("DataRows[1]")).Should().HaveCount(3);
        result.Errors.Where(e => e.ErrorMessage.Contains("DataRows[2]")).Should().HaveCount(3);
    }

    private IList<object> SomeStrings(int i) => 
        CreateMany<string>(i).Select(s => (object)s).ToList();

    private IList<object> ValidHeaderRow => 
        LedgerInputRow.HeaderRow.Cast<object>().ToList();

    public static IList<object> ValidLedgerDataRow =>
        new List<object> { "2022-12-22", "GBP", "ISA", "GSK", "1234.56", "11" };
}