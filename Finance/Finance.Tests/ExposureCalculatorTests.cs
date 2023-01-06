using System;
using System.Linq;
using AutoFixture;
using Finance.Domain.GoogleSheets;
using Finance.Domain.GoogleSheets.Models;
using FluentAssertions;
using Xunit;

namespace Finance.Tests;

public class ExposureCalculatorTests :TestFixture<ExposureCalculator>
{
    [Theory]
    [InlineData(0)]
    public void CalculateLocalExposure_ReturnsZero_IfCashIn(int testDataRowCount)
    {
        // Arrange        
        var input = GetTestData().Take(testDataRowCount + 1).ToArray();
        
        // Act
        Sut.CalculateLocalExposure(input).Should().Be(GetTestData()[testDataRowCount].LocalExposure);
        Sut.CalculatePositionExposure(input).Should().Be(GetTestData()[testDataRowCount].PositionExposure);
        // Sut.CalculatePortfolioExposure(input).Should().Be(GetTestData()[testDataRowCount].PortfolioExposure);
    }

    private LedgerRow[] GetTestData()
    {
        return new []
        {
            new LedgerRow(DateOnly.Parse("2012-11-01"), "GBP", "MF", "Cash in", 20055, 0) { LocalExposure = 0, PositionExposure = 0, PortfolioExposure = 20055},
        };
    }
}
