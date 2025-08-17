using UnityEngine;

namespace RealMethod
{
    public interface IResourceContainer
    {
        IResource GetResource(string nameID);
        IConsumableResource GetConsumableResource(string nameID);
        ResourceData GetResourceData(string nameID);
    }
    public interface IResourceData
    {
        void Initialize(StatProfile profile);
        void SetAdditiveValue(float val);
        float AdditiveMaxValue { get; }
    }

    public abstract class ResourceData : IIdentifier, IResource, IModifiableResource, IConsumableResource
    {
        [SerializeField]
        private string resourceName;
        [SerializeField]
        private float value;
        [SerializeField]
        private float defaultMaxValue;


        public IResource provider => this;
        public System.Action<IResource> OnChangeMaxValue;
        public System.Action<IResource> OnChangeValue;

        public ResourceData()
        {

        }
        public ResourceData(string ReName, float val, float max)
        {
            resourceName = ReName;
            defaultMaxValue = max;
            value = val;
        }

        // Implement IResource Interface
        float IResource.Value => value;
        float IResource.MaxValue => GetMaxValue(defaultMaxValue);
        // Implement IIdentifier Interface
        public string NameID => resourceName;
        void IResource.Refill()
        {
            value = provider.MaxValue;
            OnChangeValue?.Invoke(this);
        }
        void IResource.Deplete()
        {
            value = 0;
            OnChangeValue?.Invoke(this);
        }
        // Implement IModifiableResource Interface
        void IModifiableResource.Set(float val)
        {
            value = val;
            OnChangeValue?.Invoke(this);
        }
        void IModifiableResource.Modify(float amount)
        {
            value = Mathf.Clamp(value + amount, 0, provider.MaxValue);
            OnChangeValue?.Invoke(this);
        }
        // Implement IConsumableResource Interface
        bool IConsumableResource.CanConsume(float amount) => value >= amount;
        void IConsumableResource.Consume(float amount)
        {
            if (((IConsumableResource)this).CanConsume(amount))
            {
                value -= amount;
                OnChangeValue?.Invoke(this);
            }
        }

        // Abstract Method
        protected abstract float GetMaxValue(float defaultValue);
    }
    public abstract class ResourceData<T> : ResourceData, IResourceData where T : System.Enum
    {
        [System.Serializable]
        private class StatRatio : SerializableDictionary<T, float> { }
        // Variable
        [SerializeField, ReadOnly]
        private float additiveMaxValue = 0;
        [SerializeField, ReadOnly]
        private float maxStatsValue;
        [SerializeField]
        private StatRatio maxHealthStats;

        private StatProfile statProfile;

        public ResourceData(string ReName, float val, float max) : base(ReName, val, max)
        {
        }

        // ResourceData Methods
        protected sealed override float GetMaxValue(float defaultValue)
        {
            return defaultValue + maxStatsValue + additiveMaxValue;
        }

        // Implement IResourceDatainitializer Interface
        public void Initialize(StatProfile profile)
        {
            statProfile = profile;
            if (statProfile == null)
            {
                Debug.LogWarning($"The ResourceData need to StatProfiel, initializ failed!");
                return;
            }

            float Finalvalue = 0;
            foreach (var effect in maxHealthStats)
            {
                IStat TargetStat = statProfile.GetStat(effect.Key.ToString());
                if (TargetStat != null)
                {
                    TargetStat.BindNotify(AnyStatChange);
                    Finalvalue = Finalvalue + (TargetStat.Value * effect.Value);
                }
                else
                {
                    Debug.LogWarning($"Can't find Stat {effect.Key} in Profile {statProfile}");
                }
            }
            maxStatsValue = Finalvalue;
            OnChangeMaxValue?.Invoke(this);

            OnInitiate();
        }
        public void SetAdditiveValue(float val)
        {
            additiveMaxValue = val;
            OnChangeMaxValue?.Invoke(this);
        }
        public float AdditiveMaxValue => additiveMaxValue;

        // Private Functions
        private void AnyStatChange(IStat stat)
        {
            float Finalvalue = 0;
            foreach (var effect in maxHealthStats)
            {
                IStat TargetStat = statProfile.GetStat(effect.Key.ToString());
                if (TargetStat != null)
                {
                    Finalvalue = Finalvalue + (TargetStat.Value * effect.Value);
                }
                else
                {
                    Debug.LogWarning($"Can't find Stat {effect.Key} in Profile {statProfile}");
                }
            }
            maxStatsValue = Finalvalue;
            OnChangeMaxValue?.Invoke(this);
        }

        // Abstract Method
        protected abstract void OnInitiate();
    }


    // public class RegenerableResource : IRegenerableResource
    // {
    //     public float MaxValue { get; private set; }
    //     public float CurrentValue { get; private set; }

    //     public float RegenerationRate { get; private set; }

    //     public RegenerableResource(float maxValue, float regenerationRate)
    //     {
    //         MaxValue = maxValue;
    //         CurrentValue = maxValue;
    //         RegenerationRate = regenerationRate;
    //     }

    //     public void Regenerate(float deltaTime)
    //     {
    //         CurrentValue = Mathf.Min(CurrentValue + RegenerationRate * deltaTime, MaxValue);
    //     }
    // }

    // public class ChargeableResource : IChargeableResource
    // {
    //     private float _charge;
    //     private float _maxCharge;

    //     public bool IsFullyCharged => _charge >= _maxCharge;

    //     public ChargeableResource(float maxCharge)
    //     {
    //         _maxCharge = maxCharge;
    //         _charge = 0f;
    //     }

    //     public void Charge(float amount)
    //     {
    //         _charge = Mathf.Min(_charge + amount, _maxCharge);
    //     }

    //     public void ResetCharge()
    //     {
    //         _charge = 0f;
    //     }
    // }

    // public class TimeGatedResource : ITimeGatedResource
    // {
    //     public float CooldownDuration { get; private set; }
    //     public float CooldownRemaining { get; private set; }

    //     public bool IsAvailable => CooldownRemaining <= 0f;

    //     public TimeGatedResource(float cooldownDuration)
    //     {
    //         CooldownDuration = cooldownDuration;
    //         CooldownRemaining = 0f;
    //     }

    //     public void StartCooldown()
    //     {
    //         CooldownRemaining = CooldownDuration;
    //     }

    //     public void TickCooldown(float deltaTime)
    //     {
    //         if (CooldownRemaining > 0f)
    //             CooldownRemaining -= deltaTime;
    //     }
    // }

    // public class RefundableResource : IRefundableResource
    // {
    //     public float MaxValue { get; private set; }
    //     public float CurrentValue { get; private set; }

    //     public RefundableResource(float maxValue)
    //     {
    //         MaxValue = maxValue;
    //         CurrentValue = maxValue;
    //     }

    //     public float Refund(float percent)
    //     {
    //         float refundAmount = MaxValue * Mathf.Clamp01(percent);
    //         CurrentValue = Mathf.Min(CurrentValue + refundAmount, MaxValue);
    //         return refundAmount;
    //     }
    // }

    // public class DecayingResource : IDecayingResource
    // {
    //     public float MaxValue { get; private set; }
    //     public float CurrentValue { get; private set; }

    //     public float DecayRate { get; private set; }

    //     public DecayingResource(float maxValue, float decayRate)
    //     {
    //         MaxValue = maxValue;
    //         CurrentValue = maxValue;
    //         DecayRate = decayRate;
    //     }

    //     public void Decay(float deltaTime)
    //     {
    //         CurrentValue = Mathf.Max(CurrentValue - DecayRate * deltaTime, 0f);
    //     }
    // }
}