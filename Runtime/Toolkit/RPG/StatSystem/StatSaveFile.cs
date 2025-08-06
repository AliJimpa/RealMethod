using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "StatSaveFile", menuName = "RealMethod/RPG/SaveFile", order = 1)]
    public class StatSaveFile : SaveFile, IStatStorage
    {
        [Header("Stat")]
        [SerializeField, ReadOnly, TextArea]
        protected string Description = "This Save file include IStatStorage for store data by StatProfile for saving Stat items";
        [SerializeField]
        private bool UsePlayerPrefs = true;
        [Header("Storage")]
        public List<string> Names = new List<string>(5);
        public List<float> Values = new List<float>(5);
        public List<float> Mins = new List<float>(5);
        public List<float> Maxs = new List<float>(5);


        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
            {
                RM_PlayerPrefs.SetArray("StatName", Names.ToArray());
                RM_PlayerPrefs.SetArray("StatBaseValue", Values.ToArray());
                RM_PlayerPrefs.SetArray("StatMin", Mins.ToArray());
                RM_PlayerPrefs.SetArray("StatMax", Maxs.ToArray());
            }
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
            {
                Names = RM_PlayerPrefs.GetArray<string>("StatName").ToList();
                Values = RM_PlayerPrefs.GetArray<float>("StatBaseValue").ToList();
                Mins = RM_PlayerPrefs.GetArray<float>("StatMin").ToList();
                Maxs = RM_PlayerPrefs.GetArray<float>("StatMax").ToList();
            }
        }
        protected override void OnDeleted()
        {
            if (UsePlayerPrefs)
            {
                Names = null;
                Values = null;
                Mins = null;
                Maxs = null;
            }
        }


        // Implement IStorage Interface
        void IStorage.StorageCreated(Object author)
        {
        }
        void IStorage.StorageLoaded(Object author)
        {
        }
        void IStorage.StorageClear()
        {
        }
        // Implement IStatStorage Interface
        void IStatStorage.StoreStats(IStat[] stats)
        {
            foreach (var stat in stats)
            {
                Names.Add(stat.NameID);
                Values.Add(stat.BaseValue);
                Mins.Add(stat.MinValue);
                Maxs.Add(stat.MaxValue);
            }
        }
        bool IStatStorage.TryLoadStats(StatData data)
        {
            if (Names.Contains(data.NameID))
            {
                int targetindex = Names.IndexOf(data.NameID);
                data.SetExtraValue(Values[targetindex] - data.FirstValue);
                data.SetLimitation(Mins[targetindex], Maxs[targetindex]);
            }
            return false;
        }
    }
}