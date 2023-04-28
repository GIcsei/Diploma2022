namespace PSI_Checker_2p0.FileHandler.FileSaver
{
    internal class FillDiskScopeSaver : BaseScopeFileHandler
    {
        public FillDiskScopeSaver(string folderName = @"NotDefined", string dirPath = @"C:\waveform")
            : base(folderName, dirPath)
        {
        }

        protected override void AddSaveFile()
        {
            if (!MaxSizeReached)
            {
                return;
            }
            index++;
            UpdateFilePath();
            filePaths.Add(filePath);
        }
    }
}
