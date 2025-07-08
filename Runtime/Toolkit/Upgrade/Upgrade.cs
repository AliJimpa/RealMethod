using UnityEngine;

namespace RealMethod
{
    public abstract class Upgrade : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField]
        private UpgradeConfig[] Configs;



        private Hictionary<UpgradeAsset> UpgradesAsset = new Hictionary<UpgradeAsset>(5);
        public int Count => UpgradesAsset.Count;
        private UpgradeSaveFile SaveFile;
        private DataManager SaveManager;


        // Unity Methods
        protected virtual void Awake()
        {
            // Find Data Maanger 
            SaveManager = Game.FindManager<DataManager>();
            if (SaveManager == null)
            {
                Debug.LogError($"Can't find manager [{typeof(DataManager)}] ! ");
                enabled = false;
            }

            // Create Clone of all UpgradeAssets
            foreach (var conf in Configs)
            {
                UpgradeAsset previousasset = null;
                foreach (var Uasset in conf.line)
                {
                    UpgradeAsset NewAsset = Instantiate(Uasset);
                    IUpgradeable IController = NewAsset;
                    if (IController.Initiate(this, conf, previousasset))
                    {
                        UpgradesAsset.Add(Uasset.Title, NewAsset);
                        previousasset = NewAsset;
                    }
                }
            }

            // Create SaveFile
            SaveFile = ScriptableObject.CreateInstance<UpgradeSaveFile>();
            SaveFile.name = "RMUpgradeSaveFile";


            // Load File
            if (SaveManager.IsExistFile(SaveFile))
            {
                SaveManager.LoadFile(SaveFile);
            }
            else
            {
                SaveFile.Initiate(this , UpgradesAsset.GetKeys());
            }
        }

        // Publci Functions
        public bool IsUnlocked(string Title)
        {
            return SaveFile.IsUnAvalibal(Title);
        }
        public bool CanUnlock(string Title)
        {
            return FindAsset(Title).CanUnlock();
        }
        public void Unlock(string Title)
        {
            if (CanUnlock(Title))
            {
                UnlockeAsset(FindAsset(Title), false);
            }
        }
        public void ForceUnlock(string Title)
        {
            UnlockeAsset(FindAsset(Title), true);
        }
        public void Lock(string Title)
        {
            if (IsUnlocked(Title))
            {
                LockAsset(FindAsset(Title));
            }
            else
            {
                Debug.LogWarning("This asset is already Locked");
            }
        }
        public UpgradeAsset FindAsset(string Title)
        {
            return UpgradesAsset[Title];
        }


        // Private Functions
        private void UnlockeAsset(UpgradeAsset asset, bool free)
        {
            if (SaveFile.SwapToUnAvalibal(asset.Title))
            {
                IUpgradeable IController = asset;
                IController.SetUnlock(free);
            }
            else
            {
                Debug.LogError("There is issue please Remove UpgradeSavefile");
            }
        }
        private void LockAsset(UpgradeAsset asset)
        {
            if (SaveFile.SwapToAvalibal(asset.Title))
            {
                IUpgradeable IController = asset;
                IController.SetLock();
            }
            else
            {
                Debug.LogError("There is issue please Remove UpgradeSavefile");
            }
        }

        // Abstract Methods
        public abstract void InitiateService(Service service);

    }





}