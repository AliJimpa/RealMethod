using UnityEngine;

namespace RealMethod
{
    public interface IDamage<T> where T : struct
    {
        /// <summary>
        /// Applies damage to the object.
        /// </summary>
        /// <param name="data">The raw data of damage.</param>
        /// <param name="source">The GameObject or system causing the damage.</param>
        void TakeDamage(T data, GameObject source = null);

        /// <summary>
        /// Called when the object dies or is destroyed due to damage.
        /// </summary>
        void Die();

        /// <summary>
        /// Restore the structure by a certain data.
        /// </summary>
        void Restore(T data);

        /// <summary>
        /// Returns true if the object is already dead.
        /// </summary>
        bool IsDead { get; }
    }

    public interface IDamageable<T,J> : IDamage<T> where T : struct
    {
        J MaxHealth { get; }
        J CurrentHealth { get; }

        public delegate void OnHealthChangedDelegate(J amount);
    }


}