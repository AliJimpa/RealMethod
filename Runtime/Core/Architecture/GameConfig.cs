
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Base configuration asset for game-wide settings and initialization logic.
    /// </summary>
    public abstract class GameConfig : ConfigAsset
    {
        /// <summary>
        /// Called when the game is initialized, allowing the configuration
        /// to apply settings or perform setup logic.
        /// </summary>
        public abstract void Initialized();
    }


}