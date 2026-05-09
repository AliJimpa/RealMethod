namespace RealMethod
{
    public interface IDamageable
    {
        /// <summary>
        ///  Simple damage (only amount + optional source)
        /// </summary>
        /// <param name="HitInfo">Incloude all data that need for reperesent damage</param>
        void TakeDamage(HitData HitInfo);
        /// <summary>
        /// Called when the object dies or is destroyed due to damage.
        /// </summary>
        void Die();
    }

}