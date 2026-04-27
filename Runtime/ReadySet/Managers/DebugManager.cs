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


    public interface IDrawWorkaround : IDraw
    {
        bool Start(IGameManager Manager);
        bool CanDraw();
        void Draw(Vector2 Pivot, int Index);
        void End();
    }

    [System.Serializable]
    public class ButtonData : IDrawWorkaround, IButton
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
        bool IDrawWorkaround.Start(IGameManager Manager)
        {
            if (Manager.GetManagerClass() is DebugManager target)
            {
                MyOwner = target;
                return true;
            }
            else
            {
                Debug.LogWarning($"LogData can't created the start manager should be {typeof(DebugManager)}");
                return false;
            }
        }
        bool IDrawWorkaround.CanDraw()
        {
            return true;
        }
        void IDrawWorkaround.Draw(Vector2 Pivot, int Index)
        {
            if (GUI.Button(new Rect(Pivot.x + Offcet.x, (Pivot.y + Offcet.y) * Index, MyOwner.ButtonSize.x, MyOwner.ButtonSize.y), MyName))
            {
                PressedEvent?.Invoke();
            }

        }
        void IDrawWorkaround.End()
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

        public void Start(IGameManager Manager)
        {
            throw new NotImplementedException();
        }

        public bool CanDraw(int Index)
        {
            throw new NotImplementedException();
        }

        public void Draw(int Index)
        {
            throw new NotImplementedException();
        }
    }

    [AddComponentMenu("RealMethod/Manager/DebugManager")]
    public class DebugManager : MonoBehaviour
    {
        [Header("Debug Button")]
        public Vector2 ScrollPosition = Vector2.zero;
        [SerializeField]
        private Vector2 buttonSize = new Vector2(200, 40);
        public Vector2 ButtonSize => buttonSize;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField]
        private float buttonMargin = 20;
#endif

        // GUIManager Methods
        // public override void InitiateManager(bool AlwaysLoaded)
        // {
        //     Pivot = new Vector2(0, 50);
        // }
        // public override void ResolveService(Service service, bool active)
        // {
        // }


        public IButton AddButton(Name16 ButtonName, Action Callback)
        {
            var Result = new ButtonData(ButtonName, Callback);
            //((IDrawWorkaround)Result).Start(this);
            //Add(Result);
            return Result;
        }
        public bool Remove(IButton button)
        {
           // var result = Find(button);
            //((IDrawWorkaround)result).End();
            return false;//DrawList.Remove(result);
        }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        protected  void PreDraw()
        {
            //base.PreDraw();
            RectOffset padding = GUI.skin.button.padding;
            RectOffset margin = GUI.skin.button.margin;
            // TODO: The height calculation should be done more correctly.
            Rect viewRect = new Rect(0, 0, buttonSize.x, ((buttonSize.y + (padding.vertical + margin.vertical)) * 000) - buttonSize.y);

            ScrollPosition = GUI.BeginScrollView(
           position: new Rect(Screen.width - buttonSize.x - buttonMargin, 10, buttonSize.x + buttonMargin, Screen.height - 10),
           scrollPosition: ScrollPosition,
           viewRect: viewRect,
           alwaysShowHorizontal: false,
           alwaysShowVertical: false);
        }
        protected  void PostDraw()
        {
            //base.PostDraw();
            GUI.EndScrollView();
        }
#endif

    }
}