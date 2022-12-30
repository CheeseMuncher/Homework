using Finance.Domain.GoogleSheets.Models;
using Google.Apis.Sheets.v4.Data;

namespace Finance.Domain.GoogleSheets;

public interface ILedgerManager
{
    public LedgerRow[] GetLedger();
    public LedgerInputRow[] LoadInputFromSource(ValueRange valueRange);
    ValueRange BuildLedgerWriteData();
}

public class LedgerManager : ILedgerManager
{
    private readonly LedgerValidator _ledgerValidator = new();
    private LedgerRow[] _ledgerData;
    public LedgerRow[] GetLedger() => _ledgerData;

    public LedgerInputRow[] LoadInputFromSource(ValueRange valueRange)
    {
        var data = valueRange.Values.Skip(1).ToList();
        var candidate = new LedgerCandidate { HeaderRow = valueRange.Values.First(), DataRows = data };
        var validationResult = _ledgerValidator.Validate(candidate);
        if (!validationResult.IsValid)
            throw new InvalidDataException($"Errors: {string.Join(',', validationResult.Errors.Select(e => e.ErrorMessage))}");

        var output = new List<LedgerRow>();
        foreach(var row in data)
        {
            output.Add(new LedgerRow(
                DateOnly.Parse(row[0].ToString()),
                row[1].ToString(),
                row[2].ToString(),
                row[3].ToString(),
                decimal.Parse(row[4].ToString()),
                row.Count > 5 ? int.Parse(row[5].ToString()) : 0);
        }

        _ledgerData = output
            .OrderBy(row => row.Date)
            .ThenBy(row => row.Currency)
            .ThenBy(row => row.Code)
            .ThenBy(row => row.Portfolio)
            .ToArray();

        return _ledgerData.Cast<LedgerInputRow>().ToArray();
    }

    public ValueRange BuildLedgerWriteData()
    {
        var result = new ValueRange();
        result.Values = new List<IList<object>>();           
        result.Values.Add(new List<object> (LedgerRow.FullHeaderRow));
        foreach(var row in _ledgerData)
            result.Values.Add(row.ToSpreadsheetRow);

        return result;
    }    
}