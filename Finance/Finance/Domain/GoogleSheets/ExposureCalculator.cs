using Finance.Domain.GoogleSheets.Models;

namespace Finance.Domain.GoogleSheets;

public interface IExposureCalculator
{
    public decimal CalculateLocalExposure(LedgerInputRow[] rows);
    public decimal CalculatePositionExposure(LedgerInputRow[] rows);
    public decimal CalculatePortfolioExposure(LedgerInputRow[] rows);
}

public class ExposureCalculator : IExposureCalculator
{
    public decimal CalculateLocalExposure(LedgerInputRow[] rows)
    {
        return 0;
    }
    public decimal CalculatePositionExposure(LedgerInputRow[] rows)
    {
        return 0;
    }
    public decimal CalculatePortfolioExposure(LedgerInputRow[] rows)
    {
        return 0;
    }
}