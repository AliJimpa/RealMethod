
using UnityEngine;

namespace RealMethod
{
    public abstract class GameConfig : ConfigAsset
    {
        [Header("Game")]
        [SerializeField]
        private float fadeTime = 0;
        public float FadeTime => fadeTime;

        public abstract void GameStarted(Game Author);
    }

}