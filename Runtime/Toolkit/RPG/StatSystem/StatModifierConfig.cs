using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class StatModifierConfig : ConfigAsset, IIdentifier
    {
        [Header("StatConfig")]
        [SerializeField]
        private string configName;
        // Implement IIdentifier Interface
        public string NameID => configName;

        // Abstract Method
        public abstract IStatModifier[] MakeModifiers<J>(J StateName, IStatStorage storage) where J : System.Enum;
    }
    public abstract class StatModifierConfig<T> : StatModifierConfig where T : System.Enum
    {
        [System.Serializable]
        private struct StatModifierPreset
        {
            [SerializeField]
            private T Stat;
            public int StatIndex => System.Convert.ToInt32(Stat);
            [SerializeField]
            private float value;
            public float Value => value;
            [SerializeField]
            private IStatModifier.StatUnitModifierType type;
            public IStatModifier.StatUnitModifierType Type => type;
            [SerializeField]
            private int priority;
            public int Priority => priority;
        }

        [Header("Modifiers")]
        [SerializeField]
        private StatModifierPreset[] presets;
        public int Count => presets != null ? presets.Length : 0;

        // StatModifierConfig Methods
        public sealed override IStatModifier[] MakeModifiers<J>(J StateName, IStatStorage storage)
        {
            List<IStatModifier> Result = new List<IStatModifier>();
            int targetindex = System.Convert.ToInt32(StateName);
            foreach (var pre in presets)
            {
                if (pre.StatIndex == targetindex)
                {
                    Result.Add(storage.CreateModifier(targetindex, this, pre.Value, pre.Type, pre.Priority));
                }
            }
            return Result.ToArray();
        }

    }

}