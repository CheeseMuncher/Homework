using MetaEdit.Conventions;
using System;
using System.IO;
using System.Linq;
using Media = MediaInfo.MediaInfo;

namespace MetaEdit
{
    public class CallDataFileProcessor : IFileProcessor
    {
        private readonly Media _mediaInfo = new Media();
        public readonly IFileOperations _totalRecallFileOperations;
        public readonly IFileOperations _superBackupFileOperations;
        private readonly IFileNameDecoder<CallData> _totalRecallDecoder;
        private readonly IFileNameDecoder<CallData> _superBackupDecoder;

        public CallDataFileProcessor(Func<DecodeConventionType, IDecodeConvention> conventionSelector)
        {
            _totalRecallFileOperations = new FileOperations(conventionSelector(DecodeConventionType.TotalRecall));
            _superBackupFileOperations = new FileOperations(conventionSelector(DecodeConventionType.SuperBackup));
            _totalRecallDecoder = new TotalRecallDecoder(conventionSelector(DecodeConventionType.TotalRecall));
            _superBackupDecoder = new SuperBackupDecoder(conventionSelector(DecodeConventionType.SuperBackup));
        }

        /// <summary>
        /// Update Call Audio files from TotalRecall using CallLog published by SuperBackup
        /// </summary>
        public void ProcessData(string source, string destination, string fileData, bool trialRun)
        {
            var callFiles = _totalRecallFileOperations.GetFiles(source);
            var callLog = GetCallLogs(source, fileData);

            CallData match = null;
            foreach (var callFile in callFiles)
            {
                var callData = ExtractCallData(callFile);
                match = TryGetMatch(callLog, callData);

                if (match != null)
                {
                    callData.ContactName = callData.ContactName ?? match.ContactName;
                    callData.ContactNumber = callData.ContactNumber ?? match.ContactNumber;
                    callData.CallDuration = callData.CallDuration == new TimeSpan() ? match.CallDuration : callData.CallDuration;
                }

                var destinationFile = $"{destination}{Path.DirectorySeparatorChar}{callData}";
                if (source == destination)
                    if (trialRun)
                        Console.WriteLine($"{callFile} would get renamed to {destinationFile}");
                    else
                        File.Move(callFile, destinationFile);
                else
                    if (trialRun)
                    Console.WriteLine($"{callFile} would get copied to {destinationFile}");
                else
                    File.Copy(callFile, destinationFile);
            }
        }

        private CallData[] GetCallLogs(string source, string fileName)
        {
            return fileName is null
                ? null
                : _superBackupFileOperations
                    .GetData($"{source}{Path.DirectorySeparatorChar}{fileName}")
                    .Skip(1)
                    .Reverse()
                    .Select(cl => _superBackupDecoder.DecodeFileName(cl))
                    .ToArray();
        }

        private CallData ExtractCallData(string filePath)
        {
            if (_totalRecallFileOperations.TryExtractFileName(filePath, out string fileName))
            {
                var duration = GetDurationString(filePath);
                return _totalRecallDecoder.DecodeFileName(fileName, duration);
            }

            throw new ArgumentException($"{nameof(CallDataFileProcessor)}.{nameof(ExtractCallData)} could not extract file name from path {filePath}");
        }

        private string GetDurationString(string filePath)
        {
            _mediaInfo.Open(filePath);
            return _mediaInfo
                .Inform()
                .Split("\r\n")
                .First(line => line.StartsWith("Duration"))
                .Split(":")
                .Last();
        }

        private CallData TryGetMatch(CallData[] callLog, CallData callData)
        {
            return callLog
                ?.LastOrDefault(cl =>
                    cl.CallType != CallType.Missed
                    && cl.CallTime < callData.CallTime
                    && callData.CallTime.AddMinutes(-1) < cl.CallTime);
        }
    }
}