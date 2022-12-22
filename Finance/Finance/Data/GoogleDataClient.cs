using Finance.Utils;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace Finance.Data;

public interface IGoogleDataClient
{
    void Connect(string fileName, string[] scopes);
}

public class GoogleDataClient : IGoogleDataClient
{
    private SheetsService? _sheetsService;
    private IFileIO _fileIO;
    
    public GoogleDataClient(IFileIO fileIO)
    {
        _fileIO = fileIO;        
    }

    public void Connect(string fileName, string[] scopes)
    {        
        var credential = _fileIO.BuildCredentialFromFile(fileName, scopes);
        _sheetsService = new SheetsService(new BaseClientService.Initializer() { 
            HttpClientInitializer = credential, 
            ApplicationName = "Finance" 
        });
    }
}
