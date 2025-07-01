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
    /// <summary>
    /// Interface for objects that can take and restore damage.
    /// </summary>
    /// <typeparam name="D">The type representing damage data (e.g., int, float, struct).</typeparam>
    public interface IDamage<D> : IDamage where D : struct
    {
        /// <summary>
        /// Applies damage to the object.
        /// </summary>
        /// <param name="data">The raw data of damage.</param>
        /// <param name="source">The GameObject or system causing the damage.</param>
        void TakeDamage(D data, GameObject source = null);
        /// <summary>
        /// Restore the structure by a certain data.
        /// </summary>
        void Restore(D data);
    }
    /// <summary>
    /// Extended interface for damageable objects with health tracking.
    /// </summary>
    /// <typeparam name="D">The type representing damage data.</typeparam>
    /// <typeparam name="H">The type representing health values (e.g., int, float).</typeparam>
    public interface IDamageable<D, H> : IDamage<D> where D : struct
    {
        H MaxHealth { get; }
        H CurrentHealth { get; }

        public delegate void OnHealthChangedDelegate(H amount);
    }
}