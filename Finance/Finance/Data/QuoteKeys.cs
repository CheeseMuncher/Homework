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
        "GBPUSD=X"
    };

    public static readonly string[] DelistedStocks = new []
    {
        "AZEM.L", "BG.L","XONE"
    };

    public static readonly string[] ClosedPositions = new []
    {
        "NWG.L","GRG.L","VAL.L","WTG.L","GE","DDD","RIO.L","SGE.L","BBOX.L" 
    };

    public static readonly string[] LiveMixedStocks = new []
    { 
        "BLTG.L","AXS.L","TPK.L","WIX.L","BOO.L","DARK.L","PUR.L"
    };

    public static readonly string[] LiveIsaStocks = new []
    { 
        "GSK.L","BARC.L","HSBA.L","IPF.L","TLW.L","AFX.L","XLM.L","PHE.L","HEIQ.L","AEG.L"
    };

    public static readonly string[] LiveSippStocks = new []
    { 
        "RDSB.L","FRES.L","IAG.L","HSX.L","LSEG.L"
    };

    public static readonly string[] LiveSchwabStocks = new []
    { 
        "SSYS","DM" 
    };

    public static readonly Dictionary<string, string> VanguardTickers = new Dictionary<string, string>
    {
        ["0P0000KM22.L"] = "Emerging Markets",
        ["0P0000KSP9.L"] = "Developed Europe ex-U.K.",
        ["0P0000SAVS.L"] = "U.K. All Share",
        ["0P0000N47O.L"] = "Global Small-Cap",
        ["0P0000KM1Y.L"] = "Japan Stock Index",
        ["0P0000KSPA.L"] = "U.S. Equity Index",
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