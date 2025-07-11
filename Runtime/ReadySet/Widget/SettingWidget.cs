using System;
using UnityEngine;
using UnityEngine.UI;

namespace RealMethod
{
    [Serializable]
    public class SelectableElement : SerializableDictionary<string, Selectable> { }

    public sealed class SettingWidget : MonoBehaviour, IWidget
    {
        [Header("Save")]
        [SerializeField,]
        private SaveFile savefile;
        [Header("UI")]
        [SerializeField]
        private SelectableElement elements;


        private ISettingStorage storage;
        private DataManager saveManager;
        public UIManager HUD { get; private set; }

        // Implement IWidget Interface
        public MonoBehaviour GetWidgetClass()
        {
            return this;
        }
        public void InitiateWidget(UnityEngine.Object Owner)
        {
            if (Owner is UIManager hudmanager)
            {
                HUD = hudmanager;
            }
            else
            {
                Debug.LogError($"{this} should initiate by HUD");
                enabled = false;
                return;
            }

            if (savefile == null)
            {
                Debug.LogError($"{this} savefile is not valid ");
                enabled = false;
                return;
            }

            storage = savefile as ISettingStorage;
            if (storage == null)
            {
                Debug.LogError("ISettingStorage Interface not implemented in CustomSavefile.");
                enabled = false;
                return;
            }

            storage.Initiate(this);
        }


        // Unity Method
        private void Awake()
        {
            saveManager = Game.FindManager<DataManager>();
            if (saveManager == null)
            {
                Debug.LogError($"{this} Can't find DataManager");
                enabled = false;
                return;
            }

            if (saveManager.IsExistFile(savefile))
            {
                LoadSetting();
            }
            else
            {
                SaveSetting();
            }
        }
        private void OnEnable()
        {
            SyncElements();
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
            saveManager.SaveFile(savefile);
        }
        public void LoadSetting()
        {
            saveManager.LoadFile(savefile);
        }
        public void ChangeelementValue(Selectable element)
        {
            string name = FindLabel(element);
            if (elements.ContainsKey(name))
            {
                storage.ChangeValue(name, element);
            }
        }
        public bool IsSettingDirty()
        {
            return storage.IsDirty();
        }

        // Private Functions
        private void SyncElements()
        {
            foreach (var elem in elements)
            {
                storage.Sync(elem.Key, elem.Value);
            }
        }

    }


    public interface ISettingStorage
    {
        void Initiate(SettingWidget owner);
        void Sync(string label, Selectable element);
        void ChangeValue(string label, Selectable element);
        bool IsDirty();
    }


    public abstract class GameSetting : SaveFile, ISettingStorage
    {
        [Header("Audio")]
        [SerializeField, Range(0, 1)]
        protected float musicVolume = 0.7f;
        [SerializeField, Range(0, 1)]
        protected float sfxVolume = 0.7f;
        [Header("Haptic")]
        [SerializeField, Range(0, 1)]
        protected int hasVibration;
        [SerializeField, ConditionalHide("hasVibration", true, false), Range(0, 1)]
        protected float vibrationPower = 0.7f;



        protected SettingWidget setting { get; private set; }
        protected bool isDirty { get; private set; } = false;


        void ISettingStorage.Initiate(SettingWidget owner)
        {
            setting = owner;
        }
        void ISettingStorage.Sync(string label, Selectable element)
        {
            switch (element)
            {
                case Slider slid:
                    OnSyncSlider(label, slid, false);
                    break;
                case Toggle tog:
                    OnSyncToggle(label, tog, false);
                    break;
                case Dropdown dro:
                    OnSyncDropdown(label, dro, false);
                    break;
                default:
                    OnSyncOther(label, element, false);
                    break;
            }
            isDirty = false;
        }
        void ISettingStorage.ChangeValue(string label, Selectable element)
        {
            switch (element)
            {
                case Slider slid:
                    OnSyncSlider(label, slid, true);
                    break;
                case Toggle tog:
                    OnSyncToggle(label, tog, true);
                    break;
                case Dropdown dro:
                    OnSyncDropdown(label, dro, true);
                    break;
                default:
                    OnSyncOther(label, element, true);
                    break;
            }
            isDirty = true;
        }
        bool ISettingStorage.IsDirty()
        {
            return isDirty;
        }

        //Abstract Methods
        protected abstract void OnSyncSlider(string label, Slider element, bool isValueChange);
        protected abstract void OnSyncToggle(string label, Toggle element, bool isValueChange);
        protected abstract void OnSyncDropdown(string label, Dropdown element, bool isValueChange);
        protected abstract void OnSyncOther(string label, Selectable element, bool isValueChange);

    }







}