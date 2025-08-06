using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    public abstract class BuffConfig : ConfigAsset, IIdentifier
    {
        [Header("StatConfig")]
        [SerializeField]
        private string configName;
        // Implement IIdentifier Interface
        public string NameID => configName;

        // Abstract Method
        public abstract IStatModifier[] GetModifiers<J>(J StateName) where J : System.Enum;

#if UNITY_EDITOR
        public void ChangeName(string NewName)
        {
            configName = NewName;
        }
#endif
    }
    public abstract class BuffConfig<T> : BuffConfig where T : System.Enum
    {
        [System.Serializable]
        private struct Modifier : IStatModifier
        {
            [SerializeField]
            private T Stat;
            private int cacheindex;
            private bool iscahed;
            public int StatIndex
            {
                get
                {
                    if (!iscahed)
                    {
                        cacheindex = System.Convert.ToInt32(Stat);
                        iscahed = true;
                    }
                    return cacheindex;
                }
            }
            [SerializeField]
            private float value;
            [SerializeField]
            private IStatModifier.StatUnitModifierType type;
            [SerializeField]
            private int priority;

            // Implement IStatModifier Interface
            public float Value => value;
            public IStatModifier.StatUnitModifierType Type => type;
            public int Priority => priority;
            public void UpdateValue(float newValue)
            {
                value = newValue;
            }
        }

        [Header("Modifiers")]
        [SerializeField]
        private Modifier[] presets;
        public int Count => presets != null ? presets.Length : 0;

        // StatModifierConfig Methods
        public sealed override IStatModifier[] GetModifiers<J>(J StateName)
        {
            List<IStatModifier> Result = new List<IStatModifier>();
            int targetindex = System.Convert.ToInt32(StateName);
            foreach (var pre in presets)
            {
                if (pre.StatIndex == targetindex)
                {
                    Result.Add(pre);
                }
            }
            return Result.ToArray();
        }

    }

}