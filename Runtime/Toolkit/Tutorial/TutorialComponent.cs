using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Tutorial/TutorialScreen")]
    public class W_TutorialComponent : W_TutorialStorage
    {
        [Header("Events")]
        public UnityEvent OnDisplay;

        protected override void Initiate(bool isloaded)
        {
        }
        protected override void OnShownMessage(TutorialConfig config)
        {
            OnDisplay?.Invoke();
        }
    }
}