using System.Collections.Generic;

namespace PSI_Checker_2p0.FileHandler.FileSaver
{
    internal class OneFileScopeSaver : BaseScopeFileHandler
    {
        public OneFileScopeSaver(string folderName = @"NotDefined", string dirPath = @"C:\waveform")
            : base(folderName, dirPath)
        {
        }

        private List<string> usedFiles;
        private string currentFolder = null;

        protected override void AddSaveFile()
        {
            if (currentFolder != FolderName)
            {
                currentFolder = FolderName;
                usedFiles = new List<string>();
            }
            if (usedFiles.Contains(filePath.FullName))
                return;
            usedFiles.Add(filePath.FullName);
            filePaths.Add(filePath);
        }
    }
}
