using Finance.Utils;
using System.Collections.Generic;

namespace Finance.Data;

public static class Constants
{
    public static readonly HashSet<string> DelistedStocks = new HashSet<string>
    {
        "AZEM.L", 
    };

    public static readonly HashSet<string> ClosedPositions = new HashSet<string>
    {
        "NWG.L","GRG.L","VAL.L","WTG.L","GE","DDD","RIO.L","SGE.L", 
    };

    public static readonly HashSet<string> LiveMixedStocks = new HashSet<string>
    { 
        "BLTG.L","AXS.L","TPK.L","WIX.L","BOO.L","DARK.L","PUR.L"
    };

    public static readonly HashSet<string> LiveIsaStocks = new HashSet<string>
    { 
        "GSK.L","BARC.L","HSBA.L","IPF.L","TLW.L","BBOX.L","AFX.L","XLM.L","PHE.L","HEIQ.L","AEG.L"
    };

    public static readonly HashSet<string> LiveSippStocks = new HashSet<string>
    { 
        "RDSB.L","FRES.L","IAG.L","HSX.L","LSEG.L"
    };

    public static readonly HashSet<string> LiveSchwabStocks = new HashSet<string>
    { 
        "SSYS","XONE" 
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

    public static string[] HeaderConcat(this string[] current, string[] toAdd) =>        
        current.Concat(new [] { "" }).Concat(toAdd).ToArray();

    public static string[] Headers =>
        new [] { "Date", "USDGBP" }
            .HeaderConcat(VanguardTickers.Keys.OrderBy(k => k).ToArray())
            .HeaderConcat(DelistedStocks.Concat(ClosedPositions).OrderBy(s => s).ToArray())            
            .HeaderConcat(LiveSchwabStocks.OrderBy(s => s).ToArray())
            .HeaderConcat(LiveMixedStocks.OrderBy(s => s).ToArray())
            .HeaderConcat(LiveIsaStocks.OrderBy(s => s).ToArray())
            .HeaderConcat(LiveSippStocks.OrderBy(s => s).ToArray())
            .Select(value => value.HandleIndex())
            .ToArray();
}