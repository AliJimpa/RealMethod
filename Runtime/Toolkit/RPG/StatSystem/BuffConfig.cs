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
        public abstract IStatModifier[] GetModifiers<T>(T StateName) where T : System.Enum;

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
            private T stat;
            public T Stat => stat;
            [Space]
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
        }
        [Header("Modifiers")]
        [SerializeField]
        private Modifier[] presets;
        public int Count => presets != null ? presets.Length : 0;

        public IStatModifier this[int index]
        {
            get
            {
                return presets[index];
            }
        }

        // StatModifierConfig Methods
        public sealed override IStatModifier[] GetModifiers<J>(J StateName)
        {
            List<IStatModifier> Result = new List<IStatModifier>();
            int targetindex = System.Convert.ToInt32(StateName);
            foreach (var modif in presets)
            {
                if (RM_Core.enume.AreEnumValuesEqual(modif.Stat , StateName))
                {
                    Result.Add(modif);
                }
            }
            return Result.ToArray();
        }

    }

}