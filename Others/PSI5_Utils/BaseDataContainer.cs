using PSI_Checker_2p0.Checker;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PSI_Checker_2p0.PSI5_Utils
{
    /// <summary>
    /// Abstract class for analyzation data. The filled structure then can be stored both in TDMS and JSON format.
    /// Contains a <see cref="DataContainer"/> class which can be manipulated through this Container. The Container acts as a Dictionary<string, List<<see cref="DataElement"/>>>
    /// with less functionality than a Dictionary<>
    /// </summary>
    public class BaseDataContainer : IListableFields<DataElement>
    {
        private int counter = 0; // Overall number of data --> makes sure that none of the Lists is longer than Counter+1
        public int Counter
        {
            get => counter;
            protected set => counter = value;
        }

        private List<DataContainer> data = new List<DataContainer>();
        public List<DataContainer> Data
        {
            get => data;
            private set => data = value;
        }
        public String Name { get; set; }
        public List<string> FieldNames
        {
            get => Data.AsParallel().Select(x => x.GroupName).ToList();
        }

        public void Add(DataContainer data)
        {
            if (ContainsKey(data.GroupName))
                return;
            Data.Add(data);
        }

        #region Constructors
        public BaseDataContainer(string Name)
        {
            this.Name = Name;
        }

        public BaseDataContainer()
        {

        }
        #endregion

        protected bool isReadOnly = false;
        public bool IsReadOnly => this.isReadOnly;

        public virtual DataContainer this[string key]
        {
            get
            {
                try
                {
                    return Data.Where(x => x.GroupName == key).First();
                }
                catch
                {
                    var data = new DataContainer(key);

                    Data.Add(data);
                    return data;
                }
            }
        }
        public virtual void CalculateRemainingFields()
        {
            isReadOnly = true;
        }
        public void AddValue(string containerName, string key, double value)
        {
            if (isReadOnly) return;
            this[containerName][key].Add(value);
            return;
        }
        public void AddValue(string containerName, DataElement value)
        {
            if (isReadOnly) return;
            this[containerName].AddElement(value);
            return;
        }

        private bool ContainsKey(string key) => Data.Any(x => x.GroupName == key);

        public IEnumerable<DataElement> GetFields()
        {
            foreach (var value in Data)
            {
                foreach (var elem in value.GetElementsList())
                    yield return elem;
            }
        }

        public DataElement GetFieldValue(string fieldName, int index)
        {
            return this[fieldName].ElementAt(index);
        }

        public void AddList(string key, List<DataElement> list)
        {
            if (!isReadOnly)
            {
                isReadOnly = true;
            }
            foreach (var elem in list)
            {
                this[key].AddElement(elem);
            };
        }

        IEnumerable<KeyValuePair<string, List<DataElement>>> IListableFields<DataElement>.GetFields()
        {
            foreach (var field in Data)
            {
                yield return new KeyValuePair<string, List<DataElement>>(field.GroupName, field.GetElementsList().ToList());
            }
        }
    }

    public class DataElement
    {
        public string Name { get; }
        public List<double> Values { get; private set; }

        public DataElement(string key)
        {
            Name = key;
            Values = new List<double>();
        }

        public void ChangeList(List<double> newList)
        {
            Values = newList;
        }
    }

    public class DataContainer : IListableFields<double>
    {
        private object Locker = new object();
        public string GroupName { get; }
        private List<DataElement> Elements;
        protected List<string> RestrictedNames = new List<string>();

        public List<double> this[string key]
        {
            get
            {
                lock (Locker)
                {
                    if (IsRestricted(key))
                        return new List<double>();
                    try
                    {
                        return Elements.Where(x => x.Name == key).First().Values;
                    }
                    catch
                    {
                        Elements.Add(new DataElement(key));
                        return Elements.Last().Values;
                    }
                }
            }
            set
            {
                lock (Locker)
                {
                    Elements.Where(x => x.Name == key).First().ChangeList(value);
                }
            }
        }

        public virtual void CalculateRestrictedFields()
        {
            return;
        }

        public bool ContainsKey(string key) => Elements.AsParallel().Any(e => e.Name == key);

        public List<string> ElementNames()
        {
            return Elements.AsParallel().Select(x => x.Name).ToList();
        }

        public void AddElement(DataElement element)
        {
            if (Elements.Any(x => x.Name == element.Name) || IsRestricted(element.Name))
            {
                return;
            }
            Elements.Add(element);
        }

        public bool IsRestricted(string key) => RestrictedNames.Contains(key);

        public IEnumerable<KeyValuePair<string, List<double>>> GetFields()
        {
            foreach (var elem in Elements)
            {
                yield return new KeyValuePair<string, List<double>>(elem.Name, elem.Values);
            }
        }

        public double GetFieldValue(string fieldName, int index)
        {
            return Elements.Where(x => x.Name == fieldName).First().Values[index];
        }

        public void AddValue(string key, double value)
        {
            this[key].Add(value);
        }

        public void AddList(string key, List<double> list)
        {
            if (IsRestricted(key))
                return;
            foreach (var elem in list.AsReadOnly())
            {
                AddValue(key, elem);
            }
        }

        public DataContainer(string name)
        {
            GroupName = name;
            Elements = new List<DataElement>();
        }

        public DataElement ElementAt(int index) => Elements[index];

        internal IEnumerable<DataElement> GetElementsList()
        {
            foreach (var elem in Elements)
            {
                yield return elem;
            };
        }
    }
}
