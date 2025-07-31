using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace RealMethod
{
    [Serializable]
    public class SelectableElement : SerializableDictionary<string, Selectable> { }
    public interface ISettingStorage : IStorage
    {
        bool IsSettingDirty { get; }
        void OnFileSynced();
    }

    [AddComponentMenu("RealMethod/UI/GameSetting")]
    public sealed class UI_GameSetting : MonoBehaviour
    {
        [Header("Setting")]
        [SerializeField]
        private FieldContainer Scaning;
        [SerializeField]
        private bool AutoSync = true;
        [SerializeField]
        private SelectableElement UIElements;
        [Header("Save")]
        [SerializeField]
        private StorageFile<ISettingStorage, SettingFile> Storage;

        public bool isFileDirty => Storage.provider.IsSettingDirty;
        public SaveFile file => Storage.file;

        // Unity Methods
        private void Start()
        {
            // Sacan all variable in save file & check
            Scaning.Scan(Storage.file);

            // Binding all Element
            if (AutoSync)
            {
                foreach (var element in UIElements)
                {
                    switch (element.Value)
                    {
                        case Slider slide:
                            slide.onValueChanged.AddListener(OnSlideChanged);
                            break;
                        case Dropdown dropdown:
                            dropdown.onValueChanged.AddListener(OnDropdownChanged);
                            break;
                        case Toggle toggle:
                            toggle.onValueChanged.AddListener(OnToggleChanged);
                            break;
                        default:
                            Debug.LogWarning($"Undefined Selector {element.Value}");
                            break;
                    }
                }
            }

            Storage.Load(this);
            SyncUI();
        }
        private void OnEnable()
        {
            if (Scaning.isStore)
                SyncUI();
        }

        // Public Functions
        public void SyncFile(Selectable element)
        {
            string label = FindLabel(element);
            if (label == string.Empty)
            {
                Debug.LogError("Can't Find this element!");
                return;
            }
            foreach (FieldInfo field in Scaning.GetFields())
            {
                object value = field.GetValue(Storage.file);
                switch (value)
                {
                    case float f:
                        if (element is Slider slide)
                        {
                            if (field.Name == label)
                                field.SetValue(Storage.file, slide.value);
                        }
                        break;
                    case int i:
                        if (element is Dropdown dropdown)
                        {
                            if (field.Name == label)
                                field.SetValue(Storage.file, dropdown.value);
                        }
                        break;
                    case bool b:
                        if (element is Toggle toggle)
                        {
                            if (field.Name == label)
                                field.SetValue(Storage.file, toggle.isOn);
                        }
                        break;
                    default:
                        Debug.Log($"Unkown field: {field.Name}, Type: {field.FieldType}, Value: {value}");
                        break;
                }
            }
            Storage.provider.OnFileSynced();
        }
        public void SyncFile()
        {
            foreach (var element in UIElements)
            {
                foreach (FieldInfo field in Scaning.GetFields())
                {
                    object value = field.GetValue(Storage.file);
                    switch (value)
                    {
                        case float f:
                            if (element.Value is Slider slide)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(Storage.file, slide.value);
                            }
                            break;
                        case int i:
                            if (element.Value is Dropdown dropdown)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(Storage.file, dropdown.value);
                            }
                            break;
                        case bool b:
                            if (element.Value is Toggle toggle)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(Storage.file, toggle.isOn);
                            }
                            break;
                        default:
                            Debug.Log($"Unkown field: {field.Name}, Type: {field.FieldType}, Value: {value}");
                            break;
                    }
                }
            }
            Storage.provider.OnFileSynced();
        }
        [ContextMenu("Sync_UI")]
        public void SyncUI()
        {
            foreach (var element in UIElements)
            {
                foreach (FieldInfo field in Scaning.GetFields())
                {
                    object value = field.GetValue(Storage.file);
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
        public void ResetFile()
        {
            Storage.Clear();
            SyncUI();
        }

        // Private Functions
        private void OnSlideChanged(float value)
        {
            SyncFile();
            SendMessage("OnSettingUpdate", SendMessageOptions.DontRequireReceiver);
        }
        private void OnDropdownChanged(int value)
        {
            SyncFile();
            SendMessage("OnSettingUpdate", SendMessageOptions.DontRequireReceiver);
        }
        private void OnToggleChanged(bool value)
        {
            SyncFile();
            SendMessage("OnSettingUpdate", SendMessageOptions.DontRequireReceiver);
        }
    }

    public abstract class SettingFile : SaveFile, ISettingStorage
    {
        protected bool IsDirty { get; private set; }

        // Implement ISettingStorage Interface
        public bool IsSettingDirty => IsDirty;
        void IStorage.StorageCreated(UnityEngine.Object author)
        {
            SetupSettingData();
        }
        void IStorage.StorageLoaded(UnityEngine.Object author)
        {
            ApplySettingData();
        }
        void ISettingStorage.OnFileSynced()
        {
            IsDirty = true;
            ApplySettingData();
        }
        void IStorage.StorageClear()
        {
            OnResetSetting();
        }

        // Protected Method
        protected sealed override void OnSaved()
        {
            IsDirty = false;
            OnSettingFileSaved();
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
        protected abstract void SetupSettingData();
        protected abstract void ApplySettingData();
        protected abstract void OnSettingFileSaved();
        protected abstract void OnResetSetting();


    }

}