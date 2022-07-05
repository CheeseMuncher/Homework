namespace MetaEdit
{
    public interface IFileProcessor
    {
        /// <summary>
        /// Does all the work
        /// </summary>
        void ProcessData(string source, string destination, string fileData, bool trialRun);
    }
}