using UnityEngine;

namespace RealMethod
{
    public abstract class UI_Tutorial : MonoBehaviour, ITutorialMessage
    {
        // Implement ITutorialMessage Interface
        public event ITutorialMessage.Finish OnFinished;
        T ITutorialMessage.GetClass<T>()
        {
            return this as T;
        }
        void ITutorialMessage.Initiate(Object author, Tutorial owner, TutorialConfig config)
        {
            OnInitiateTutorial(author, owner);
            OnStartTutorial(config);
        }

        // Unity Events
        private void OnDestroy()
        {
            OnFinished = null;
        }

        // Public Functions
        public void SetPosition(Vector3 position)
        {
            OnUpdatePosition(position);
        }
        public void Close()
        {
            OnFinished?.Invoke();
            OnEndTutorial();
        }

        protected abstract void OnInitiateTutorial(Object author, Tutorial owner);
        protected abstract void OnStartTutorial(TutorialConfig config);
        protected abstract void OnUpdatePosition(Vector3 pos);
        protected abstract void OnEndTutorial();
    }
}