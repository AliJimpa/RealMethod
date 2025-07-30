using UnityEngine;

namespace RealMethod
{
    public abstract class Tutorial : MonoBehaviour
    {
        public System.Action<ITutorialMessage> OnTutorialShow;
        protected ITutorialStorage tutorialStorage;

        // Unity Methods
        protected virtual void Start()
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
        public bool IsTutorialShown(TutorialConfig config)
        {
            return tutorialStorage.IsValidTutorial(config);
        }
        public bool DisplayTutorial(TutorialConfig config, Object author, out ITutorialMessage provider)
        {
            if (IsTutorialShown(config))
            {
                provider = null;
                return false;
            }
            provider = ShowTutorial(config, author);
            return true;
        }
        public ITutorialMessage ShowTutorial(TutorialConfig config, Object author, ITutorialMessage.Finish callback)
        {
            return ShowTutorial(config, transform, author, callback);
        }
        public ITutorialMessage ShowTutorial(TutorialConfig config, Transform parent, Object author, ITutorialMessage.Finish callback)
        {
            ITutorialSpawner spawner = config;
            ITutorialMessage provider = spawner.InstantiateObject(parent);
            provider.Initiate(author, this, config);
            provider.OnFinished += callback;
            OnTutorialMessageShown(config, provider);
            return provider;
        }
        public ITutorialMessage ShowTutorial(TutorialConfig config, Object author)
        {
            return ShowTutorial(config, transform, author);
        }
        public ITutorialMessage ShowTutorial(TutorialConfig config, Transform parent, Object author)
        {
            ITutorialSpawner spawner = config;
            ITutorialMessage provider = spawner.InstantiateObject(parent);
            provider.Initiate(author, this, config);
            OnTutorialMessageShown(config, provider);
            return provider;
        }
        public bool RemoveFromStorage(TutorialConfig config)
        {
            return tutorialStorage.RemoveTutorial(config);
        }
        public void Clear()
        {
            tutorialStorage.StorageClear();
        }

        // Private Functions
        private void OnTutorialMessageShown(TutorialConfig config, ITutorialMessage provider)
        {
            tutorialStorage.AddNewTutorial(config);
            OnTutorialShown(config);
            OnTutorialShow?.Invoke(provider);
        }

        // Abstract Methods 
        protected abstract void OnTutorialShown(TutorialConfig config);
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