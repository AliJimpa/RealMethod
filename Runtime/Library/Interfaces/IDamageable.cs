using UnityEngine;

namespace RealMethod
{
    public interface IDamage<T>
    {
        /// <summary>
        /// Applies damage to the object.
        /// </summary>
        /// <param name="amount">The raw amount of damage.</param>
        /// <param name="source">The GameObject or system causing the damage.</param>
        void TakeDamage(T amount, GameObject source = null);

        /// <summary>
        /// Called when the object dies or is destroyed due to damage.
        /// </summary>
        void Die();

        /// <summary>
        /// Repairs the structure by a certain amount.
        /// </summary>
        void Repair(T amount);

        /// <summary>
        /// Returns true if the object is already dead.
        /// </summary>
        bool IsDead { get; }
    }

    public interface IDamageable : IDamage<int>
    {
        int MaxHealth { get; }
        int CurrentHealth { get; }

        public delegate void OnHealthChangedDelegate(int amount);
    }


}