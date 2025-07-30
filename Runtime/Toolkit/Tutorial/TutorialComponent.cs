using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Tutorial/Tutorial")]
    public class TutorialComponent : TutorialStorage
    {
        [Header("Events")]
        public UnityEvent OnDisplay;
        
        protected override void OnTutorialShown(TutorialConfig config)
        {
            OnDisplay?.Invoke();
        }
    }
}