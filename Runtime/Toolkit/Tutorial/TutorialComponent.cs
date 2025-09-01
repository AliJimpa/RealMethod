using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Tutorial/TutorialScreen")]
    public class TutorialComponent : W_TutorialStorage
    {
        [Header("Events")]
        public UnityEvent OnDisplay;
        
        protected override void OnShownMessage(TutorialConfig config)
        {
            OnDisplay?.Invoke();
        }
    }
}