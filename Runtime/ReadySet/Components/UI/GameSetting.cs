using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace RealMethod
{
    [Serializable]
    public class SelectableElement : SerializableDictionary<string, Selectable> { }

    [AddComponentMenu("RealMethod/UI/GameSetting")]
    public sealed class GameSetting : MonoBehaviour
    {
        [Header("File")]
        [SerializeField]
        private SettingFile settingFile;
        [SerializeField]
        private FieldContainer ScanSetting;
        [Header("Resource")]
        [SerializeField]
        private SelectableElement UIElements;

        private DataManager saveManager;
        private ISettingStorage storage => settingFile.provider;
        public bool isFileDirty => storage.IsSettingDirty;


        // Unity Method
        private void Awake()
        {
            // Check 'SettingFile' for saving data
            if (settingFile == null)
            {
                Debug.LogError($"{this} savefile is not valid ");
                enabled = false;
                return;
            }

            // Sacan all variable in save file & check
            ScanSetting.Scan(settingFile);
            if (!ScanSetting.isStore)
            {
                Debug.LogError($"{this} should initiat by UIManager");
                enabled = false;
                return;
            }

            // Find save manger & check
            saveManager = Game.FindManager<DataManager>();
            if (saveManager != null)
            {
                Debug.LogError($"{this} Can't find DataManager");
                enabled = false;
                return;
            }

            // If file saved befor first load data else FirstSave
            if (saveManager.IsExistFile(settingFile))
            {
                saveManager.LoadFile(settingFile);
                storage.OnSettingStarted();
            }
            else
            {
                storage.OnSettingCreated();
            }
        }
        private void Start()
        {
            SyncUI();
        }


        // Public Functions
        public Selectable FindElement(string label)
        {
            return UIElements[label];
        }
        public string FindLabel(Selectable element)
        {
            foreach (var elem in UIElements)
            {
                if (elem.Value == element)
                {
                    return elem.Key;
                }
            }
            return string.Empty;
        }
        public void SaveSetting()
        {
            saveManager.SaveFile(settingFile);
        }
        public void SyncFile(Selectable element)
        {
            string label = FindLabel(element);
            if (label == string.Empty)
            {
                Debug.LogError("Can't Find this element!");
                return;
            }
            foreach (FieldInfo field in ScanSetting.GetFields())
            {
                object value = field.GetValue(settingFile);
                switch (value)
                {
                    case float f:
                        if (element is Slider slide)
                        {
                            if (field.Name == label)
                                field.SetValue(settingFile, slide.value);
                        }
                        break;
                    case int i:
                        if (element is Dropdown dropdown)
                        {
                            if (field.Name == label)
                                field.SetValue(settingFile, dropdown.value);
                        }
                        break;
                    case bool b:
                        if (element is Toggle toggle)
                        {
                            if (field.Name == label)
                                field.SetValue(settingFile, toggle.isOn);
                        }
                        break;
                    default:
                        Debug.Log($"Unkown field: {field.Name}, Type: {field.FieldType}, Value: {value}");
                        break;
                }
            }
            storage.OnFileSynced();
        }
        public void SyncFile()
        {
            foreach (var element in UIElements)
            {
                foreach (FieldInfo field in ScanSetting.GetFields())
                {
                    object value = field.GetValue(settingFile);
                    switch (value)
                    {
                        case float f:
                            if (element.Value is Slider slide)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(settingFile, slide.value);
                            }
                            break;
                        case int i:
                            if (element.Value is Dropdown dropdown)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(settingFile, dropdown.value);
                            }
                            break;
                        case bool b:
                            if (element.Value is Toggle toggle)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(settingFile, toggle.isOn);
                            }
                            break;
                        default:
                            Debug.Log($"Unkown field: {field.Name}, Type: {field.FieldType}, Value: {value}");
                            break;
                    }
                }
            }
            storage.OnFileSynced();
        }
        public void SyncUI()
        {
            foreach (var element in UIElements)
            {
                foreach (FieldInfo field in ScanSetting.GetFields())
                {
                    object value = field.GetValue(settingFile);
                    switch (value)
                    {
                        case float f:
                            if (element.Value is Slider slide)
                            {
                                if (field.Name == element.Key)
                                    slide.value = f;
                            }
                            break;
                        case int i:
                            if (element.Value is Dropdown dropdown)
                            {
                                if (field.Name == element.Key)
                                    dropdown.value = i;
                            }
                            break;
                        case bool b:
                            if (element.Value is Toggle toggle)
                            {
                                if (field.Name == element.Key)
                                    toggle.isOn = b;
                            }
                            break;
                        default:
                            Debug.Log($"Unkown field: {field.Name}, Type: {field.FieldType}, Value: {value}");
                            break;
                    }
                }
            }
        }
    }

    public interface ISettingStorage
    {
        bool IsSettingDirty { get; }
        void OnSettingCreated();
        void OnSettingStarted();
        void OnFileSynced();
    }

    public abstract class SettingFile : SaveFile, ISettingStorage
    {
        public ISettingStorage provider => this;
        protected bool IsDirty { get; private set; }


        // Implement ISettingStorage Interface
        public bool IsSettingDirty => IsDirty;
        void ISettingStorage.OnSettingCreated()
        {
            OnSettingCreate();
        }
        void ISettingStorage.OnSettingStarted()
        {
            OnSettingStart();
        }
        void ISettingStorage.OnFileSynced()
        {
            IsDirty = true;
            OnSettingDataUpdated();
        }

        // Protected Method
        protected sealed override void OnSaved()
        {
            IsDirty = false;
            OnSettingSaved();
        }
        protected float NormalMixerVolume(float param)
        {
            return Mathf.InverseLerp(-80f, 0f, param);
        }
        protected float ConvertToMixerVolume(float value)
        {
            return -80 + (value * 80);
        }

        // Abstract Methods
        protected abstract void OnSettingCreate();
        protected abstract void OnSettingStart();
        protected abstract void OnSettingDataUpdated();
        protected abstract void OnSettingSaved();
    }

}