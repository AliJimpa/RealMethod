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
        [Space, SerializeField]
        private bool AutoSync = true;
        [SerializeField]
        private SelectableElement UIElements;
        [Header("Save")]
        [SerializeField]
        private StorageFile<ISettingStorage, SettingFile> Storage;

        public bool isFileDirty => Storage.provider.IsSettingDirty;
        public SaveFile file => Storage.file;
        public bool isInSync { get; private set; } = false;

        // Unity Methods
        private void Awake()
        {
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
        }
        private void OnEnable()
        {
            // Sacan all variable in save file
            Scaning.Scan(Storage.file);
        }
        private void Start()
        {
            Storage.Load(this);
            SyncUI();
        }
        private void OnDisable()
        {
            Scaning.Clean();
        }

        // Public Functions
        public void SyncFile(Selectable element)
        {
            isInSync = true;
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
            isInSync = false;
        }
        public void SyncFile()
        {
            isInSync = true;
            FieldInfo[] fileData = Scaning.GetFields();
            for (int i = 0; i < fileData.Length; i++)
            {
                foreach (var element in UIElements)
                {
                    if (element.Key == fileData[i].Name)
                    {
                        object value = fileData[i].GetValue(Storage.file);
                        switch (value)
                        {
                            case float fl:
                                if (element.Value is Slider slide)
                                {
                                    fileData[i].SetValue(Storage.file, slide.value);
                                }
                                break;
                            case int iin:
                                if (element.Value is Dropdown dropdown)
                                {
                                    fileData[i].SetValue(Storage.file, dropdown.value);
                                }
                                break;
                            case bool bo:
                                if (element.Value is Toggle toggle)
                                {
                                    fileData[i].SetValue(Storage.file, toggle.isOn);
                                }
                                break;
                            default:
                                Debug.Log($"Unkown field: {fileData[i].Name}, Type: {fileData[i].FieldType}, Value: {value}");
                                break;
                        }
                    }
                }
            }
            Storage.provider.OnFileSynced();
            isInSync = false;
        }
        [ContextMenu("Sync_UI")]
        public void SyncUI()
        {
            isInSync = true;
            FieldInfo[] fileData = Scaning.GetFields();
            for (int i = 0; i < fileData.Length; i++)
            {
                foreach (var element in UIElements)
                {
                    if (element.Key == fileData[i].Name)
                    {
                        object value = fileData[i].GetValue(Storage.file);
                        switch (value)
                        {
                            case float fl:
                                if (element.Value is Slider slide)
                                {
                                    slide.value = fl;
                                }
                                break;
                            case int iin:
                                if (element.Value is Dropdown dropdown)
                                {
                                    dropdown.value = iin;
                                }
                                break;
                            case bool bo:
                                if (element.Value is Toggle toggle)
                                {
                                    toggle.isOn = bo;
                                }
                                break;
                            default:
                                Debug.Log($"Unkown field: {fileData[i].Name}, Type: {fileData[i].FieldType}, Value: {value}");
                                break;
                        }
                    }
                }
            }
            isInSync = false;
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
            if (isInSync)
                return;
            SyncFile();
            SendMessage("OnSettingUpdate", SendMessageOptions.DontRequireReceiver);
        }
        private void OnDropdownChanged(int value)
        {
            if (isInSync)
                return;
            SyncFile();
            SendMessage("OnSettingUpdate", SendMessageOptions.DontRequireReceiver);
        }
        private void OnToggleChanged(bool value)
        {
            if (isInSync)
                return;
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