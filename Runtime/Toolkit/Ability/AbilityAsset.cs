using UnityEngine;

namespace RealMethod
{
    public abstract class AbilityAsset : DataAsset, IAbility, ICooldown
    {
        private float lastUsedTime = -Mathf.Infinity;

        // Implement IIdentifier Interface
        string IIdentifier.NameID => name;
        // Implement IAbility Interface
        public bool CanUse(GameObject user)
        {
            if (user == null)
            {
                Debug.LogWarning($"{this} can't use user is not valid");
                return false;
            }
            return Prerequisite(user) && ((ICooldown)this).IsAvailable;
        }
        public bool TryUse(GameObject user, IAbilityContext context)
        {
            if (CanUse(user))
            {
                foreach (var effect in GetEffects())
                {
                    CheckEffect(effect).Apply(user, context);
                }
                ((ICooldown)this).StartCooldown();
                return true;
            }
            return false;
        }
        // Implement ICooldown Interface
        public float CooldownDuration => GetCooldown();
        bool ICooldown.IsAvailable => Time.time > lastUsedTime + GetCooldown();
        void ICooldown.StartCooldown()
        {
            lastUsedTime = Time.time;
        }

        // Public Functions
        public void ResetCooldown()
        {
            lastUsedTime = -Mathf.Infinity;
        }

        // Abstract Methods
        protected abstract float GetCooldown();
        protected abstract AbilityEffect[] GetEffects();
        protected abstract bool Prerequisite(GameObject user);
        protected abstract IAbilityEffect CheckEffect(AbilityEffect effect);
    }
}