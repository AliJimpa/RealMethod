using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Upgrade/Upgrade")]
    public sealed class UpgradeComponent : Upgrade
    {
        [Header("Save")]
        [SerializeField]
        private bool UseCustomFile;
        [SerializeField, ConditionalHide("UseCustomFile", true, true)]
        private SaveFile file;
        [Header("Events")]
        public UnityEvent<UpgradeAsset> Unlocked;

        // Base Upgrade Methods
        protected override void OnLockedAsset(UpgradeAsset asset)
        {

        }
        protected override void OnUnlockedAsset(UpgradeAsset asset)
        {
            Unlocked?.Invoke(asset);
        }
        protected override SaveFile GetSaveFile(DataManager savesystem)
        {
            if (UseCustomFile)
            {
                return file;
            }

            if (savesystem is SaveManager Storage)
            {
                if (Storage.Format == SaveMethod.PlayerPrefs)
                {
                    SaveFile Result = ScriptableObject.CreateInstance<UpgradeSaveFile_Prefs>();
                    Result.name = "RMUpgradeSaveFile";
                    return Result;
                }
                else
                {
                    Debug.LogError("Default Save File Can use just for PlayerPrefs");
                    return null;
                }
            }
            else
            {
                Debug.LogError("You didnt use SaveManger Drived Your Upgrade Class");
                return null;
            }
        }
    }


    public class UpgradeSaveFile_Prefs : UpgradeSaveFile
    {
        // Base SaveFile Method
        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
            RM_PlayerPrefs.SetArray("availableUpgrades", Avalable.ToArray());
            RM_PlayerPrefs.SetArray("unlockedUpgrades", UnAvalibal.ToArray());
            RM_PlayerPrefs.SetBool("UpgradeFile", true);
        }
        protected override void OnLoaded()
        {
            Avalable = RM_PlayerPrefs.GetArray<string>("availableUpgrades").ToList();
            UnAvalibal = RM_PlayerPrefs.GetArray<string>("unlockedUpgrades").ToList();
        }
        protected override void OnDeleted()
        {
            PlayerPrefs.DeleteKey("UpgradeFile");
        }
    }
}