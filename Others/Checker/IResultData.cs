namespace PSI_Checker_2p0.Checker
{
    public interface IResultData<T> where T : new()
    {
        string SaveLocation { get; set; }
        string FileName { get; }
        string Description { get; }
        T Result { get; }
        bool AddData(T newData);
        bool SaveToFile();
        IResultData<T> LoadFromFile(string filePath);
    }
}