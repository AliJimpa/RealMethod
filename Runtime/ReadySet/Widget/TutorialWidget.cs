using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Widgets/TutorialWidget")]
    public sealed class TutorialWidget : MonoBehaviour, IWidget
    {
        [Header("Save")]
        [SerializeField]
        private bool LoadOnAwake = false;
        [SerializeField]
        private bool AutoSave = false;
        [SerializeField]
        private bool UseCustomFile = false;
        [SerializeField, ConditionalHide("UseCustomFile", true, false)]
        private SaveFile saveFile;
        public SaveFile file => saveFile;
        [Header("Events")]
        public UnityEvent OnTutorialShow;

        public System.Action<TutorialMessage> OnShowTutorial;

        private ITutorialStorage storage;

        // Implement IWidget Interface
        MonoBehaviour IWidget.GetWidgetClass()
        {
            return this;
        }
        void IWidget.InitiateWidget(Object Owner)
        {
            storage = GetStorage();
            if (storage == null)
            {
                Debug.LogError("Storage is Not Valid");
                enabled = false;
                return;
            }

            if (GetComponent<Canvas>() == null)
            {
                Debug.LogError("this Widget shoud use in Canvas");
                enabled = false;
                return;
            }
        }

        // Unity Methods
        private void Awake()
        {
            if (LoadOnAwake)
            {
                DataManager savesystem = Game.FindManager<DataManager>();
                if (savesystem != null)
                {
                    if (savesystem.IsExistFile(saveFile))
                    {
                        savesystem.LoadFile(saveFile);
                    }
                }
            }
        }


        // Public Functions
        public bool CanShow(TPrefab prefabs)
        {
            TutorialMessage Messanger = prefabs.GetSoftClassTarget();
            return !storage.IsValidLabel(Messanger.Label);
        }
        public TutorialMessage Show(TPrefab prefab, Object author)
        {
            return Show(prefab, transform, author);
        }
        public TutorialMessage Show(TPrefab prefab, Transform parent, Object author)
        {
            if (!CheckLabel(prefab.GetSoftClassTarget().Label))
            {
                return null;
            }
            GameObject obj = Instantiate(prefab.asset, parent);
            TutorialMessage resutl = obj.GetComponent<TutorialMessage>();
            ((ITutorialInitiator)resutl).Initiate(author, this);
            OnShowUp(resutl, resutl.Label);
            return resutl;
        }
        public TutorialMessage ShowMessage(TPrefab prefab, string newlabel, string title, string message, Object author)
        {
            return ShowMessage(prefab, newlabel, title, message, transform, author);
        }
        public TutorialMessage ShowMessage(TPrefab prefab, string newlabel, string title, string message, Transform parent, Object author)
        {
            if (!CheckLabel(newlabel))
            {
                return null;
            }
            GameObject obj = Instantiate(prefab.asset, parent);
            TutorialMessage resutl = obj.GetComponent<TutorialMessage>();
            ((ITutorialInitiator)resutl).Initiate(author, this);
            ((ITutorialInitiator)resutl).InitiateLabel(newlabel);
            ((ITutorialInitiator)resutl).InitiateMessage(title, message);
            OnShowUp(resutl, newlabel);
            return resutl;
        }
        public TutorialMessage ShowMessage(TPrefab prefab, string newlabel, string title, string message, Sprite icon, Color tint, Object author)
        {
            return ShowMessage(prefab, newlabel, title, message, icon, tint, transform, author);
        }
        public TutorialMessage ShowMessage(TPrefab prefab, string newlabel, string title, string message, Sprite icon, Color tint, Transform parent, Object author)
        {
            if (!CheckLabel(newlabel))
            {
                return null;
            }
            GameObject obj = Instantiate(prefab.asset, parent);
            TutorialMessage resutl = obj.GetComponent<TutorialMessage>();
            ((ITutorialInitiator)resutl).Initiate(author, this);
            ((ITutorialInitiator)resutl).InitiateLabel(newlabel);
            ((ITutorialInitiator)resutl).InitiateMessage(title, message);
            ((ITutorialInitiator)resutl).InitiateDesign(icon, tint);
            OnShowUp(resutl, newlabel);
            return resutl;
        }
        public TutorialMessage Show3D(TPrefab prefab, string newlabel, Vector3 pos, TutorialPlacement direction, Object author)
        {
            return Show3D(prefab, newlabel, pos, direction, 0, transform, author);
        }
        public TutorialMessage Show3D(TPrefab prefab, string newlabel, Vector3 pos, TutorialPlacement direction, float offset, Object author)
        {
            return Show3D(prefab, newlabel, pos, direction, offset, transform, author);
        }
        public TutorialMessage Show3D(TPrefab prefab, string newlabel, Vector3 pos, TutorialPlacement direction, float offset, Transform parent, Object author)
        {
            if (!CheckLabel(newlabel))
            {
                return null;
            }
            GameObject obj = Instantiate(prefab.asset, parent);
            TutorialMessage resutl = obj.GetComponent<TutorialMessage>();
            ((ITutorialInitiator)resutl).Initiate(author, this);
            ((ITutorialInitiator)resutl).InitiateLabel(newlabel);
            ((ITutorialInitiator)resutl).InitiatePosition(pos, true, direction, offset);
            OnShowUp(resutl, newlabel);
            return resutl;
        }
        public TutorialMessage Show3D(TPrefab prefab, string newlabel, string message, Vector3 pos, TutorialPlacement direction, float offset, Object author)
        {
            return Show3D(prefab, newlabel, string.Empty, message, pos, direction, offset, transform, author);
        }
        public TutorialMessage Show3D(TPrefab prefab, string newlabel, string message, Vector3 pos, TutorialPlacement direction, float offset, Transform parent, Object author)
        {
            return Show3D(prefab, newlabel, string.Empty, message, pos, direction, offset, parent, author);
        }
        public TutorialMessage Show3D(TPrefab prefab, string newlabel, string title, string message, Vector3 pos, TutorialPlacement direction, float offset, Transform parent, Object author)
        {
            if (!CheckLabel(newlabel))
            {
                return null;
            }
            GameObject obj = Instantiate(prefab.asset, parent);
            TutorialMessage resutl = obj.GetComponent<TutorialMessage>();
            ((ITutorialInitiator)resutl).Initiate(author, this);
            ((ITutorialInitiator)resutl).InitiateLabel(newlabel);
            ((ITutorialInitiator)resutl).InitiateMessage(title, message);
            ((ITutorialInitiator)resutl).InitiatePosition(pos, true, direction, offset);
            OnShowUp(resutl, newlabel);
            return resutl;
        }
        public TutorialMessage Show2D(TPrefab prefab, string newlabel, Vector3 pos, TutorialPlacement direction, Object author)
        {
            return Show2D(prefab, newlabel, pos, direction, 0, transform, author);
        }
        public TutorialMessage Show2D(TPrefab prefab, string newlabel, Vector3 pos, TutorialPlacement direction, float offset, Object author)
        {
            return Show2D(prefab, newlabel, pos, direction, offset, transform, author);
        }
        public TutorialMessage Show2D(TPrefab prefab, string newlabel, Vector3 pos, TutorialPlacement direction, float offset, Transform parent, Object author)
        {
            if (!CheckLabel(newlabel))
            {
                return null;
            }
            GameObject obj = Instantiate(prefab.asset, parent);
            TutorialMessage resutl = obj.GetComponent<TutorialMessage>();
            ((ITutorialInitiator)resutl).Initiate(author, this);
            ((ITutorialInitiator)resutl).InitiateLabel(newlabel);
            ((ITutorialInitiator)resutl).InitiatePosition(pos, false, direction, offset);
            OnShowUp(resutl, newlabel);
            return resutl;
        }
        public TutorialMessage Show2D(TPrefab prefab, string newlabel, string message, Vector3 pos, TutorialPlacement direction, float offset, Object author)
        {
            return Show2D(prefab, newlabel, string.Empty, message, pos, direction, offset, transform, author);
        }
        public TutorialMessage Show2D(TPrefab prefab, string newlabel, string message, Vector3 pos, TutorialPlacement direction, float offset, Transform parent, Object author)
        {
            return Show2D(prefab, newlabel, string.Empty, message, pos, direction, offset, parent, author);
        }
        public TutorialMessage Show2D(TPrefab prefab, string newlabel, string title, string message, Vector3 pos, TutorialPlacement direction, float offset, Transform parent, Object author)
        {
            if (!CheckLabel(newlabel))
            {
                return null;
            }
            GameObject obj = Instantiate(prefab.asset, parent);
            TutorialMessage resutl = obj.GetComponent<TutorialMessage>();
            ((ITutorialInitiator)resutl).Initiate(author, this);
            ((ITutorialInitiator)resutl).InitiateLabel(newlabel);
            ((ITutorialInitiator)resutl).InitiateMessage(title, message);
            ((ITutorialInitiator)resutl).InitiatePosition(pos, false, direction, offset);
            OnShowUp(resutl, newlabel);
            return resutl;
        }
        public bool Remove(TutorialMessage target)
        {
            if (target != null)
            {
                Destroy(target.gameObject);
                return true;
            }
            return false;
        }
        public void Save()
        {
            DataManager savesystem = Game.FindManager<DataManager>();
            if (savesystem != null)
            {
                savesystem.SaveFile(saveFile);
            }
            else
            {
                Debug.LogWarning("File can't save! didn't find DataManager");
            }
        }

        // Private Functions
        private ITutorialStorage GetStorage()
        {
            if (UseCustomFile)
            {
                if (saveFile is ITutorialStorage newstorage)
                {
                    return newstorage;
                }
                else
                {
                    Debug.LogWarning("IUpgradeStorage Interface not implemented in CustomSavefile.");
                    UseCustomFile = false;
                }

            }

            saveFile = ScriptableObject.CreateInstance<TutorialFile>();
            saveFile.name = "RMTutorialFile";
            return saveFile as ITutorialStorage;
        }
        private bool CheckLabel(string label)
        {
            return !storage.IsValidLabel(label);
        }
        private void OnShowUp(TutorialMessage tutorial, string label)
        {
            OnShowTutorial?.Invoke(tutorial);
            OnTutorialShow?.Invoke();
            storage.NewTutorialShowUP(label);
            if (AutoSave)
                Save();
        }


    }


    // TutorialMessage
    public enum TutorialPlacement
    {
        POINT_TO_TOP,
        POINT_TO_BOTTOM,
        POINT_TO_LEFT,
        POINT_TO_RIGHT
    }
    public interface ITutorialInitiator
    {
        void Initiate(Object author, TutorialWidget owner);
        void InitiateLabel(string newLabel);
        void InitiateMessage(string title, string message);
        void InitiatePosition(Vector3 position, bool isWorld, TutorialPlacement direction, float bufferOffset);
        void InitiateDesign(Sprite image, Color messagecolor);
    }
    public abstract class TutorialMessage : MonoBehaviour, ITutorialInitiator
    {
        [Header("Tutorial")]
        [SerializeField]
        private string label;
        public string Label => label;

        protected GameObject Owner;

        // Implement ITutorialInitiator Interface
        void ITutorialInitiator.Initiate(Object author, TutorialWidget owner)
        {
            if (owner == null)
            {
                Debug.LogError("Owner is not Valid in Initiate");
                return;
            }
            Owner = owner.gameObject;
            OnInitiate(author);
        }
        void ITutorialInitiator.InitiateLabel(string newLabel)
        {
            label = newLabel;
        }
        void ITutorialInitiator.InitiateMessage(string title, string message)
        {
            OnMessage(title, message);
        }
        void ITutorialInitiator.InitiatePosition(Vector3 position, bool isWorld, TutorialPlacement direction, float bufferOffset)
        {
            if (isWorld)
            {
                OnWorldPosition(position, direction, bufferOffset);
            }
            else
            {
                OnScreenPosition(position, direction, bufferOffset);
            }
        }
        void ITutorialInitiator.InitiateDesign(Sprite image, Color messagecolor)
        {
            OnDesign(image, messagecolor);
        }



#if UNITY_EDITOR
        protected void ChangeLabel(string NewName)
        {
            label = NewName;
        }
#endif

        // Abstract Methods
        protected abstract void OnInitiate(Object author);
        protected abstract void OnMessage(string title, string message);
        protected abstract void OnWorldPosition(Vector3 position, TutorialPlacement placement, float Offset);
        protected abstract void OnScreenPosition(Vector3 position, TutorialPlacement placement, float Offset);
        protected abstract void OnDesign(Sprite image, Color tint);
    }

    // Prefab for TutorialMessage
    [System.Serializable]
    public class TPrefab : Prefab<TutorialMessage>
    {

    }


    // TutorialSaveFile
    public interface ITutorialStorage
    {
        void NewTutorialShowUP(string label);
        bool IsValidLabel(string label);
    }
    public class TutorialFile : SaveFile, ITutorialStorage
    {
        [Header("Storage")]
        public List<string> TutorialMessage = new List<string>(5);

        // SaveFile Method
        protected override void OnStable(DataManager manager)
        {
        }
        protected override void OnSaved()
        {
            RM_PlayerPrefs.SetArray("Tutorial", TutorialMessage.ToArray());
        }
        protected override void OnLoaded()
        {
            TutorialMessage = RM_PlayerPrefs.GetArray<string>("Tutorial").ToList();
        }
        protected override void OnDeleted()
        {
        }


        // IMplement ITutorialStorage Interface
        void ITutorialStorage.NewTutorialShowUP(string label)
        {
            TutorialMessage.Add(label);
        }
        bool ITutorialStorage.IsValidLabel(string label)
        {
            return TutorialMessage.Contains(label);
        }
    }




}