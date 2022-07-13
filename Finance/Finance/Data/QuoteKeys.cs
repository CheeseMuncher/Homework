using Finance.Utils;
using System.Collections.Generic;

namespace Finance.Data;

/// <summary>
/// Yahoo Finance API tickers
/// </summary>
public static class QuoteKeys
{
    public static readonly string[] ForexTickers = new []
    {
        "USDGBP"
    };

    public static readonly string[] DelistedStocks = new []
    {
        "AZEM.L", "BG.L","XONE"
    };

    public static readonly string[] ClosedPositions = new []
    {
        "DDD","GE","GRG.L","HSX.L","LSEG.L","NWG.L","RIO.L","SGE.L","VAL.L","WTG.L"
    };

    public static readonly string[] LiveSchwabStocks = new []
    { 
        "SSYS","DM" 
    };

    public static readonly string[] LiveMixedStocks = new []
    { 
        "AVV.L","AXS.L","BBOX.L","BOO.L","BLTG.L","DARK.L","PUR.L","TPK.L","WIX.L"
    };

    public static readonly string[] LiveIsaStocks = new []
    { 
        "AEG.L","AFX.L","BARC.L","GSK.L","HEIQ.L","HSBA.L","IPF.L","PHE.L","TLW.L","XLM.L"
    };

    public static readonly string[] LiveSippStocks = new []
    { 
        "ADM.L","FRES.L","IAG.L","SHEL.L"
    };

    public static readonly Dictionary<string, string> VanguardTickers = new Dictionary<string, string>
    {
        ["0P0000KM1Y.L"] = "Japan Stock Index",
        ["0P0000KM20.L"] = "Pacific ex-Japan",
        ["0P0000KM22.L"] = "Emerging Markets",
        ["0P0000KSP9.L"] = "Developed Europe ex-U.K.",
        ["0P0000KSPA.L"] = "U.S. Equity Index",
        ["0P0000N47O.L"] = "Global Small-Cap",
        ["0P0000SAVS.L"] = "U.K. All Share",
    };

    public static string[] HeaderConcat(this IEnumerable<string> current, IEnumerable<string> toAdd) =>        
        current.Concat(new [] { "" }).Concat(toAdd).ToArray();

    public static string[] Headers =>
        new [] { "Date" }
            .Concat(ForexTickers.OrderBy(t => t))
            .HeaderConcat(VanguardTickers.Keys.OrderBy(k => k).ToArray())
            .HeaderConcat(DelistedStocks.Concat(ClosedPositions).OrderBy(s => s))
            .HeaderConcat(LiveSchwabStocks.OrderBy(s => s))
            .HeaderConcat(LiveMixedStocks.OrderBy(s => s))
            .HeaderConcat(LiveIsaStocks.OrderBy(s => s))
            .HeaderConcat(LiveSippStocks.OrderBy(s => s))
            .Select(value => value.HandleSuffix())
            .ToArray();
}