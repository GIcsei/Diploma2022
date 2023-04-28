using System.IO;

namespace PSI_Checker_2p0.FileHandler.FileLoader
{
    public class TdmsFileLoader : BaseFileLoader
    {
        public TdmsFileLoader(FileInfo file) : base()
        {
            filePath = file.FullName;
            name = file.Name;
            Extension = file.Extension;
        }

        public TdmsFileLoader() : base()
        {
            Extension = ".tdms";
        }
    }
}
