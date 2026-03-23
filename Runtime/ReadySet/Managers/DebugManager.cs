using System;
using UnityEngine;

namespace RealMethod
{
    public interface IButton : IIdentifier
    {
        void Update(string Name, string tooltip = "Empty");
        void Update(Vector2 Offcet);
        event Action OnPressed;
    }

    [System.Serializable]
    public class ButtonData : IDraw, IButton
    {
        private DebugManager MyOwner;
        [SerializeField]
        private Name16 MyName;
        private Action PressedEvent;
        private Vector2 Offcet = Vector2.zero;

        public string Tooltip { get; private set; }


        public ButtonData(Name16 ButtonName, Action callback)
        {
            MyName = ButtonName;
            PressedEvent = callback;
        }

        // Implement IIdentifier Interface
        public Name16 NameID => MyName;
        // Implement IGUIDrawer Interface
        bool IDraw.Start(IGameManager Manager)
        {
            if (Manager.GetManagerClass() is DebugManager target)
            {
                MyOwner = target;
                return true;
            }
            else
            {
                Debug.LogWarning($"LogData can't created the start manager should be {typeof(PrintManager)}");
                return false;
            }
        }
        bool IDraw.CanDraw()
        {
            return true;
        }
        void IDraw.Draw(Vector2 Pivot, int Index)
        {
            if (GUI.Button(new Rect(Pivot.x + Offcet.x, (Pivot.y + Offcet.y) * Index, MyOwner.ButtonSize.x, MyOwner.ButtonSize.y), MyName))
            {
                PressedEvent?.Invoke();
            }

        }
        void IDraw.End()
        {
            PressedEvent = null;
        }
        // Implement IButton Interface
        public event Action OnPressed
        {
            add
            {
                PressedEvent += value;
            }

            remove
            {
                PressedEvent -= value;
            }
        }
        public void Update(string Name, string tooltip = "Empty")
        {
            MyName = Name;
            Tooltip = tooltip;
        }
        public void Update(Vector2 offcet)
        {
            Offcet = offcet;
        }
    }

    [AddComponentMenu("RealMethod/Manager/DebugManager")]
    public class DebugManager : GUIManager<ButtonData>
    {
        [Header("Debug Button")]
        public Vector2 ScrollPosition = Vector2.zero;
        [SerializeField]
        private Vector2 buttonSize = new Vector2(200, 40);
        public Vector2 ButtonSize => buttonSize;
        [SerializeField]
        private float buttonMargin = 20;

        // GUIManager Methods
        public override void InitiateManager(bool AlwaysLoaded)
        {
            Pivot = new Vector2(0, 50);
        }
        public override void ResolveService(Service service, bool active)
        {
        }


        public IButton AddButton(Name16 ButtonName, Action Callback)
        {
            var Result = new ButtonData(ButtonName, Callback);
            ((IDraw)Result).Start(this);
            Add(Result);
            return Result;
        }
        public bool Remove(IButton button)
        {
            var result = Find(button);
            ((IDraw)result).End();
            return DrawList.Remove(result);
        }

        protected override void PreDraw()
        {
            base.PreDraw();
            RectOffset padding = GUI.skin.button.padding;
            RectOffset margin = GUI.skin.button.margin;
            // TODO: The height calculation should be done more correctly.
            Rect viewRect = new Rect(0, 0, buttonSize.x, ((buttonSize.y + (padding.vertical + margin.vertical)) * Count) - buttonSize.y);

            ScrollPosition = GUI.BeginScrollView(
           position: new Rect(Screen.width - buttonSize.x - buttonMargin, 10, buttonSize.x + buttonMargin, Screen.height - 10),
           scrollPosition: ScrollPosition,
           viewRect: viewRect,
           alwaysShowHorizontal: false,
           alwaysShowVertical: false);
        }
        protected override void PostDraw()
        {
            base.PostDraw();
            GUI.EndScrollView();
        }

    }
}