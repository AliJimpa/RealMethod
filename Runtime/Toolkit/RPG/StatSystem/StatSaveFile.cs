using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "StatSaveFile", menuName = "RealMethod/RPG/SaveFile", order = 1)]
    public class StatSaveFile : SaveFile, IStatStorage
    {
        [System.Serializable]
        private class Sg_Modifier : IStatModifier
        {
            public int StatIndex { get; private set; }
            public string ConfigName { get; private set; }
            public float Value { get; private set; }
            public IStatModifier.StatUnitModifierType Type { get; private set; }
            public int Priority { get; private set; }
            public Sg_Modifier(int statIndex, IIdentifier config, float value, IStatModifier.StatUnitModifierType type, int priority)
            {
                StatIndex = statIndex;
                ConfigName = config.NameID;
                Value = value;
                Type = type;
                Priority = priority;
            }
            public void UpdateValue(float newValue)
            {
                Value = newValue;
            }
        }
        private class Sg_Stat
        {
            public float value;
            public float min;
            public float max;
        }


        [SerializeField, ReadOnly]
        private List<Sg_Modifier> modifiersObject = new List<Sg_Modifier>(5);
        private Hictionary<Sg_Stat> Stats = new Hictionary<Sg_Stat>();


        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
        }
        protected override void OnLoaded()
        {
        }
        protected override void OnDeleted()
        {
        }


        // Implement IStorage Interface
        void IStorage.StorageCreated(Object author)
        {
            modifiersObject.Clear();
            Stats.Clear();
        }
        void IStorage.StorageLoaded(Object author)
        {
        }
        void IStorage.StorageClear()
        {
            modifiersObject.Clear();
            Stats.Clear();
        }
        // Implement IStatStorage Interface
        IStatModifier IStatStorage.CreateModifier(int StatIndex, IIdentifier config, float value, IStatModifier.StatUnitModifierType type, int priority)
        {
            Sg_Modifier NewModif = new(StatIndex, config, value, type, priority);
            modifiersObject.Add(NewModif);
            return NewModif;
        }
        IStatModifier[] IStatStorage.GetModifiers(int StatIndex)
        {
            List<IStatModifier> Filterd = new List<IStatModifier>();
            foreach (var modif in modifiersObject)
            {
                if (modif.StatIndex == StatIndex)
                {
                    Filterd.Add(modif);
                }
            }
            return Filterd.ToArray();
        }
        public void RemoveModifier(int StatIndex, IIdentifier config, IModifiableStat StatData)
        {
            foreach (var modif in modifiersObject)
            {
                if (modif.StatIndex == StatIndex)
                {
                    if (modif.ConfigName == config.NameID)
                    {
                        StatData.RemoveModifier(modif);
                        modifiersObject.Remove(modif);
                        return;
                    }
                }
            }
        }

        void IStatStorage.StartStats(IStat[] stats)
        {
            foreach (var stat in stats)
            {
                Sg_Stat myval = new Sg_Stat();
                myval.value = stat.BaseValue;
                myval.min = stat.MinValue;
                myval.max = stat.MaxValue;
                Stats.Add(stat.NameID, myval);
            }
        }
        bool IStatStorage.TryLoadStats(StatData data)
        {
            if (Stats.ContainsKey(data.NameID))
            {
                Sg_Stat myvalue = Stats[data.NameID];
                data.SetValue(myvalue.value);
                data.SetLimitation(myvalue.min, myvalue.max);
            }
            return false;
        }
    }
}