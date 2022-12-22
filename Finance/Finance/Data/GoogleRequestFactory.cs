using Google.Apis.Sheets.v4;
using GetRequest = Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.GetRequest;

namespace Finance.Data;

public interface IGoogleRequestFactory
{
    GetRequest GetSheetData(SheetsService service, string sheetid, string range);
}

public class GoogleRequestFactory : IGoogleRequestFactory
{
    public GetRequest GetSheetData(SheetsService service, string sheetid, string range) 
        => service.Spreadsheets.Values.Get(sheetid, range);
}
