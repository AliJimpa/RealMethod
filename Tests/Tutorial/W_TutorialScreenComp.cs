using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Tutorial/TutorialScreen")]
    public class W_TutorialScreenComp : W_TutorialStorage
    {
        [Header("Events")]
        public UnityEvent OnDisplay;

        public override void OnSceneInitialized(UIManager manager)
        {
        }
        protected override void OnTutorialLoaded(bool hasValue)
        {
        }
        protected override void OnShownMessage(TutorialConfig config)
        {
            OnDisplay?.Invoke();
        }
    }
}