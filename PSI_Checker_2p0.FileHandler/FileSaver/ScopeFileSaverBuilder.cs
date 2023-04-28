using PSI_Checker_2p0.Enums;

namespace PSI_Checker_2p0.FileHandler.FileSaver
{
    public static class ScopeFileSaverBuilder
    {
        public static BaseScopeFileHandler CreateFileSaver(FileSavePolicy policy,
            string folderName = @"NotDefined", string dirPath = @"C:\waveform")
        {
            switch (policy)
            {
                case FileSavePolicy.FillDisk:
                    return new FillDiskScopeSaver(folderName, dirPath);
                case FileSavePolicy.Circular:
                    return new CircularScopeSaver(folderName, dirPath);
                case FileSavePolicy.OneFile:
                    return new OneFileScopeSaver(folderName, dirPath);
                default:
                    return new FillDiskScopeSaver(folderName, dirPath);
            }
        }
    }
}
