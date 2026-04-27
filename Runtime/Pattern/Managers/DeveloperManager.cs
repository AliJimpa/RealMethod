#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace RealMethod
{
    public abstract class DeveloperManager : MonoBehaviour, IGameManager
    {
        private enum VisiblityModes
        {
            [DescriptionEnum("WindowsAsset Draw in screen just with OnEnable & OnDisable Component")]
            None,
            [DescriptionEnum("WindowsAsset Draw in screen with Open() & Close() Functions")]
            Manually,
            [DescriptionEnum("WindowsAsset Draw in screen with Open & Close Button in Game")]
            UI,
            [DescriptionEnum("WindowsAsset Draw in screen with [~] Button(Toggle) in Keyboard")]
            Input,
        }
        [Header("Developer")]
        [SerializeField]
        private Map<DrawAsset, bool> WindowsAsset = new Map<DrawAsset, bool>();
        [Header("Setting")]
        [SerializeField]
        private VisiblityModes ControlMode;
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.UI)]
        private Corner ButtonPosition = Corner.UpRight;
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.UI)]
        private Vector2 Scale = new Vector2(50, 30);
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.UI)]
        private float margin = 0.8f;

        public bool IsOpen { get; private set; } = true;
        public Rect ButtonRect => RM_GUI.GetButtonRect(ButtonPosition, Scale.x, Scale.y, margin);



        // Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            InitiateManager(AlwaysLoaded);
        }
        void IGameManager.ResolveService(Service service, bool active)
        {
            ResolveService(service);
        }


        // Unity Methods
        private void OnEnable()
        {
            foreach (var Asset in WindowsAsset)
            {
                ((ITask)Asset.Key).Active();
            }
        }
        private void OnGUI()
        {
            if (IsOpen)
            {
                if (ControlMode == VisiblityModes.UI)
                {
                    if (GUI.Button(ButtonRect, "Close"))
                    {
                        Close();
                    }
                }


                for (int i = 0; i < WindowsAsset.Count; i++)
                {
                    if (WindowsAsset.GetValue(i) == false)
                        continue;

                    IDraw task = WindowsAsset.GetKey(i);

                    if (task == null)
                    {
                        WindowsAsset.Remove(i);
                        continue;
                    }

                    if (task.CanDraw(i))
                        task.Draw(i);
                }
            }
            else
            {
                if (ControlMode == VisiblityModes.UI)
                {
                    if (GUI.Button(ButtonRect, "Open"))
                    {
                        Open();
                    }
                }
            }
        }
        private void OnDisable()
        {
            foreach (var Asset in WindowsAsset)
            {
                ((ITask)Asset.Key).Deactive();
            }
        }

        // Functions
        public void Add(DrawAsset asset)
        {
            if (asset == null)
            {
                Debug.LogWarning("Can't Add Asset, in not valid!");
                return;
            }
            WindowsAsset.Add(asset, true);
            ((ITask)asset).Active();
        }
        public bool Remove(DrawAsset asset)
        {
            for (int i = 0; i < WindowsAsset.Count; i++)
            {
                DrawAsset target = WindowsAsset.GetKey(i);
                if (target == asset)
                {
                    ((ITask)target).Deactive();
                    return WindowsAsset.Remove(target);
                }
            }
            return false;
        }
        [Button]
        public void Open()
        {
            if (ControlMode == VisiblityModes.None)
                return;
            IsOpen = true;
        }
        [Button]
        public void Close()
        {
            if (ControlMode == VisiblityModes.None)
                return;
            IsOpen = false;
        }


        // Abstract Methods
        protected abstract void InitiateManager(bool alwaysLoaded);
        protected abstract void ResolveService(Service service);
    }


}
#endif