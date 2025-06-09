using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RealMethod
{
    public abstract class WidgetToolkit : MonoBehaviour, IWidget
    {
        [Header("Widget")]
        [SerializeField]
        protected string[] PageNames;

        public UIDocument Design { get; private set; }
        public VisualElement Canvas => Design.rootVisualElement;
        protected Hictionary<VisualElement> Pages;


        public VisualElement this[string Name]
        {
            get => Pages[Name];
        }

        //Widget Interface
        public MonoBehaviour GetWidgetClass()
        {
            return this;
        }
        public void InitiateWidget(Object Owner)
        {
            if (!enabled)
                return;

            Design = GetComponent<UIDocument>();

            Pages = new Hictionary<VisualElement>();
            List<VisualElement> allElement = Canvas.Query<VisualElement>().ToList();
            foreach (var ElmName in PageNames)
            {
                bool found = false;
                foreach (var visual in allElement)
                {
                    if (visual.name == ElmName)
                    {
                        Pages.Add(ElmName, visual);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    Debug.LogWarning($"Widget: Could not find VisualElement with name '{ElmName}' in UIDocument '{Design.name}'.");
                }
            }
        }


        // Protected Methods
        protected T GetElement<T>(string Name) where T : VisualElement
        {
            return Canvas.Q<T>(Name);
        }
        protected T[] GetElements<T>(string Name) where T : VisualElement
        {
            List<T> Result = Canvas.Query<T>(Name).ToList();
            return Result.ToArray();
        }
        protected bool ButtonClicked(string ButtonName, System.Action Function)
        {
            // Query the button by its name (set in UXML)
            Button TargetQuery = GetElement<Button>(ButtonName);
            if (TargetQuery != null)
            {
                TargetQuery.clicked += Function;
                return true;
            }
            return false;
        }
        protected VisualElement PageNumber(int Index)
        {
            return Pages[PageNames[Index]];
        }
    }
}