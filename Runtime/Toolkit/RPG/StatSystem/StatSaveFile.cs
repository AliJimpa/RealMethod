using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "StatSaveFile", menuName = "RealMethod/RPG/StatSaveFile", order = 1)]
    public class StatSaveFile : SaveAsset, IStatStorage
    {
        [Header("Stat")]
        [SerializeField, ReadOnly, TextArea]
        protected string Description = "This Save file include IStatStorage for store data by StatProfile for saving Stat items";
        [SerializeField]
        private bool UsePlayerPrefs = true;
        [Header("Storage")]
        public List<string> Names = new List<string>(5);
        public List<float> BaseValue = new List<float>(5);
        public List<float> Mins = new List<float>(5);
        public List<float> Maxs = new List<float>(5);

        // SaveFile Methods
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
            {
                RM_Save.SetArray("StatName", Names.ToArray());
                RM_Save.SetArray("StatBaseValue", BaseValue.ToArray());
                RM_Save.SetArray("StatMin", Mins.ToArray());
                RM_Save.SetArray("StatMax", Maxs.ToArray());
            }
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
            {
                Names = RM_Save.GetArray<string>("StatName").ToList();
                BaseValue = RM_Save.GetArray<float>("StatBaseValue").ToList();
                Mins = RM_Save.GetArray<float>("StatMin").ToList();
                Maxs = RM_Save.GetArray<float>("StatMax").ToList();
            }
        }

        // Implement IStorage Interface
        // void IStorage.StorageCreated(Object author)
        // {
        // }
        // void IStorage.StorageLoaded(Object author)
        // {
        // }
        // void IStorage.StorageClear()
        // {
        // }
        // Implement IStatStorage Interface
        void IStatStorage.StoreStats(IStat[] stats)
        {
            foreach (var stat in stats)
            {
                if (Names.Contains(stat.NameID))
                {
                    int targetindex = Names.IndexOf(stat.NameID);
                    BaseValue[targetindex] = stat.BaseValue;
                    Mins[targetindex] = stat.MinValue;
                    Maxs[targetindex] = stat.MaxValue;
                }
                else
                {
                    Names.Add(stat.NameID);
                    BaseValue.Add(stat.BaseValue);
                    Mins.Add(stat.MinValue);
                    Maxs.Add(stat.MaxValue);
                }
            }
        }
        bool IStatStorage.TryLoadStats(StatData data)
        {
            if (Names.Contains(data.NameID))
            {
                int targetindex = Names.IndexOf(data.NameID);
                data.SetUpgradeValue(BaseValue[targetindex] - data.FirstValue);
                data.SetLimitation(Mins[targetindex], Maxs[targetindex]);
                return true;
            }
            return false;
        }


        // Unity Event
        protected override void Reset()
        {
            base.Reset();
            
            Names.Clear();
            BaseValue.Clear();
            Mins.Clear();
            Maxs.Clear();
        }
    }
}