using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/RPG/CharacterStat")]
    public class CharacterStat : MonoBehaviour, IStatContainer
    {
        [Header("Character")]
        [SerializeField]
        private StatProfile profile;
        [Header("Setting")]
        [SerializeField]
        private BuffConfig[] DefaultBuff;
        [Header("Events")]
        public UnityEvent<BaseStatData> OnStatUpdate;


        // Unity Methods
        private void Awake()
        {
            if (DefaultBuff != null)
            {
                foreach (var buff in DefaultBuff)
                {
                    profile.ApplyBuff(buff);
                }
            }
        }
        private void OnEnable()
        {
            for (int i = 0; i < profile.Count; i++)
            {
                profile.GetStatData(i).OnChangeValue += OnAnyStatChange;
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < profile.Count; i++)
            {
                profile.GetStatData(i).OnChangeValue -= OnAnyStatChange;
            }
        }


        // Implement IStatContainer Interface
        public IStat GetStat(int index)
        {
            return profile.GetStat(index);
        }
        public BaseStatData GetStatData(int index)
        {
            return profile.GetStatData(index);
        }
        public void ApplyBuff(BuffConfig config)
        {
            profile.ApplyBuff(config);
        }
        public void DeclineBuff(BuffConfig config)
        {
            profile.DeclineBuff(config);
        }
        public BaseStatData[] GetAllStatData()
        {
            if (profile != null)
            {
                BaseStatData[] result = new BaseStatData[profile.Count];
                for (int i = 0; i < profile.Count; i++)
                {
                    result[i] = profile.GetStatData(i);
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        // Private Functions
        private void OnAnyStatChange(BaseStatData data)
        {
            OnStatUpdate?.Invoke(data);
        }

    }
}