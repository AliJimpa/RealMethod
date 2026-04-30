#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Linq;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

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
        private Map<GUIAsset, bool> GUIAssets = new Map<GUIAsset, bool>();
        [Header("Setting")]
        [SerializeField]
        private bool isOpen = true;
        public bool IsOpen => isOpen;
        [SerializeField]
        private VisiblityModes ControlMode;
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.UI)]
        private Dir9 ButtonPosition = Dir9.UpRight;
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.UI)]
        private Vector2 Scale = new Vector2(50, 30);
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.UI)]
        private float margin = 0.8f;
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.Input)]
        public InputSystemType inputSystem = InputSystemType.OldInputSystem;
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.Input), ConditionalHideByEnum("inputSystem", InputSystemType.NewInputSystem)]
        public KeyCode oldKey = KeyCode.BackQuote;
#if ENABLE_INPUT_SYSTEM
        [SerializeField, ConditionalShowByEnum("ControlMode", VisiblityModes.Input), ConditionalHideByEnum("inputSystem", InputSystemType.OldInputSystem)]
        public InputActionReference inputAction;
#endif

        public Rect ButtonRect => RM_GUI.GetButtonRect(ButtonPosition, Scale.x, Scale.y, margin);

        // Unity Methods
        private void OnEnable()
        {
            if (ControlMode == VisiblityModes.Input && inputSystem == InputSystemType.NewInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                inputAction.action.Enable();
                inputAction.action.performed += OnUITrigger;
#endif
            }


            foreach (var Asset in GUIAssets)
            {
                ((ITask)Asset.Key).Active();
            }
        }
        void Update()
        {
            if (ControlMode == VisiblityModes.Input && inputSystem == InputSystemType.OldInputSystem)
            {
                if (Input.GetKeyDown(oldKey))
                {
                    TriggerVisiblity();
                }
            }
        }
        private void OnGUI()
        {
            if (ControlMode == VisiblityModes.UI)
            {
                if (GUI.Button(ButtonRect, IsOpen ? "Open" : "Close"))
                {
                    TriggerVisiblity();
                }
            }

            if (IsOpen)
            {
                for (int i = 0; i < GUIAssets.Count; i++)
                {
                    if (GUIAssets.GetValue(i) == false)
                        continue;

                    IDraw task = GUIAssets.GetKey(i);

                    if (task == null)
                    {
                        GUIAssets.Remove(i);
                        continue;
                    }

                    if (task.CanDraw(i))
                        task.Draw(i);
                }
            }
        }
        private void OnDisable()
        {
            if (ControlMode == VisiblityModes.Input && inputSystem == InputSystemType.NewInputSystem)
            {
#if ENABLE_INPUT_SYSTEM
                inputAction.action.performed -= OnUITrigger;
                inputAction.action.Disable();
#endif
            }

            foreach (var Asset in GUIAssets)
            {
                ((ITask)Asset.Key).Deactive();
            }
        }

        // Functions
        public void SetVisisblityEnable(int Index)
        {
            if (Index < 0)
            {
                Debug.LogWarning("Index should not be less than 0 for Change Visisblity");
                return;
            }
            if (GUIAssets.Count > Index)
            {
                var key = GUIAssets.Keys.ElementAt(Index);
                GUIAssets[key] = true;
            }
            else
            {
                Debug.LogWarning($"Can't Find Asset for Index {Index}");
            }
        }
        public void SetVisisblityDisable(int Index)
        {
            if (Index < 0)
            {
                Debug.LogWarning("Index should not be less than 0 for Change Visisblity");
                return;
            }
            if (GUIAssets.Count > Index)
            {
                var key = GUIAssets.Keys.ElementAt(Index);
                GUIAssets[key] = false;
            }
            else
            {
                Debug.LogWarning($"Can't Find Asset for Index {Index}");
            }
        }
        public void SetVisisblityEnable(GUIAsset asset)
        {
            if (asset == null)
            {
                Debug.LogWarning("Asset is not valid for Change Visisblity");
                return;
            }
            if (GUIAssets.ContainsKey(asset))
            {
                GUIAssets[asset] = true;
            }
            else
            {
                Debug.LogWarning($"Can't Find Asset for Index {asset}");
            }
        }
        public void SetVisisblityDisable(GUIAsset asset)
        {
            if (asset == null)
            {
                Debug.LogWarning("Asset is not valid for Change Visisblity");
                return;
            }
            if (GUIAssets.ContainsKey(asset))
            {
                GUIAssets[asset] = false;
            }
            else
            {
                Debug.LogWarning($"Can't Find Asset for Index {asset}");
            }
        }
        public void Add(DrawAsset asset)
        {
            if (asset == null)
            {
                Debug.LogWarning("Can't Add Asset, in not valid!");
                return;
            }
            GUIAssets.Add(asset, true);
            ((ITask)asset).Active();
        }
        public bool Remove(GUIAsset asset)
        {
            for (int i = 0; i < GUIAssets.Count; i++)
            {
                GUIAsset target = GUIAssets.GetKey(i);
                if (target == asset)
                {
                    ((ITask)target).Deactive();
                    return GUIAssets.Remove(target);
                }
            }
            return false;
        }
        public void Open()
        {
            if (ControlMode == VisiblityModes.None)
                return;
            isOpen = true;
        }
        public void Close()
        {
            if (ControlMode == VisiblityModes.None)
                return;
            isOpen = false;
        }
        public void TriggerVisiblity()
        {
            if (IsOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        // Abstract Methods
#if ENABLE_INPUT_SYSTEM
        private void OnUITrigger(InputAction.CallbackContext context)
        {
            TriggerVisiblity();
        }
#endif
        public abstract void InitiateManager(Scope owner);
    }


}
#endif