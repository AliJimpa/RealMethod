using UnityEngine;
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
            [EnumDescription("IMGUI (Legacy - Editor Only)")]
            IMGUI,
            [EnumDescription("uGUI - Canvas-based UI for runtime")]
            uGUI,
            [EnumDescription("UI Toolkit - Modern retained UI system")]
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



        // Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        void IGameManager.InitiateManager(bool alwaysLoaded)
        {
            foreach (Transform child in transform)
            {
                if (Layers.ContainsKey(child.gameObject.name))
                {
                    Debug.LogError("InitiateManager Failed : Same name Detected in Child");
                    return;
                }
                Layers.Add(child.gameObject.name, child.gameObject);
                IWidget LayerWidget = child.GetComponent<IWidget>();
                if (LayerWidget != null)
                {
                    LayerWidget.InitiateWidget(this);
                }

                switch (method)
                {
                    case UIMethod.uGUI:
                        if (child.GetComponent<Canvas>() == null)
                            Debug.LogWarning($"{child.gameObject.name} should have Canvas in Method {method}");
                        break;
                    case UIMethod.UI_Toolkit:
                        if (child.GetComponent<UIDocument>() == null)
                            Debug.LogWarning($"{child.gameObject.name} should have UIDocument in Method {method}");
                        break;
                }
            }

            InitiateManager(alwaysLoaded);
        }
        void IGameManager.InitiateService(Service service)
        {
            InitiateService(service);
        }


        // Unity Methods
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (gameObject.layer != 5)
                gameObject.layer = 5;
        }
#endif

        //Public Functions
        public bool IsValid(string name)
        {
            return Layers.ContainsKey(name);
        }
        public GameObject GetLayer(string name)
        {
            return Layers[name];
        }
        public GameObject CreateLayer(string name)
        {
            GameObject Result;
            switch (Method)
            {
                case UIMethod.IMGUI:
                    Result = new GameObject(name);
                    break;
                case UIMethod.uGUI:
                    Result = new GameObject(name, new Type[4] { typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup) });
                    break;
                case UIMethod.UI_Toolkit:
                    Result = new GameObject(name, new Type[1] { typeof(UIDocument) });
                    break;
                default:
                    Debug.LogError("Unkown Method!");
                    return null;
            }
            Result.transform.SetParent(transform);
            Result.layer = 5;
            Layers.Add(name, Result);
            return Result;
        }
        public T CreateLayer<T>(string name, MonoBehaviour Owner) where T : MonoBehaviour
        {
            GameObject layer = CreateLayer(name);
            if (layer)
            {
                T TargetClass = layer.AddComponent<T>();
                IWidget widget = TargetClass.GetComponent<IWidget>();
                if (widget != null)
                {
                    if (Owner != null)
                    {
                        widget.InitiateWidget(Owner);
                    }
                    else
                    {
                        widget.InitiateWidget(this);
                        Debug.LogWarning("Owner is null, initiating widget with UIManager as owner.");
                    }
                }
                return TargetClass;
            }
            else
            {
                return null;
            }
        }
        public T CreateLayer<T>(string name) where T : MonoBehaviour
        {
            return CreateLayer<T>(name, this);
        }
        public UIDocument CreateLayer(string name, VisualTreeAsset UIAsset)
        {
            if (Method == UIMethod.UI_Toolkit)
            {
                GameObject layer = CreateLayer(name);
                if (layer)
                {
                    UIDocument doc = layer.GetComponent<UIDocument>();
                    if (UISetting != null)
                    {
                        doc.panelSettings = UISetting;
                    }
                    else
                    {
                        Debug.LogWarning($"[{this}] UISetting is not Valid");
                    }
                    doc.visualTreeAsset = UIAsset;
                    return doc;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                Debug.LogError($"[{this}] Method '{Method}' is not UI_Toolkit. Cannot create UIDocument layer.");
                return null;
            }
        }
        public T CreateLayer<T>(string name, VisualTreeAsset UIAsset, MonoBehaviour Owner) where T : MonoBehaviour
        {
            GameObject layer = CreateLayer(name, UIAsset).gameObject;
            if (layer)
            {
                T TargetClass = layer.AddComponent<T>();
                IWidget widget = TargetClass.GetComponent<IWidget>();
                if (widget != null)
                {
                    if (Owner != null)
                    {
                        widget.InitiateWidget(Owner);
                    }
                    else
                    {
                        widget.InitiateWidget(this);
                        Debug.LogWarning("Owner is null, initiating widget with UIManager as owner.");
                    }
                }
                return TargetClass;
            }
            else
            {
                return null;
            }
        }
        public T CreateLayer<T>(string name, VisualTreeAsset UIAsset) where T : MonoBehaviour
        {
            return CreateLayer<T>(name, UIAsset, this);
        }
        public GameObject AddLayer(string name, UPrefab Prefab, MonoBehaviour Owner)
        {
            GameObject SpawnedObject = Instantiate(Prefab.asset, transform.position, Quaternion.identity, transform);
            SpawnedObject.name = name;
            Layers.Add(name, SpawnedObject);
            IWidget widget = SpawnedObject.GetComponent<IWidget>();
            if (widget != null)
            {
                if (Owner != null)
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
        public GameObject AddLayer(string name, UPrefab Prefab)
        {
            return AddLayer(name, Prefab, this);
        }
        public T AddLayer<T>(string name, UPrefab Prefab, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (!Prefab.HasInterface<IWidget>())
            {
                Debug.LogError($"Prefab should has Widget Class Component");
                return null;
            }

            GameObject SpawnedObject = Instantiate(Prefab.asset, transform.position, Quaternion.identity, transform);
            SpawnedObject.name = name;
            Layers.Add(name, SpawnedObject);
            IWidget widget = SpawnedObject.GetComponent<IWidget>();
            if (widget != null)
            {
                if (Owner != null)
                {
                    widget.InitiateWidget(Owner);
                }
                else
                {
                    widget.InitiateWidget(this);
                    Debug.LogWarning("Owner is null, initiating widget with UIManager as owner.");
                }
                return widget.GetWidgetClass() as T;
            }
            else
            {
                return null;
            }
        }
        public T AddLayer<T>(string name, UPrefab Prefab) where T : MonoBehaviour
        {
            return AddLayer<T>(name, Prefab, this);
        }
        public bool RemoveLayer<T>(T Comp) where T : MonoBehaviour
        {
            GameObject target = Comp.gameObject;
            string layername = Layers.Find(target);
            if (layername != string.Empty)
            {
                return RemoveLayer(layername);
            }
            else
            {
                return false;
            }
        }
        public bool RemoveLayer(string name)
        {
            if (IsValid(name))
            {
                GameObject Target = Layers[name];

                if (Target != null)
                    Destroy(Target);

                Layers.Remove(name);

                return true;
            }

            return false;
        }
        public void SortLayer(string name, int Order)
        {
            if (Method == UIMethod.UI_Toolkit)
            {
                UIDocument Target = Layers[name].GetComponent<UIDocument>();
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
                Transform MyLayer = Layers[name].GetComponent<Transform>();
                if (MyLayer)
                {
                    MyLayer.SetSiblingIndex(Order);
                }
                else
                {
                    Debug.LogError("Can't find any layer with this name");
                }
            }
        }
        public void SortLayer(string name, bool First = true)
        {
            if (First)
            {
                SortLayer(name, 0);
            }
            else
            {
                SortLayer(name, GetComponent<Transform>().childCount - 1);
            }
        }
        public bool EnableLayer(string name)
        {
            if (IsValid(name))
            {
                switch (method)
                {
                    case UIMethod.IMGUI:
                        Layers[name].GetComponent<IWidget>().GetWidgetClass().enabled = true;
                        break;
                    case UIMethod.uGUI:
                        CanvasGroup CG = Layers[name].GetComponent<CanvasGroup>();
                        CG.alpha = 1;
                        CG.interactable = true;
                        CG.blocksRaycasts = true;
                        break;
                    case UIMethod.UI_Toolkit:
                        Layers[name].GetComponent<UIDocument>().enabled = true;
                        break;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DisableLayer(string name)
        {
            if (IsValid(name))
            {
                switch (method)
                {
                    case UIMethod.IMGUI:
                        Layers[name].GetComponent<IWidget>().GetWidgetClass().enabled = false;
                        break;
                    case UIMethod.uGUI:
                        CanvasGroup CG = Layers[name].GetComponent<CanvasGroup>();
                        CG.alpha = 0;
                        CG.interactable = false;
                        CG.blocksRaycasts = false;
                        break;
                    case UIMethod.UI_Toolkit:
                        Layers[name].GetComponent<UIDocument>().enabled = false;
                        break;
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool FadeIn(string name, float Duration)
        {
            if (Method == UIMethod.uGUI)
            {
                GameObject TargetLayer = Layers[name];
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
        public bool FadeIn(string name, float Duration, Action<GameObject> callback)
        {
            if (Method == UIMethod.uGUI)
            {
                GameObject TargetLayer = Layers[name];
                if (TargetLayer)
                {
                    CanvasGroup CG = TargetLayer.GetComponent<CanvasGroup>();
                    if (CG)
                    {
                        StartCoroutine(FadeIn(CG, Duration, callback));
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
        public bool FadeOut(string name, float Duration)
        {
            if (Method == UIMethod.uGUI)
            {
                GameObject TargetLayer = Layers[name];
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
        public bool FadeOut(string name, float Duration, Action<GameObject> callback)
        {
            if (Method == UIMethod.uGUI)
            {
                GameObject TargetLayer = Layers[name];
                if (TargetLayer)
                {
                    CanvasGroup CG = TargetLayer.GetComponent<CanvasGroup>();
                    if (CG)
                    {
                        StartCoroutine(FadeOut(CG, Duration, callback));
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
                FadeOut(LayerA, Duration);
                FadeIn(LayerB, Duration);
            }
            else
            {
                Debug.LogError("Just use for 'uGUI' Method");
            }
        }
        public bool TryFindLayer(string name, out GameObject layer)
        {
            if (IsValid(name))
            {
                layer = Layers[name];
                return true;
            }
            else
            {
                layer = null;
                return false;
            }
        }
        public bool TryFindWidget(string name, out IWidget TargetWidget)
        {
            IWidget Target = Layers[name].GetComponent<IWidget>();
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
        protected abstract void InitiateManager(bool alwaysLoaded);
        protected abstract void InitiateService(Service newService);

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
            EnableLayer(canvas.gameObject.name);
            OnFadeIn?.Invoke(canvas, false);
        }
        private IEnumerator FadeIn(CanvasGroup canvas, float fadeDuration, Action<GameObject> callback)
        {
            float elapsedTime = 0f;
            OnFadeIn?.Invoke(canvas, true);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvas.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);  // Increase alpha over time

                yield return null; // Wait for the next frame
            }
            EnableLayer(canvas.gameObject.name);
            OnFadeIn?.Invoke(canvas, false);
            callback?.Invoke(canvas.gameObject);
        }
        private IEnumerator FadeOut(CanvasGroup canvas, float fadeDuration)
        {
            float elapsedTime = 0f;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
            OnFadeOut?.Invoke(canvas, true);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvas.alpha = Mathf.Clamp01(1 - elapsedTime / fadeDuration);  // Increase alpha over time

                yield return null; // Wait for the next frame
            }
            DisableLayer(canvas.gameObject.name);
            OnFadeOut?.Invoke(canvas, false);
        }
        private IEnumerator FadeOut(CanvasGroup canvas, float fadeDuration, Action<GameObject> callback)
        {
            float elapsedTime = 0f;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
            OnFadeOut?.Invoke(canvas, true);
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvas.alpha = Mathf.Clamp01(1 - elapsedTime / fadeDuration);  // Increase alpha over time

                yield return null; // Wait for the next frame
            }
            DisableLayer(canvas.gameObject.name);
            OnFadeOut?.Invoke(canvas, false);
            callback?.Invoke(canvas.gameObject);
        }

    }


    public interface IWidget
    {
        MonoBehaviour GetWidgetClass();
        void InitiateWidget(UnityEngine.Object Owner);
    }
}
