using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    public abstract class CharacterStat : MonoBehaviour, IStatContainer
    {
        [Header("Character")]
        [SerializeField]
        private StatProfileStorage profile;
        [Header("Setting")]
        [SerializeField]
        private BuffConfig[] DefaultBuff;


        protected DataManager SaveSystem;


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
        private void Start()
        {
            SaveSystem = Game.FindManager<DataManager>();
        }
        private void OnEnable()
        {
            for (int i = 0; i < profile.Count; i++)
            {
                profile.GetStatData(i).OnChangeValue += OnStatChange;
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < profile.Count; i++)
            {
                profile.GetStatData(i).OnChangeValue -= OnStatChange;
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
            OnBuffAppled(config);
        }
        public void DeclineBuff(BuffConfig config)
        {
            profile.DeclineBuff(config);
            OnBuffDeclined(config);
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
        public void Save()
        {
            if (SaveSystem)
            {
                SaveSystem.SaveFile(profile.file);
            }
            else
            {
                Debug.LogWarning("DataManger is not find in scen");
            }

        }

        // Abstract Methods
        protected abstract void OnStatChange(BaseStatData data);
        protected abstract void OnBuffAppled(BuffConfig config);
        protected abstract void OnBuffDeclined(BuffConfig config);
    }


    [AddComponentMenu("RealMethod/Toolkit/RPG/CharacterStat")]
    public class CharacterStatComponent : CharacterStat
    {
        [Header("Events")]
        public UnityEvent<BaseStatData> OnStatUpdate;

        // CharacterStat Methods
        protected override void OnStatChange(BaseStatData data)
        {
            OnStatUpdate?.Invoke(data);
        }
        protected override void OnBuffAppled(BuffConfig config)
        {

        }
        protected override void OnBuffDeclined(BuffConfig config)
        {

        }
        
    }

}