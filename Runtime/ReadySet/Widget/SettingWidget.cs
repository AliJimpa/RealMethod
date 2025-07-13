using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace RealMethod
{
    [Serializable]
    public class SelectableElement : SerializableDictionary<string, Selectable> { }

    [AddComponentMenu("RealMethod/Widgets/SettingWidget")]
    public sealed class SettingWidget : MonoBehaviour, IWidget
    {
        [Header("File")]
        [SerializeField]
        private SettingFile settingfile;
        public FieldContainer ScanSetting;
        [Header("UI")]
        [SerializeField]
        private SelectableElement elements;




        private DataManager saveManager;
        public bool IsSettingDirty { get; private set; } = false;
        private ISettingStorage storage;


        // Implement IWidget Interface
        MonoBehaviour IWidget.GetWidgetClass()
        {
            return this;
        }
        void IWidget.InitiateWidget(UnityEngine.Object Owner)
        {
            if (settingfile == null)
            {
                Debug.LogError($"{this} savefile is not valid ");
                enabled = false;
                return;
            }

            storage = settingfile as ISettingStorage;
            if (storage == null)
            {
                Debug.LogError($"{this} ISettingStorage not implement in this savefile ");
                enabled = false;
                return;
            }
            ScanSetting.Scan(settingfile);
        }


        // Unity Method
        private void Awake()
        {
            if (!ScanSetting.isStore)
            {
                Debug.LogError($"{this} should initiat by UIManager");
                enabled = false;
                return;
            }

            saveManager = Game.FindManager<DataManager>();
            if (saveManager == null)
            {
                Debug.LogError($"{this} Can't find DataManager");
                enabled = false;
                return;
            }

            if (saveManager.IsExistFile(settingfile))
            {
                storage.OnSettingStarted();
                saveManager.LoadFile(settingfile);
            }
            else
            {
                storage.OnSettingCreated();
                SaveSetting();
            }
        }
        private void OnEnable()
        {
            SyncUI();
        }


        // Public Functions
        public Selectable FindElement(string label)
        {
            return elements[label];
        }
        public string FindLabel(Selectable element)
        {
            foreach (var elem in elements)
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
            saveManager.SaveFile(settingfile);
            IsSettingDirty = false;
        }
        public void SyncElement(Selectable element)
        {
            string label = FindLabel(element);
            if (label == string.Empty)
            {
                Debug.LogError("Can't Find this element!");
                return;
            }
            foreach (FieldInfo field in ScanSetting.GetFields())
            {
                object value = field.GetValue(settingfile);
                switch (value)
                {
                    case float f:
                        if (element is Slider slide)
                        {
                            if (field.Name == label)
                                field.SetValue(settingfile, slide.value);
                        }
                        break;
                    case int i:
                        if (element is Dropdown dropdown)
                        {
                            if (field.Name == label)
                                field.SetValue(settingfile, dropdown.value);
                        }
                        break;
                    case bool b:
                        if (element is Toggle toggle)
                        {
                            if (field.Name == label)
                                field.SetValue(settingfile, toggle.isOn);
                        }
                        break;
                    default:
                        Debug.Log($"Unkown field: {field.Name}, Type: {field.FieldType}, Value: {value}");
                        break;
                }
            }
            IsSettingDirty = true;
        }
        public void SyncFile()
        {
            foreach (var element in elements)
            {
                foreach (FieldInfo field in ScanSetting.GetFields())
                {
                    object value = field.GetValue(settingfile);
                    switch (value)
                    {
                        case float f:
                            if (element.Value is Slider slide)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(settingfile, slide.value);
                            }
                            break;
                        case int i:
                            if (element.Value is Dropdown dropdown)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(settingfile, dropdown.value);
                            }
                            break;
                        case bool b:
                            if (element.Value is Toggle toggle)
                            {
                                if (field.Name == element.Key)
                                    field.SetValue(settingfile, toggle.isOn);
                            }
                            break;
                        default:
                            Debug.Log($"Unkown field: {field.Name}, Type: {field.FieldType}, Value: {value}");
                            break;
                    }
                }
            }
            IsSettingDirty = true;
        }
        public void SyncUI()
        {
            foreach (var element in elements)
            {
                foreach (FieldInfo field in ScanSetting.GetFields())
                {
                    object value = field.GetValue(settingfile);
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
        void OnSettingCreated();
        void OnSettingStarted();

    }

    public abstract class SettingFile : SaveFile, ISettingStorage
    {
        // Implement ISettingStorage Interface
        void ISettingStorage.OnSettingCreated()
        {
            OnSettingCreate();
        }
        void ISettingStorage.OnSettingStarted()
        {
            OnSettingStart();
        }

        protected abstract void OnSettingCreate();
        protected abstract void OnSettingStart();
    }

}