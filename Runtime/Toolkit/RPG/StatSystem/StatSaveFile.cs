using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "StatSaveFile", menuName = "RealMethod/RPG/SaveFile", order = 1)]
    public class StatSaveFile : SaveFile, IStatStorage
    {
        [System.Serializable]
        private class Modifier : IStatModifier, IIdentifier
        {
            public string NameID { get; private set; }
            public string ConfigID { get; private set; }
            public int ModifyIndex { get; private set; }
            public float Value { get; private set; }
            public IStatModifier.StatUnitModifierType Type { get; private set; }
            public int Priority { get; private set; }
            public Modifier(IIdentifier statID, IIdentifier buffID, int index, float value, IStatModifier.StatUnitModifierType type, int priority)
            {
                NameID = statID.NameID;
                ConfigID = buffID.NameID;
                ModifyIndex = index;
                Value = value;
                Type = type;
                Priority = priority;
            }
            public void UpdateValue(float newValue)
            {
                Value = newValue;
            }
        }

        [SerializeField, ReadOnly]
        private List<Modifier> modifiersObject = new List<Modifier>(5);



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

        }
        void IStorage.StorageLoaded(Object author)
        {
        }
        void IStorage.StorageClear()
        {
        }
        // Implement IStatStorage Interface
        IStatModifier IStatStorage.CreateModifier(IStat stat, IIdentifier ModiferID, int ModiferIndex, float value, IStatModifier.StatUnitModifierType type, int priority)
        {
            Modifier NewModif = new(stat, ModiferID, ModiferIndex, value, type, priority);
            modifiersObject.Add(NewModif);
            return NewModif;
        }
        public IStatModifier GetModifier(IIdentifier ModiferID, int ModiferIndex)
        {
            foreach (var modif in modifiersObject)
            {
                if (modif.ConfigID == ModiferID.NameID)
                {
                    if (modif.ModifyIndex == ModiferIndex)
                    {
                        return modif;
                    }
                }
            }
            return null;
        }
        public void RemoveModifier(IIdentifier ModiferID, int ModiferIndex)
        {
            foreach (var modif in modifiersObject)
            {
                if (modif.ConfigID == ModiferID.NameID)
                {
                    if (modif.ModifyIndex == ModiferIndex)
                    {
                        modifiersObject.Remove(modif);
                        return;
                    }
                }
            }
            Debug.LogWarning($"Can't find Modifer: {ModiferID.NameID} with Index: {ModiferIndex} for Remove");
        }



    }
}