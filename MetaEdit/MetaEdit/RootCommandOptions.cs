using MetaEdit.Conventions;
using System.CommandLine;

namespace MetaEdit
{
    public static class RootCommandOptions
    {
        public static Option<string> Convention => new Option<string>(
            new[] { "--convention", "-c" },
            getDefaultValue: () => $"{DecodeConventionType.TotalRecall}",
            description: "The convention used to decode file names");

        public static Option<string> InputPath => new Option<string>(
            new[] { "--input-path", "-i" },
            getDefaultValue: () => "./data",
            description: "The location of the input files to process");

        public static Option<string> OutputPath => new Option<string>(
            new[] { "--output-path", "-o" },
            getDefaultValue: () => "./output",
            description: "The location to save the output files");

        public static Option<bool> DryRun => new Option<bool>(
            new[] { "--dry-run", "-d" },
            getDefaultValue: () => true,
            description: "Print expected output without creating");

        public static Option<bool> Replace => new Option<bool>(
            new[] { "--replace", "-r" },
            getDefaultValue: () => false,
            description: "Deletes input files and writes output to the input directory. Use with caution");
    }
}