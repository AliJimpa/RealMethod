using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class TableAsset : DataAsset
    {
        public enum ModifyTableType
        {
            AddRow,
            RemoveRow,
            ClearTable
        }
        public Action<ModifyTableType> OnModifyTable;

        public abstract bool RemoveRow(string name);
        public abstract int GetRowCount();
        public abstract void ClearTable();

        // For Table Editor
        public abstract void FillList(ref List<string[]> Target, bool RowName);
    }
    public abstract class TableAsset<T> : TableAsset where T : struct
    {
        [Serializable]
        class RowKey<J>
        {
            public string key;
            public J value;

            public RowKey(string key, J value)
            {
                this.key = key;
                this.value = value;
            }
        }

        [SerializeField]
        private List<RowKey<T>> Table = new List<RowKey<T>>();
        private Dictionary<string, T> dataTable;


        private void OnEnable()
        {
            RebuildDictionary();
        }
        private void OnValidate()
        {
            RebuildDictionary();
        }


        public bool AddRow(string name, T value)
        {
            if (string.IsNullOrEmpty(name))
            {
                Debug.LogWarning("Row name cannot be null or empty.");
                return false;
            }

            if (dataTable.ContainsKey(name))
            {
                Debug.LogWarning($"A row with the name '{name}' already exists!");
                return false;
            }

            dataTable[name] = value;
            OnModifyTable?.Invoke(ModifyTableType.AddRow);
            RebuildList();
            return true;
        }
        public bool TryGetRow(string name, out T value)
        {
            return dataTable.TryGetValue(name, out value);
        }
        public Dictionary<string, T> GetAllRows()
        {
            return new Dictionary<string, T>(dataTable);
        }

        public override bool RemoveRow(string name)
        {
            if (dataTable.Remove(name))
            {
                RebuildList();
                return true;
            }

            OnModifyTable?.Invoke(ModifyTableType.RemoveRow);
            return false;
        }
        public override int GetRowCount()
        {
            return dataTable.Count;
        }
        public override void ClearTable()
        {
            dataTable.Clear();
            RebuildList();
            OnModifyTable?.Invoke(ModifyTableType.ClearTable);
        }
        public override void FillList(ref List<string[]> Target, bool RowName)
        {
            Target.Clear();
            foreach (var kvp in dataTable)
            {
                if (RowName)
                {
                    string[] CashRow = ConvertRowToString(kvp.Value);
                    string[] Result = new string[CashRow.Length + 1];
                    Result[0] = kvp.Key;
                    for (int i = 1; i < Result.Length; i++)
                    {
                        Result[i] = CashRow[i - 1];
                    }
                    Target.Add(Result);
                }
                else
                {
                    Target.Add(ConvertRowToString(kvp.Value));
                }
            }
        }

        protected abstract string[] ConvertRowToString(T Row);


        // Base Function for Dictionery
        private void RebuildDictionary()
        {
            dataTable = new Dictionary<string, T>();
            foreach (var entry in Table)
            {
                if (!dataTable.ContainsKey(entry.key))
                    dataTable.Add(entry.key, entry.value);
            }
        }
        private void RebuildList()
        {
            Table.Clear();
            foreach (var item in dataTable)
            {
                Table.Add(new RowKey<T>(item.Key, item.Value));
            }
        }



    }
}