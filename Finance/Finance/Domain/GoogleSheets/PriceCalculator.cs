using Finance.Domain.Models;

namespace Finance.Domain.GoogleSheets;

public interface IPriceCalculator
{
    decimal CalculatePrice(LedgerInputRow row);
}

public class PriceCalculator : IPriceCalculator
{
    public decimal CalculatePrice(LedgerInputRow row)
    {
        if (row.Units == 0)
            return 0;

        if (Math.Abs(row.Consideration) < 2000m)
            return 0;

        return -100 * row.Consideration / row.Units;
    }
}