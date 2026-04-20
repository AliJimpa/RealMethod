using UnityEngine;

namespace RealMethod
{

    public interface IStorage
    {
        void StorageCreated(Object author);
        void StorageLoaded(Object author);
        void StorageClear();
    }
    [System.Serializable]
    public struct StorageFile<T, J> where T : IStorage where J : FileAsset
    {
        [SerializeField]
        private bool UseCustomFile;
        [SerializeField, ConditionalHide("UseCustomFile", true, false)]
        private J _SaveFile;
        public J file
        {
            get
            {
                if (_SaveFile == null)
                {
                    if (!TryGetStorage(out cacheProvider))
                    {
                        Debug.LogWarning($"{this}: Can't get Storage Interface Something is wrong in your 'CustomSavefile' or 'SaveFileClass'");
                        return default;
                    }
                }
                return _SaveFile;
            }
        }
        private T cacheProvider;
        public T provider
        {
            get
            {
                if (cacheProvider == null)
                {
                    if (!TryGetStorage(out cacheProvider))
                    {
                        Debug.LogWarning($"{this}: Can't get Storage Interface Something is wrong in your 'CustomSavefile' or 'SaveFileClass'");
                        return default;
                    }
                }
                return cacheProvider;
            }
        }


        // Public Functions
        public bool Load(Object author)
        {
            ISaveSystem Savesystem = Game.Instance.gameObject.GetComponent<ISaveSystem>();
            if (Savesystem == null)
                Savesystem = Game.World.gameObject.GetComponent<ISaveSystem>();

            return Load(author, Savesystem);
        }
        public bool Load(Object author, ISaveSystem manager)
        {
            if (manager == null)
            {
                Debug.LogWarning($"{this}: Initiate faield we need SaveManager");
                return false;
            }

            if (manager.IsExist(file))
            {
                manager.Load(file);
                provider.StorageLoaded(author);
                return true;
            }
            else
            {
                provider.StorageCreated(author);
                return false;
            }
        }
        public void Clear()
        {
            provider.StorageClear();
        }

        // Private Functions
        private bool TryGetStorage(out T _provider)
        {
            if (UseCustomFile)
            {
                if (_SaveFile is T customProvider)
                {
                    _provider = customProvider;
                    return true;
                }
                else
                {
                    Debug.LogWarning("Storage Interface not implemented in Customfile.");
                    UseCustomFile = false;
                }
            }
            _SaveFile = ScriptableObject.CreateInstance<J>();
            if (_SaveFile is T autoProvider)
            {
                _SaveFile.name = $"RM{typeof(J)}";
                _provider = autoProvider;
                return true;
            }
            else
            {
                Debug.LogError($"Storage Interface not implemented in SaveFileType {typeof(J)}");
                _provider = default;
                return false;
            }
        }
    }

}