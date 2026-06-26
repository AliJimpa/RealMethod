#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace RealMethod
{

    public interface IDrawWindow
    {
        void OnButtonClick(string ButtonName);
        void OnToggleChange(string ToggleName, bool Val);
        Rect GetRect(Rect element, int index);
        GUIStyle GetStyle(int ID);
    }

    public abstract class GUIAsset : UniqueAsset, IDraw, ITask, IDrawWindow
    {
        [Serializable]
        public class MemberVariable : MemberBinding
        {
            public object GetValue()
            {
                if (SelectedMember is System.Reflection.FieldInfo FInfo)
                {
                    return FInfo.GetValue(SelectedComponent);
                }
                else if (SelectedMember is System.Reflection.PropertyInfo PInfo)
                {
                    return PInfo.GetValue(SelectedComponent);
                }
                else
                {
                    Debug.LogWarning($"Member '{SelectedMember.Name}' not found on component type '{SelectedComponent.GetType()}'.");
                    return default;
                }
            }
        }
        public enum BuiltInGUIType
        {
            Label = 0,
            Button = 1,
            RepeatButton = 2,
            Toggle = 3,
            TextField = 4,
            PasswordField = 5,
            TextArea = 6,
            HorizontalSlider = 7,
            HorizontalScrollbar = 8,
            VerticalSlider = 9,
            VerticalScrollbar = 10,
            Box = 11,
            Group_Begin = 12,
            Group_End = 13,
            ScrollView_Begin = 14,
            ScrollView_End = 15,
            SelectionGrid = 16,
            Clip_Begin = 17,
            Clip_End = 18,
            DrawTexture = 19,
            Toolbar = 20,
        }
        [Serializable]
        private class DrawSlot : IDraw
        {
            private enum GUIStyleMode
            {
                Skin = 0,
                Custom_1 = 1,
                Custom_2 = 2,
                Custom_3 = 3,
                None = 4,
            }
            [Header("GUI Info")]
            [Slug]
            public string NameID;
            public BuiltInGUIType GUIType;
            [SerializeField]
            private GUIStyleMode StyleMode;
            public bool Reset = false;
            public Rect Transform = Rect.zero;
            [Header("GUI Setting")]
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.Button, BuiltInGUIType.RepeatButton, BuiltInGUIType.Toggle, BuiltInGUIType.Box)]
            public string Title;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.Label, BuiltInGUIType.TextField, BuiltInGUIType.PasswordField, BuiltInGUIType.TextArea)]
            public string Text;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.Toggle)]
            public bool Toggle;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.HorizontalScrollbar, BuiltInGUIType.HorizontalSlider, BuiltInGUIType.VerticalScrollbar, BuiltInGUIType.VerticalSlider)]
            public float Slider;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.HorizontalSlider, BuiltInGUIType.VerticalSlider)]
            public float SliderMin;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.HorizontalSlider, BuiltInGUIType.VerticalSlider)]
            public float SliderMax;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.HorizontalScrollbar, BuiltInGUIType.VerticalScrollbar)]
            public float ScrollSize;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.ScrollView_Begin)]
            public Vector2 scrollPos;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.ScrollView_Begin)]
            public Vector2 ContentSize;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.SelectionGrid)]
            public int SelectedIndex;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.SelectionGrid, BuiltInGUIType.Toolbar)]
            public string Option;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.SelectionGrid)]
            public int Columns;
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.DrawTexture)]
            public Texture Texture;
            [Header("MemberInfo")]
            [ConditionalShowByEnum("GUIType", BuiltInGUIType.Button, BuiltInGUIType.RepeatButton, BuiltInGUIType.Toggle,
            BuiltInGUIType.Label, BuiltInGUIType.TextField, BuiltInGUIType.PasswordField, BuiltInGUIType.TextArea)]
            public bool UseMember = false;
            [ConditionalHide("UseMember", true, false)]
            public string gameObjectName;
            [ConditionalHide("UseMember", true, false)]
            public SoftType<MonoBehaviour> componentType;
            [ConditionalHide("UseMember", true, false)]
            public string memberName;


            // Private Variable
            private IDrawWindow window;
            public bool IsActive => window != null;
            private Component CachedComponent;


            // Implement IDraw Interface
            bool IDraw.CanDraw(int Index)
            {
                if (!IsActive)
                    return false;

                if (string.IsNullOrEmpty(NameID))
                {
                    Debug.LogWarning($"[{GUIType}]: NameID Should Not be Empty");
                    return false;
                }

                switch (GUIType)
                {
                    // ----------- SIMPLE TEXT ELEMENTS -----------
                    case BuiltInGUIType.Label:
                        return !string.IsNullOrEmpty(Text);

                    case BuiltInGUIType.TextField:
                    case BuiltInGUIType.PasswordField:
                    case BuiltInGUIType.TextArea:
                        // These can draw even if empty text
                        return true;

                    // ----------- BUTTONS & TOGGLES -----------
                    case BuiltInGUIType.Button:
                    case BuiltInGUIType.RepeatButton:
                    case BuiltInGUIType.Toggle:
                    case BuiltInGUIType.Toolbar:
                        return !string.IsNullOrEmpty(Text);

                    // ----------- SLIDERS & SCROLLBARS -----------
                    case BuiltInGUIType.HorizontalSlider:
                    case BuiltInGUIType.VerticalSlider:
                    case BuiltInGUIType.HorizontalScrollbar:
                    case BuiltInGUIType.VerticalScrollbar:
                        return SliderMin < SliderMax;

                    // ----------- CONTAINERS ALWAYS DRAW -----------
                    case BuiltInGUIType.Group_Begin:
                    case BuiltInGUIType.Group_End:
                    case BuiltInGUIType.Clip_Begin:
                    case BuiltInGUIType.Clip_End:
                    case BuiltInGUIType.ScrollView_Begin:
                    case BuiltInGUIType.ScrollView_End:
                        return true;

                    // ----------- SELECTION GRID -----------
                    case BuiltInGUIType.SelectionGrid:
                        //return Options != null && Options.Length > 0;
                        return string.IsNullOrEmpty(Option);

                    // ----------- BOX -----------
                    case BuiltInGUIType.Box:
                        return !string.IsNullOrEmpty(Title);

                    // ----------- DRAW TEXTURE -----------
                    case BuiltInGUIType.DrawTexture:
                        return Texture != null;

                    // ----------- OTHER -----------

                    // ----------- DEFAULT (fallback) -----------
                    default:
                        Debug.LogWarning($"Not Implement for {GUIType}");
                        return false;
                }
            }
            void IDraw.Draw(int Index)
            {
                Rect Position = window.GetRect(Transform, Index);
                GUIStyle Style = GetStyle();

                switch (GUIType)
                {
                    // ---------------- BASIC CONTROLS ----------------
                    case BuiltInGUIType.Label:
                        if (UseMember)
                        {
                            GUI.Label(Position, GetMemberValue().ToString(), Style);
                        }
                        else
                        {
                            GUI.Label(Position, Text, Style);
                        }
                        break;

                    case BuiltInGUIType.Button:
                        if (GUI.Button(Position, Title, Style))
                        {
                            if (UseMember)
                                InvokeMemberMethod();
                            window.OnButtonClick(NameID);
                        }
                        break;

                    case BuiltInGUIType.RepeatButton:
                        if (GUI.RepeatButton(Position, Title, Style))
                        {
                            if (UseMember)
                                InvokeMemberMethod();
                            window.OnButtonClick(NameID);
                        }
                        break;

                    case BuiltInGUIType.Toggle:
                        bool result = GUI.Toggle(Position, Toggle, Title, Style);
                        if (result != Toggle)
                        {
                            if (UseMember)
                                InvokeMemberMethod(result);
                            window.OnToggleChange(NameID, result);
                        }
                        Toggle = result;
                        break;

                    case BuiltInGUIType.TextField:
                        if (UseMember)
                        {
                            Text = GUI.TextField(Position, GetMemberValue().ToString(), Style);
                        }
                        else
                        {
                            Text = GUI.TextField(Position, Text, Style);
                        }
                        break;

                    case BuiltInGUIType.PasswordField:
                        if (UseMember)
                        {
                            Text = GUI.PasswordField(Position, GetMemberValue().ToString(), '*', Style);
                        }
                        else
                        {
                            Text = GUI.PasswordField(Position, Text, '*', Style);
                        }
                        break;

                    case BuiltInGUIType.TextArea:
                        if (UseMember)
                        {
                            Text = GUI.TextArea(Position, GetMemberValue().ToString(), Style);
                        }
                        else
                        {
                            Text = GUI.TextArea(Position, Text, Style);
                        }
                        break;

                    // ---------------- SLIDERS / SCROLLBARS ----------------
                    case BuiltInGUIType.HorizontalSlider:
                        Slider = GUI.HorizontalSlider(Position, Slider, SliderMin, SliderMax);
                        break;

                    case BuiltInGUIType.VerticalSlider:
                        Slider = GUI.VerticalSlider(Position, Slider, SliderMin, SliderMax);
                        break;

                    case BuiltInGUIType.HorizontalScrollbar:
                        Slider = GUI.HorizontalScrollbar(Position, Slider, ScrollSize, SliderMin, SliderMax);
                        break;

                    case BuiltInGUIType.VerticalScrollbar:
                        Slider = GUI.VerticalScrollbar(Position, Slider, ScrollSize, SliderMin, SliderMax);
                        break;

                    // ---------------- CONTAINERS ----------------
                    case BuiltInGUIType.Box:
                        GUI.Box(Position, Title, Style);
                        break;

                    case BuiltInGUIType.Group_Begin:
                        GUI.BeginGroup(Position, Style);
                        break;

                    case BuiltInGUIType.Group_End:
                        GUI.EndGroup();
                        break;

                    case BuiltInGUIType.Clip_Begin:
                        GUI.BeginClip(Position);
                        break;

                    case BuiltInGUIType.Clip_End:
                        GUI.EndClip();
                        break;

                    // ---------------- SCROLL VIEW ----------------
                    case BuiltInGUIType.ScrollView_Begin:
                        scrollPos = GUI.BeginScrollView(
                            Position,
                            scrollPos,
                            new Rect(0, 0, ContentSize.x, ContentSize.y),
                            false,
                            true
                        );
                        break;

                    case BuiltInGUIType.ScrollView_End:
                        GUI.EndScrollView();
                        break;

                    // ---------------- COMPLEX CONTROLS ----------------
                    case BuiltInGUIType.SelectionGrid:
                        SelectedIndex = GUI.SelectionGrid(Position, SelectedIndex, new string[1] { Option }, Columns, Style);
                        break;

                    case BuiltInGUIType.DrawTexture:
                        GUI.DrawTexture(Position, Texture, ScaleMode.ScaleToFit, true);
                        break;

                    case BuiltInGUIType.Toolbar:
                        SelectedIndex = GUI.Toolbar(Position, SelectedIndex, new string[1] { Option }, Style);
                        break;

                    // ---------------- FALLBACK ----------------
                    default:
                        Debug.LogWarning($"GUI Type {GUIType} not implemented.");
                        break;
                }
            }

            // Functions
            public void Active(GUIAsset owner)
            {
                window = owner;
            }
            public void Deactive()
            {
                window = null;
            }
            public void OnValidate(int Index)
            {
                if (string.IsNullOrEmpty(NameID))
                    NameID = Index.ToString();

                if (string.IsNullOrEmpty(memberName))
                    memberName = NameID;

                if ((int)GUIType > 6)
                {
                    UseMember = false;
                }

                if (Reset == true)
                {
                    Reset = false;
                    Transform = GetDefaultRect(GUIType);
                    //Title = string.Empty;
                    //Text = string.Empty;
                    Toggle = false;
                    Slider = 0;
                    SliderMin = 0;
                    SliderMax = 1;
                    ScrollSize = 0.1f;
                    scrollPos = Vector2.zero;
                    ContentSize = new Vector2(500, 500);
                    SelectedIndex = 0;
                    //Option = string.Empty;
                    Columns = 2;
                    //Texture = null;
                }

                if (SliderMax == 0)
                    SliderMax = 1;
                if (ScrollSize == 0)
                    ScrollSize = 0.1f;
                if (ContentSize.x == 0)
                    ContentSize.x = 500;
                if (ContentSize.y == 0)
                    ContentSize.y = 500;
                if (Columns == 0)
                    Columns = 2;
            }

            // Methods
            private GUIStyle GetStyle()
            {
                switch (StyleMode)
                {
                    case GUIStyleMode.Skin:
                        switch (GUIType)
                        {
                            case BuiltInGUIType.Label: return GUI.skin.label;
                            case BuiltInGUIType.Box: return GUI.skin.box;
                            case BuiltInGUIType.Button: return GUI.skin.button;
                            case BuiltInGUIType.Toggle: return GUI.skin.toggle;
                            case BuiltInGUIType.TextField: return GUI.skin.textField;
                            case BuiltInGUIType.TextArea: return GUI.skin.textArea;
                            case BuiltInGUIType.HorizontalSlider: return GUI.skin.horizontalSlider;
                            case BuiltInGUIType.VerticalSlider: return GUI.skin.verticalSlider;
                            case BuiltInGUIType.HorizontalScrollbar: return GUI.skin.horizontalScrollbar;
                            case BuiltInGUIType.VerticalScrollbar: return GUI.skin.verticalScrollbar;
                            case BuiltInGUIType.ScrollView_Begin: return GUI.skin.scrollView;
                            default: return GUI.skin.label;
                        }
                    case GUIStyleMode.Custom_1:
                        return window.GetStyle(1);
                    case GUIStyleMode.Custom_2:
                        return window.GetStyle(2);
                    case GUIStyleMode.Custom_3:
                        return window.GetStyle(3);
                    case GUIStyleMode.None:
                        return new GUIStyle();
                    default:
                        return null;
                }
            }
            private Rect GetDefaultRect(BuiltInGUIType type)
            {
                switch (type)
                {
                    // -------- TEXT / BASIC --------
                    case BuiltInGUIType.Label:
                        return new Rect(10, 20, 120, 20);

                    case BuiltInGUIType.Button:
                    case BuiltInGUIType.RepeatButton:
                    case BuiltInGUIType.Toggle:
                        return new Rect(10, 20, 120, 25);

                    case BuiltInGUIType.TextField:
                    case BuiltInGUIType.PasswordField:
                        return new Rect(10, 20, 160, 22);

                    case BuiltInGUIType.TextArea:
                        return new Rect(10, 20, 200, 80);

                    // -------- SLIDERS --------
                    case BuiltInGUIType.HorizontalSlider:
                        return new Rect(10, 20, 200, 20);

                    case BuiltInGUIType.VerticalSlider:
                        return new Rect(10, 20, 20, 200);

                    // -------- SCROLLBARS --------
                    case BuiltInGUIType.HorizontalScrollbar:
                        return new Rect(10, 20, 200, 18);

                    case BuiltInGUIType.VerticalScrollbar:
                        return new Rect(10, 20, 18, 200);

                    // -------- CONTAINERS --------
                    case BuiltInGUIType.Box:
                        return new Rect(10, 20, 200, 100);

                    case BuiltInGUIType.Group_Begin:
                        return new Rect(10, 20, 300, 200);

                    case BuiltInGUIType.Group_End:
                        return Rect.zero;

                    case BuiltInGUIType.ScrollView_Begin:
                        return new Rect(10, 20, 300, 200);

                    case BuiltInGUIType.ScrollView_End:
                        return Rect.zero;

                    case BuiltInGUIType.Clip_Begin:
                        return new Rect(10, 20, 200, 150);

                    case BuiltInGUIType.Clip_End:
                        return Rect.zero;

                    // -------- COMPLEX CONTROLS --------
                    case BuiltInGUIType.SelectionGrid:
                        return new Rect(10, 20, 250, 100);

                    case BuiltInGUIType.Toolbar:
                        return new Rect(10, 20, 250, 25);

                    // -------- TEXTURE --------
                    case BuiltInGUIType.DrawTexture:
                        return new Rect(10, 20, 128, 128);

                    default:
                        return new Rect(10, 20, 100, 25);
                }
            }
            private Component GetComponent()
            {
                if (CachedComponent != null)
                    return CachedComponent;

                var Target = GameObject.Find(gameObjectName);
                if (Target == null)
                {
                    Debug.LogWarning($"GameObject '{gameObjectName}' not found.");
                    return null;
                }

                CachedComponent = Target.GetComponent(componentType.Type);
                if (CachedComponent == null)
                    Debug.LogWarning($"Component type '{componentType.Type}' not found.");

                return CachedComponent;
            }
            private object GetMemberValue()
            {
                Component SelectedComponent = GetComponent();

                // Get Meember
                Type type = componentType.Type;
                FieldInfo Field = type.GetField(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (Field != null)
                {
                    return Field.GetValue(SelectedComponent);
                }

                PropertyInfo Property = type.GetProperty(memberName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (Property != null)
                {
                    return Property.GetValue(SelectedComponent);
                }

                Debug.LogWarning($"Member '{memberName}' not found on component type '{SelectedComponent.GetType()}'.");
                return null;
            }
            private void InvokeMemberMethod(params object[] arguments)
            {
                Type type = componentType.Type;
                MethodInfo method = type.GetMethod(memberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (method == null)
                {
                    Debug.LogWarning($"Method '{memberName}' not found on '{type}'.");
                    return;
                }

                Component SelectedComponent = GetComponent();
                method.Invoke(SelectedComponent, arguments);
            }
        }


        [Header("Window")]
        [SerializeField]
        private bool WindowsForm = true;
        [SerializeField, ConditionalHide("WindowsForm", true, false)]
        private string Text = "Window";
        [SerializeField, ConditionalHide("WindowsForm", true, false)]
        private Rect Position = new Rect(150, 50, 250, 150);
        [SerializeField, ConditionalHide("WindowsForm", true, false)]
        private bool Dragable = true;
        [SerializeField, ConditionalHide("WindowsForm", true, false)]
        private Rect DragBoundry = new Rect(0, 0, Screen.width, Screen.height);
        [Header("Rendering")]
        [SerializeField]
        private GUIStyleData CustomStyle_1;
        [SerializeField]
        private GUIStyleData CustomStyle_2;
        [SerializeField]
        private GUIStyleData CustomStyle_3;
        [Header("Items")]
        [SerializeField, Tooltip("the value is the amount added per index step")]
        private Vector2 StepOffset = Vector2.zero;
        [SerializeField]
        private DrawSlot[] DrawItems;


        protected object Owner { get; private set; } = null;
        public bool IsActive => Owner != null;


        // Implement ITask Interface
        void ITask.Active(object Instigator)
        {
            Owner = Instigator;
            if (Owner == null)
            {
                Debug.LogWarning($"Owner is not valid for Asset({name})");
                return;
            }

            foreach (var item in DrawItems)
            {
                item.Active(this);
            }
        }
        void ITask.Deactive(object Instigator)
        {
            foreach (var item in DrawItems)
            {
                item.Deactive();
            }
        }
        // Implement IDraw Interface
        bool IDraw.CanDraw(int Index)
        {
            return DrawItems != null && DrawItems.Length > 0;
        }
        void IDraw.Draw(int Index)
        {
            if (WindowsForm)
            {
                Position = GUI.Window(Index, Position, OnWindowsDraw, Text);
            }
            else
            {
                Drawing();
            }
        }
        // Implement IDrawWindow Interface
        public abstract void OnToggleChange(string ToggleName, bool Val);
        public abstract void OnButtonClick(string ButtonName);
        Rect IDrawWindow.GetRect(Rect element, int index)
        {
            if (StepOffset == Vector2.zero)
            {
                return element;
            }
            else
            {
                Vector2 offcet = StepOffset * index;
                return new Rect(element.x + offcet.x, element.y + offcet.y, element.width, element.height);
            }
        }
        GUIStyle IDrawWindow.GetStyle(int ID)
        {
            if (ID == 1)
                return CustomStyle_1.Build();
            if (ID == 2)
                return CustomStyle_2.Build();
            if (ID == 3)
                return CustomStyle_3.Build();

            Debug.LogWarning($"Din't implement any Style for ID: {ID}");
            return null;
        }

        // Unity Method
        private void OnValidate()
        {
            for (int i = 0; i < DrawItems.Length; i++)
            {
                DrawItems[i].OnValidate(i);
            }
        }
        protected virtual void Reset()
        {
            Owner = null;
        }

        // Functions
        public string GetTextItem(string nameID)
        {
            foreach (var item in DrawItems)
            {
                if (item.GUIType == BuiltInGUIType.TextField || item.GUIType == BuiltInGUIType.PasswordField || item.GUIType == BuiltInGUIType.TextArea)
                {
                    if (item.NameID == nameID)
                        return item.Text;
                }
            }
            return string.Empty;
        }
        public void SetTextItem(string nameID, string text)
        {
            foreach (var item in DrawItems)
            {
                if (item.GUIType == BuiltInGUIType.TextField || item.GUIType == BuiltInGUIType.PasswordField || item.GUIType == BuiltInGUIType.TextArea)
                {
                    if (item.NameID == nameID)
                    {
                        item.Text = text;
                        return;
                    }
                }
            }
        }
        public bool HasItem(string nameID)
        {
            foreach (var item in DrawItems)
            {
                if (item.NameID == nameID)
                    return true;
            }
            return false;
        }
        public string[] GetItemList(BuiltInGUIType type)
        {
            List<string> result = new List<string>();
            foreach (var item in DrawItems)
            {
                if (item.GUIType == type)
                {
                    result.Add(item.NameID);
                }
            }
            return result.ToArray();
        }

        // Methods
        private void Drawing()
        {
            for (int i = 0; i < DrawItems.Length; i++)
            {
                IDraw provider = DrawItems[i];
                if (provider.CanDraw(i))
                    provider.Draw(i);
            }
        }
        private void OnWindowsDraw(int id)
        {
            Drawing();
            if (Dragable)
                GUI.DragWindow(DragBoundry);
        }


#if UNITY_EDITOR
        public override bool AutoReset(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                return true;
            return base.AutoReset(state);
        }
#endif
    }


}
#endif