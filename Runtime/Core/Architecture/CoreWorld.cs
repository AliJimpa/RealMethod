using UnityEngine;

namespace RealMethod
{
    public abstract class CoreWorld : MonoBehaviour
    {
        [Header("World")]
        [SerializeField]
        protected GameObject[] ExteraObject;
        private void Awake()
        {
            GameService Gameserv = (GameService)CoreGame.FindService("Game");
            Gameserv.IntroduceWorld(this, AdditiveWorld);
        }

        protected virtual void AdditiveWorld(CoreWorld TargetWorld)
        {
            Debug.LogWarning($"This World Class ({TargetWorld}) Deleted");
            Destroy(TargetWorld);
        }

    }
}