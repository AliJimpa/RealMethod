using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.UIElements;
using System;


namespace RealMethod
{
    public abstract class UIManager : MonoBehaviour, IGameManager
    {
        public enum UIMethod
        {
            IMGUI,
            uGUI,
            UI_Toolkit
        }

        [Header("UI")]
        [SerializeField] private UIMethod method = UIMethod.uGUI;
        public UIMethod Method => method;

        [SerializeField, ShowInInspectorByEnum("method", 2)]
        private PanelSettings UISetting;

        // Actions 
        public Action<CanvasGroup, bool> OnFadeIn;
        public Action<CanvasGroup, bool> OnFadeOut;

        // Private Variable
        private Hictionary<GameObject> Layers = new Hictionary<GameObject>(5);


        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool alwaysLoaded)
        {
            foreach (Transform child in transform)
            {
                Layers.Add(child.gameObject.name, child.gameObject);
                IWidget LayerWidget = child.GetComponent<IWidget>();
                if (LayerWidget != null)
                {
                    LayerWidget.InitiateWidget(this);
                }
            }

            if (Game.TryFindService("Spawn", out Service target) && target is Spawn SpawnServ)
            {
                SpawnServ.BringManager(this);
            }
        }


        private void OnValidate()
        {
            switch (Method)
            {
                case UIMethod.IMGUI:
                    gameObject.layer = 5;
                    break;
                case UIMethod.uGUI:
                    if (GetComponent<Canvas>() == null)
                    {
                        Debug.LogError("The HUD Should be placed in a canvas");
                        return;
                    }
                    break;
                case UIMethod.UI_Toolkit:
                    gameObject.layer = 5;
                    break;
            }
        }


        //Core Methods
        public GameObject CreateLayer(string Name)
        {
            GameObject Result = new GameObject(Name);
            Result.layer = 5;
            switch (Method)
            {
                case UIMethod.IMGUI:
                    break;
                case UIMethod.uGUI:
                    Result.AddComponent<Canvas>();
                    Result.AddComponent<CanvasScaler>();
                    Result.AddComponent<GraphicRaycaster>();
                    break;
                case UIMethod.UI_Toolkit:
                    Result.AddComponent<UIDocument>();
                    break;
            }
            Result.transform.SetParent(transform);
            Layers.Add(Name, Result);
            return Result;
        }
        public UIDocument CreateLayer(string Name, VisualTreeAsset UIAsset)
        {
            if (Method == UIMethod.UI_Toolkit)
            {
                GameObject EmptyObject = new GameObject(Name);
                EmptyObject.layer = 5;
                EmptyObject.transform.SetParent(transform);
                UIDocument Result = EmptyObject.AddComponent<UIDocument>();
                if (UISetting != null)
                {
                    Result.panelSettings = UISetting;
                }
                else
                {
                    Debug.LogWarning("UISetting is not Valid");
                }
                Result.visualTreeAsset = UIAsset;
                Layers.Add(Name, EmptyObject);
                return Result;
            }
            else
            {
                Debug.LogError("Just use for 'UI_Toolkit' Method");
                return null;
            }
        }
        public GameObject AddLayer(GameObject Prefab, string Name)
        {
            GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
            Layers.Add(Name, SpawnedObject);
            IWidget widget = SpawnedObject.GetComponent<IWidget>();
            if (widget != null)
            {
                widget.InitiateWidget(this);
            }
            return SpawnedObject;
        }
        public GameObject AddLayer(GameObject Prefab, MonoBehaviour Owner, string Name)
        {
            GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
            Layers.Add(Name, SpawnedObject);
            IWidget widget = SpawnedObject.GetComponent<IWidget>();
            if (widget != null)
            {
                if (Owner)
                {
                    widget.InitiateWidget(Owner);
                }
                else
                {
                    widget.InitiateWidget(this);
                    Debug.LogWarning("Owner is null, initiating widget with UIManager as owner.");
                }
            }
            return SpawnedObject;
        }
        public bool RemoveLayer(string Name)
        {
            GameObject Target = Layers[Name];
            if (Target)
            {
                Destroy(Target);
                Layers.Remove(Name);
                return true;
            }
            else
            {
                return false;
            }
        }
        public GameObject FindLayer(string Name)
        {
            return Layers[Name];
        }
        public void SortLayer(string Name, int Order)
        {
            if (Method == UIMethod.UI_Toolkit)
            {
                UIDocument Target = Layers[Name].GetComponent<UIDocument>();
                if (Target)
                {
                    Target.sortingOrder = Order;
                }
                else
                {
                    Debug.LogError("Can't find any layer with this name");
                }
            }
            else
            {
                Transform MyLayer = Layers[Name].GetComponent<Transform>();
                if (MyLayer)
                {
                    MyLayer.SetSiblingIndex(0);
                }
                else
                {
                    Debug.LogError("Can't find any layer with this name");
                }
            }
        }
        public void SortLayer(string Name, bool First = true)
        {
            if (First)
            {
                SortLayer(Name, 0);
            }
            else
            {
                SortLayer(Name, GetComponent<Transform>().childCount - 1);
            }
        }
        public bool ActiveLayer(string Name)
        {
            GameObject TargetLayer = Layers[Name];
            if (TargetLayer)
            {
                TargetLayer.SetActive(true);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeactivateLayer(string Name)
        {
            GameObject TargetLayer = Layers[Name];
            if (TargetLayer)
            {
                TargetLayer.SetActive(false);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool FadeIn(string Name, float Duration)
        {
            if (Method == UIMethod.uGUI)
            {
                GameObject TargetLayer = Layers[Name];
                if (TargetLayer)
                {
                    CanvasGroup CG = TargetLayer.GetComponent<CanvasGroup>();
                    if (CG)
                    {
                        StartCoroutine(FadeIn(CG, Duration));
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Your Layer doesn't have CanvasGroup");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogError("Just use for 'uGUI' Method");
                return false;
            }
        }
        public bool FadeOut(string Name, float Duration)
        {
            if (Method == UIMethod.uGUI)
            {
                GameObject TargetLayer = Layers[Name];
                if (TargetLayer)
                {
                    CanvasGroup CG = TargetLayer.GetComponent<CanvasGroup>();
                    if (CG)
                    {
                        StartCoroutine(FadeOut(CG, Duration));
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning("Your Layer doesn't have CanvasGroup");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                Debug.LogError("Just use for 'uGUI' Method");
                return false;
            }
        }
        public void SwitchLayer(string LayerA, string LayerB, float Duration)
        {
            if (Method == UIMethod.uGUI)
            {
                FadeIn(LayerA, Duration);
                FadeOut(LayerB, Duration);
            }
            else
            {
                Debug.LogError("Just use for 'uGUI' Method");
            }
        }

        public IWidget CreateWidget<T>(string Name, VisualTreeAsset UIAsset, MonoBehaviour Owner) where T : MonoBehaviour
        {
            GameObject EmptyObject = new GameObject(Name);
            EmptyObject.layer = 5;
            EmptyObject.transform.SetParent(transform);
            T WidgetClass = EmptyObject.AddComponent<T>();
            IWidget Widget = WidgetClass.GetComponent<IWidget>();
            if (Widget != null)
            {
                if (Owner)
                {
                    Widget.InitiateWidget(Owner);
                }
                else
                {
                    Widget.InitiateWidget(this);
                    Debug.LogWarning("Owner is null, initiating widget with UIManager as owner.");
                }
            }
            else
            {
                Debug.LogWarning("Widget Class Doesn't have IWidget");
            }
            UIDocument Result = EmptyObject.AddComponent<UIDocument>();
            if (UISetting != null)
            {
                Result.panelSettings = UISetting;
            }
            else
            {
                Debug.LogWarning("UISetting is not Valid");
            }
            Result.visualTreeAsset = UIAsset;
            Layers.Add(Name, EmptyObject);
            return Widget;

        }
        public IWidget CreateWidget<T>(string Name, VisualTreeAsset UIAsset) where T : MonoBehaviour
        {
            GameObject EmptyObject = new GameObject(Name);
            EmptyObject.layer = 5;
            EmptyObject.transform.SetParent(transform);
            T WidgetClass = EmptyObject.AddComponent<T>();
            IWidget Widget = WidgetClass.GetComponent<IWidget>();
            if (Widget != null)
            {
                Widget.InitiateWidget(this);
            }
            else
            {
                Debug.LogWarning("Widget Class Doesn't have IWidget");
            }
            UIDocument Result = EmptyObject.AddComponent<UIDocument>();
            if (UISetting != null)
            {
                Result.panelSettings = UISetting;
            }
            else
            {
                Debug.LogWarning("UISetting is not Valid");
            }
            Result.visualTreeAsset = UIAsset;
            Layers.Add(Name, EmptyObject);
            return Widget;

        }
        public IWidget AddWidget(string Name, GameObject Prefab, MonoBehaviour Owner)
        {
            if (Prefab.GetComponent<IWidget>() != null)
            {
                GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
                Layers.Add(Name, SpawnedObject);
                IWidget widget = SpawnedObject.GetComponent<IWidget>();
                if (Owner)
                {
                    widget.InitiateWidget(Owner);
                }
                else
                {
                    widget.InitiateWidget(this);
                    Debug.LogWarning("Owner is null, initiating widget with UIManager as owner.");
                }
                return widget;
            }
            else
            {
                Debug.LogWarning("Only Prefab that has Widget Class Component");
                return null;
            }
        }
        public IWidget AddWidget(string Name, GameObject Prefab)
        {
            if (Prefab.GetComponent<IWidget>() != null)
            {
                GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
                Layers.Add(Name, SpawnedObject);
                IWidget widget = SpawnedObject.GetComponent<IWidget>();
                widget.InitiateWidget(this);
                return widget;
            }
            else
            {
                Debug.LogWarning("Only Prefab that has Widget Class Component");
                return null;
            }
        }
        public bool FindWidgetByName(string Name, out IWidget TargetWidget)
        {
            IWidget Target = Layers[Name].GetComponent<IWidget>();
            if (Target != null)
            {
                TargetWidget = Target;
                return true;
            }
            else
            {
                TargetWidget = null;
                return false;
            }
        }

        public T FindClassInLayers<T>() where T : class
        {
            T Reslut = null;
            IWidget TargetWidget;
            foreach (var Lay in Layers.GetValues())
            {
                TargetWidget = Lay.GetComponent<IWidget>();
                if (TargetWidget == null)
                {
                    continue;
                }

                if (TargetWidget.GetWidgetClass() is T WidgetClass)
                {
                    Reslut = WidgetClass;
                    break;
                }
                else
                {
                    TargetWidget = null;
                }
            }
            return Reslut;
        }

        // Abstract Methods
        public abstract void InitiateService(Service newService);
        public abstract bool IsMaster();

        //Enumerators
        private IEnumerator FadeIn(CanvasGroup canvas, float fadeDuration)
        {
            float elapsedTime = 0f;
            OnFadeIn?.Invoke(canvas, true);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvas.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);  // Increase alpha over time

                yield return null; // Wait for the next frame
            }
            OnFadeIn?.Invoke(canvas, false);
        }
        private IEnumerator FadeOut(CanvasGroup canvas, float fadeDuration)
        {
            float elapsedTime = 0f;
            OnFadeOut?.Invoke(canvas, true);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvas.alpha = Mathf.Clamp01(1 - elapsedTime / fadeDuration);  // Increase alpha over time

                yield return null; // Wait for the next frame
            }
            OnFadeOut?.Invoke(canvas, false);
        }

    }


    public interface IWidget
    {
        public MonoBehaviour GetWidgetClass();
        public void InitiateWidget(UnityEngine.Object Owner);
    }
}
