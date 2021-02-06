using MetaEdit.Conventions;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace MetaEdit
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                RootCommandOptions.Convention,
                RootCommandOptions.InputPath,
                RootCommandOptions.OutputPath,
                RootCommandOptions.DryRun,
                RootCommandOptions.Replace
            };
            rootCommand.Description = "Metadata and FileName editor";

            rootCommand.Handler = CommandHandler.Create<string, string, string, bool, bool>((convention, inputPath, outputPath, dryRun, replace) =>
            {
                Main(convention, inputPath, replace ? inputPath : outputPath, dryRun, replace);
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        private static void Main(string convention, string inputPath, string outputPath, bool dryRun, bool replace)
        {
            Console.WriteLine($"Meta Edit running, checking inputs...");

            if (!Enum.TryParse(convention, out DecodeConventionType conventionType))
            {
                Console.WriteLine($"Invalid convention supplied. Valid conventions are: {string.Join(",", ValidConventions)}");
                return;
            }

            if (!ValidatePath(inputPath))
                return;

            if (!replace && !ValidatePath(outputPath))
                return;

            if (!ValidatePossibleIoException(inputPath, outputPath, replace))
                return;

            Console.WriteLine($"Looking for files in {inputPath} using the {conventionType} convention.");
            if (dryRun)
                Console.WriteLine($"Dry run selected, no files will be changed, expected outcome will be displayed in the console");
            else if (replace)
                Console.WriteLine($"Output directory will be ignored and input files will be overwritten");
            else
                Console.WriteLine($"Output files will be written to {outputPath}");
        }

        private static bool ValidatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Supplied file path {path} does not exist.");
                return false;
            }
            return true;
        }

        private static bool ValidatePossibleIoException(string input, string output, bool replace)
        {
            if (!replace && input == output)
            {
                Console.WriteLine($"Input and output directories match but the replace flag is false. Unexpected behaviour could result");
                return false;
            }
            return true;
        }

        private static string[] ValidConventions => Enum
            .GetValues(typeof(DecodeConventionType))
            .Cast<DecodeConventionType>()
            .Where(t => t != DecodeConventionType.None)
            .Select(t => $"{t}")
            .ToArray();
    }
}