using MetaEdit.Conventions;
using System.CommandLine;
using System.IO;

namespace MetaEdit
{
    public static class RootCommandOptions
    {
        public static Option<string> Convention => new Option<string>(
            new[] { "--convention", "-c" },
            getDefaultValue: () => $"{DecodeConventionType.TotalRecall}",
            description: "The convention used to decode file names");

        public static Option<string> Source => new Option<string>(
            new[] { "--source", "-s" },
            getDefaultValue: () => $".{Path.DirectorySeparatorChar}data",
            description: "The source location of the input files to process");

        public static Option<string> Destination => new Option<string>(
            new[] { "--destination", "-d" },
            getDefaultValue: () => $".{Path.DirectorySeparatorChar}output",
            description: "The destination location to save the output files");

        public static Option<string> FileData => new Option<string>(
            new[] { "--file-data", "-f" },
            getDefaultValue: () => "callLog.csv",
            description: "The name of a file where supplementary data will be obtained from");

        public static Option<bool> TrialRun => new Option<bool>(
            new[] { "--trial-run", "-t" },
            getDefaultValue: () => true,
            description: "Print expected output without creating");

        public static Option<bool> Replace => new Option<bool>(
            new[] { "--replace", "-r" },
            getDefaultValue: () => false,
            description: "Deletes input files and writes output to the input directory. Use with caution");
    }
}