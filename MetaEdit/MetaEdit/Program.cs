using MetaEdit.Conventions;
using Microsoft.Extensions.DependencyInjection;
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
                RootCommandOptions.Source,
                RootCommandOptions.Destination,
                RootCommandOptions.FileData,
                RootCommandOptions.TrialRun,
                RootCommandOptions.Replace
            };
            rootCommand.Description = "Metadata and FileName editor";

            rootCommand.Handler = CommandHandler.Create<string, string, string, string, bool, bool>((convention, source, destination, fileData, trialRun, replace) =>
            {
                Main(convention, source, replace ? source : destination, fileData, trialRun, replace);
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        private static void Main(string convention, string source, string destination, string fileData, bool trialRun, bool replace)
        {
            Console.WriteLine($"Meta Edit running, checking inputs...");

            if (!Enum.TryParse(convention, out DecodeConventionType conventionType))
            {
                Console.WriteLine($"Invalid convention supplied. Valid conventions are: {string.Join(",", ValidConventions)}");
                return;
            }

            if (!ValidatePath(source))
                return;

            if (!replace && !ValidatePath(destination))
                return;

            bool useCallLogs = false;
            if (conventionType == DecodeConventionType.TotalRecall)
                useCallLogs = ValidateFileData(source, fileData);

            if (!ValidatePossibleIoException(source, destination, replace))
                return;

            Console.WriteLine($"Looking for files in {source} using the {conventionType} convention");
            if (trialRun)
                Console.WriteLine($"Trial run selected, no files will be changed, expected outcome will be displayed in the console");
            else if (replace)
                Console.WriteLine($"Output directory will be ignored and input files will be overwritten");
            else
                Console.WriteLine($"Output files will be written to {destination}");

            var serviceProvider = ConfigureServiceProvider(conventionType);
            var processor = serviceProvider.GetService<IFileProcessor>();
            try
            {
                processor.ProcessData(source, destination, useCallLogs ? fileData : null, trialRun);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static bool ValidatePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Supplied file path {path} does not exist, call logs will not be used.");
                return false;
            }
            return true;
        }

        private static bool ValidateFileData(string path, string fileName)
        {
            if (!File.Exists($"{path}{Path.DirectorySeparatorChar}{fileName}"))
            {
                Console.WriteLine($"Supplied file {fileName} not found in directory {path}");
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

        private static ServiceProvider ConfigureServiceProvider(DecodeConventionType conventionType)
        {
            Func<DecodeConventionType, IDecodeConvention> conventionSelector = (DecodeConventionType convention) => convention switch
            {
                DecodeConventionType.TotalRecall => new TotalRecallConvention(),
                DecodeConventionType.AllCallRecorder => new AllCallRecorderConvention(),
                DecodeConventionType.SuperBackup => new SuperBackupConvention(),
                DecodeConventionType.None => throw new ArgumentException("No Convention Defined")
            };

            var services = new ServiceCollection();

            services.AddSingleton<Func<DecodeConventionType>>(() => conventionType);
            services.AddSingleton<IFileProcessor, CallDataFileProcessor>();
            services.AddSingleton(conventionSelector(conventionType));

            return services.BuildServiceProvider();
        }
    }
}