using UnityEngine;

namespace RealMethod
{
    public abstract class Tutorial : MonoBehaviour
    {
        public System.Action<UI_Tutorial> OnMessageShow;
        protected ITutorialStorage tutorialStorage;

        // Unity Methods
        private void Awake()
        {
            tutorialStorage = GetStorage();
            if (tutorialStorage == null)
            {
                Debug.LogWarning("Storage is Not Valid");
                enabled = false;
                return;
            }
            LoadStorage();
        }

        // Public Functions
        public bool IsMessageShown(TutorialConfig config)
        {
            return tutorialStorage.IsValidTutorial(config);
        }
        public bool TryShowMessage<T>(TutorialConfig config, Object author, out T provider) where T : UI_Tutorial
        {
            if (IsMessageShown(config))
            {
                provider = null;
                return false;
            }
            provider = ShowMessage<T>(config, author);
            return true;
        }
        public T ShowMessage<T>(TutorialConfig config, Object author, ITutorialMessage.Finish callback) where T : UI_Tutorial
        {
            return ShowMessage<T>(config, transform, author, callback);
        }
        public T ShowMessage<T>(TutorialConfig config, Transform parent, Object author, ITutorialMessage.Finish callback) where T : UI_Tutorial
        {
            ITutorialSpawner spawner = config;
            ITutorialMessage provider = spawner.InstantiateMessage(parent);
            provider.Initiate(author, this, config);
            provider.OnFinished += callback;
            MessageShown(config, provider);
            return provider.GetClass<T>();
        }
        public T ShowMessage<T>(TutorialConfig config, Object author) where T : UI_Tutorial
        {
            return ShowMessage<T>(config, transform, author);
        }
        public T ShowMessage<T>(TutorialConfig config, Transform parent, Object author) where T : UI_Tutorial
        {
            ITutorialSpawner spawner = config;
            ITutorialMessage provider = spawner.InstantiateMessage(parent);
            provider.Initiate(author, this, config);
            MessageShown(config, provider);
            return provider.GetClass<T>();
        }
        public bool ResetMessage(TutorialConfig config)
        {
            return tutorialStorage.RemoveTutorial(config);
        }
        public void Clear()
        {
            tutorialStorage.StorageClear();
        }

        // Private Functions
        private void MessageShown(TutorialConfig config, ITutorialMessage provider)
        {
            tutorialStorage.AddNewTutorial(config);
            OnShownMessage(config);
            OnMessageShow?.Invoke(provider.GetClass<UI_Tutorial>());
        }

        // Abstract Methods 
        protected abstract void OnShownMessage(TutorialConfig config);
        protected abstract ITutorialStorage GetStorage();
        protected abstract bool LoadStorage();
    }
    public abstract class TutorialStorage : Tutorial
    {
        [Header("Save")]
        [SerializeField]
        private StorageFile<ITutorialStorage, TutorialSaveFile> storage;
        public SaveFile file => storage.file;

        protected sealed override ITutorialStorage GetStorage()
        {
            return storage.provider;
        }
        protected sealed override bool LoadStorage()
        {
            return storage.Load(this);
        }
    }
}