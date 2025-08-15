using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    public abstract class CharacterStat : MonoBehaviour, IPrimitiveStatContainer
    {
        [Header("Character")]
        [SerializeField]
        private StatProfileStorage profile;
        public StatProfileStorage Profile { get; private set; }
        [Header("Setting")]
        [SerializeField]
        private BuffConfig[] DefaultBuff;


        protected DataManager SaveSystem;


        // Unity Methods
        private void Awake()
        {
            Profile = Instantiate(Profile);
            if (DefaultBuff != null)
            {
                foreach (var buff in DefaultBuff)
                {
                    Profile.ApplyBuff(buff);
                }
            }
        }
        private void Start()
        {
            SaveSystem = Game.FindManager<DataManager>();
        }
        private void OnEnable()
        {
            for (int i = 0; i < Profile.Count; i++)
            {
                Profile.GetStat(i).BindNotify(OnStatChange);
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < Profile.Count; i++)
            {
                Profile.GetStat(i).UnbindNotify(OnStatChange);
            }
        }


        // Implement IStatContainer Interface
        public IStat GetStat(int index)
        {
            return Profile.GetStat(index);
        }
        public BaseStatData GetStatData(int index)
        {
            return Profile.GetStatData(index);
        }
        public void ApplyBuff(BuffConfig config)
        {
            Profile.ApplyBuff(config);
            OnBuffAppled(config);
        }
        public void DeclineBuff(BuffConfig config)
        {
            Profile.DeclineBuff(config);
            OnBuffDeclined(config);
        }
        public BaseStatData[] GetAllStatData()
        {
            if (Profile != null)
            {
                BaseStatData[] result = new BaseStatData[Profile.Count];
                for (int i = 0; i < Profile.Count; i++)
                {
                    result[i] = Profile.GetStatData(i);
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
                Profile.StoreStats();
                SaveSystem.SaveFile(Profile.file);
            }
            else
            {
                Debug.LogWarning("DataManger is not find in scen");
            }

        }
        void IPrimitiveStatContainer.InitializeResource(IResourceData resource)
        {
            resource.Initialize(Profile);
        }

        // Abstract Methods
        protected abstract void OnStatChange(IStat data);
        protected abstract void OnBuffAppled(BuffConfig config);
        protected abstract void OnBuffDeclined(BuffConfig config);

    }


    [AddComponentMenu("RealMethod/Toolkit/RPG/CharacterStat")]
    public class CharacterStatComponent : CharacterStat
    {
        [Header("Events")]
        public UnityEvent<IStat> OnStatUpdate;

        // CharacterStat Methods
        protected override void OnStatChange(IStat data)
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