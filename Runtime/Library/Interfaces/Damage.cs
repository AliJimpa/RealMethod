using UnityEngine;

namespace RealMethod
{
    public interface IDamage
    {
        /// <summary>
        /// Returns true if the object is already dead.
        /// </summary>
        bool IsDead { get; }
        /// <summary>
        /// Called when the object dies or is destroyed due to damage.
        /// </summary>
        void Die();
    }
    public interface IDamageable : IDamage
    {
        float MaxHealth { get; }
        float CurrentHealth { get; }

        /// <summary>
        /// Simple damage (only amount + optional source)
        /// </summary>
        void TakeDamage(float amount, GameObject source = null);
        void Restore(float amount);
    }
    /// <summary>
    /// Interface for objects that can take and restore damage.
    /// </summary>
    /// <typeparam name="T">The type representing TakeDamage data (e.g., int, float, struct).</typeparam>
    public interface IDamageable<T, R> : IDamageable where T : struct where R : struct
    {
        /// <summary>
        /// Applies damage to the object.
        /// </summary>
        /// <param name="data">The raw data of damage.</param>
        /// <param name="source">The GameObject or system causing the damage.</param>
        void TakeDamage(T data);
        void Restore(R amount);
    }
}