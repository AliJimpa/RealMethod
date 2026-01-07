
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Base configuration asset for game-wide settings and initialization logic.
    /// </summary>
    public abstract class GameConfig : ConfigAsset
    {
        [Header("Game")]
        [SerializeField]
        private float fadeTime = 0;
        /// <summary>
        /// Gets the duration (in seconds) used for screen fade transitions.
        /// </summary>
        public float FadeTime => fadeTime;

        /// <summary>
        /// Called when the game is initialized, allowing the configuration
        /// to apply settings or perform setup logic.
        /// </summary>
        /// <param name="Author">
        /// The Game instance responsible for initialization.
        /// </param>
        public abstract void Initialized(Game Author);
    }


}