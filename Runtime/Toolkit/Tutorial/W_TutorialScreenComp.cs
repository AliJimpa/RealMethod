using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Tutorial/TutorialScreen")]
    public class W_TutorialScreenComp : W_TutorialStorage
    {
        [Header("Events")]
        public UnityEvent OnDisplay;

        public override void OnInitiateWidget(Object Owner)
        {
            print("fffff");
        }
        protected override void OnTutorialLoaded(bool hasValue)
        {
            print("KKKKKK");
        }
        protected override void OnShownMessage(TutorialConfig config)
        {
            OnDisplay?.Invoke();
        }
    }
}