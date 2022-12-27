using System.Collections.Generic;
using System.IO;
using Finance.Domain.GoogleSheets;
using Finance.Domain.GoogleSheets.Models;
using Finance.Tests.Validation;
using FluentAssertions;
using Google.Apis.Sheets.v4.Data;
using Xunit;

namespace Finance.Tests;

public class LedgerManagerTests : TestFixture<LedgerManager>
{
    [Fact]
    public void LoadInputFromSource_InvokesValidation()
    {
        // Arrange
        var input = GetHeaderRowValueRange();
        input.Values[0][0] = "Cheese";
        var data1 = ValidLedgerDataRow;
        data1[0] = "Cheese";
        input.Values.Add(data1);
        var data2 = ValidLedgerDataRow;
        data2[4] = "Text";
        input.Values.Add(data2);

        // Act
        var act = () => Sut.LoadInputFromSource(input);

        // Assert
        act.Should().Throw<InvalidDataException>()
            .WithMessage("Errors: Header row invalid,DataRows[0] error: Date value 'Cheese' is invalid,DataRows[1] error: Consideration value 'Text' is invalid");
    }
    
    [Fact]
    public void LoadInputFromSource_ReturnsLedgerData()
    {
        // Arrange
        var input = GetHeaderRowValueRange();
        input.Values.Add(ValidLedgerDataRow);
        input.Values.Add(ValidLedgerDataRow);
        input.Values[2][1] = "Something";

        // Act
        var result = Sut.LoadInputFromSource(input);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        foreach(var r in result)
        {
            r.Date.ToString("yyyy-MM-dd").Should().Be(ValidLedgerDataRow[0].ToString());
            r.Portfolio.Should().Be(ValidLedgerDataRow[2].ToString());
            r.Code.Should().Be(ValidLedgerDataRow[3].ToString());
            r.Consideration.ToString().Should().Be(ValidLedgerDataRow[4].ToString());
            r.Units.ToString().Should().Be(ValidLedgerDataRow[5].ToString());
        }
        result[0].Currency.Should().Be(ValidLedgerDataRow[1].ToString());
        result[1].Currency.Should().Be("Something");
    }
    
    [Fact]
    public void LoadInputFromSource_GetData_RoundTrip()
    {
        // Arrange
        var input = GetHeaderRowValueRange();
        input.Values.Add(ValidLedgerDataRow);
        input.Values.Add(ValidLedgerDataRow);
        input.Values[2][1] = "Something";

        // Act
        Sut.LoadInputFromSource(input);
        var result = Sut.GetLedger();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(2);
        foreach(var r in result)
        {
            r.Date.ToString("yyyy-MM-dd").Should().Be(ValidLedgerDataRow[0].ToString());
            r.Portfolio.Should().Be(ValidLedgerDataRow[2].ToString());
            r.Code.Should().Be(ValidLedgerDataRow[3].ToString());
            r.Consideration.ToString().Should().Be(ValidLedgerDataRow[4].ToString());
            r.Units.ToString().Should().Be(ValidLedgerDataRow[5].ToString());
        }
        result[0].Currency.Should().Be(ValidLedgerDataRow[1].ToString());
        result[1].Currency.Should().Be("Something");
    }

    [Fact]
    public void LoadInputFromSource_SortsData()
    {
        // Arrange
        var input = GetHeaderRowValueRange();
        var row1 = new List<object> { "2022-12-31", "GBP", "ISA", "GSK", "100.00", "1" };
        var row2 = new List<object> { "2022-12-30", "GBP", "ISA", "GSK", "200.00", "2" };
        var row3 = new List<object> { "2022-12-31", "USD", "ISA", "GSK", "300.00", "3" };
        var row4 = new List<object> { "2022-12-30", "USD", "ISA", "GSK", "400.00", "4" };
        var row5 = new List<object> { "2022-12-30", "GBP", "ISA", "TLW", "500.00", "5" };
        var row6 = new List<object> { "2022-12-30", "GBP", "SIPP", "GSK", "600.00", "6" };
        var row7 = new List<object> { "2022-12-29", "GBP", "SIPP", "GSK", "700.00", "7" };
        input.Values.Add(row1);
        input.Values.Add(row2);
        input.Values.Add(row3);
        input.Values.Add(row4);
        input.Values.Add(row5);
        input.Values.Add(row6);
        input.Values.Add(row7);

        // Act
        Sut.LoadInputFromSource(input);
        var result = Sut.GetLedger();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().HaveCount(7);
        result[0].Consideration.Should().Be(700m);
        result[1].Consideration.Should().Be(200m);
        result[2].Consideration.Should().Be(600m);
        result[3].Consideration.Should().Be(500m);
        result[4].Consideration.Should().Be(400m);
        result[5].Consideration.Should().Be(100m);
        result[6].Consideration.Should().Be(300m);
    }    

    private IList<object> ValidLedgerDataRow => LedgerValidatorTest.ValidLedgerDataRow;

    private ValueRange GetHeaderRowValueRange()
    {
        var range = new ValueRange();
        range.Values = new List<IList<object>>();           
        range.Values.Add(new List<object> (LedgerInputRow.HeaderRow));
        return range;
    } 
}