using System.Collections.Generic;

namespace PSI_Checker_2p0.Checker
{
    public interface IListableFields<T>
    {
        IEnumerable<KeyValuePair<string, List<T>>> GetFields();
        T GetFieldValue(string fieldName, int index);

        void AddValue(string key, T value);
        void AddList(string key, List<T> list);
    }
}
