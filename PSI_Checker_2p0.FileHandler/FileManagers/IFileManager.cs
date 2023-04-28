using System.Threading.Tasks;

namespace PSI_Checker_2p0.FileHandler.FileManagers
{
    public interface IFileManager
    {
        Task WriteTextToFileAsync(string text, string path, bool append = false);

        string NormalizePath(string path);

        string ResolvePath(string path);
    }
}
