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
            GameObject Result;
            switch (Method)
            {
                case UIMethod.IMGUI:
                    Result = new GameObject(Name);
                    break;
                case UIMethod.uGUI:
                    Result = new GameObject(Name, new Type[3] { typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster) });
                    break;
                case UIMethod.UI_Toolkit:
                    Result = new GameObject(Name, new Type[1] { typeof(UIDocument) });
                    break;
                default:
                    Debug.LogError("Unkown Method!");
                    return null;
            }
            Result.transform.SetParent(transform);
            Result.layer = 5;
            Layers.Add(Name, Result);
            return Result;
        }
        public T CreateLayer<T>(string Name, MonoBehaviour Owner) where T : MonoBehaviour
        {
            GameObject layer = CreateLayer(Name);
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
        public T CreateLayer<T>(string Name) where T : MonoBehaviour
        {
            return CreateLayer<T>(Name, this);
        }
        public UIDocument CreateLayer(string Name, VisualTreeAsset UIAsset)
        {
            if (Method == UIMethod.UI_Toolkit)
            {
                GameObject layer = CreateLayer(Name);
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
        public T CreateLayer<T>(string Name, VisualTreeAsset UIAsset, MonoBehaviour Owner) where T : MonoBehaviour
        {
            GameObject layer = CreateLayer(Name, UIAsset).gameObject;
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
        public T CreateLayer<T>(string Name, VisualTreeAsset UIAsset) where T : MonoBehaviour
        {
            return CreateLayer<T>(Name, UIAsset, this);
        }



        public GameObject AddLayer(string Name, GameObject Prefab, MonoBehaviour Owner)
        {
            GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
            Layers.Add(Name, SpawnedObject);
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
        public GameObject AddLayer(string Name, GameObject Prefab)
        {
            return AddLayer(Name, Prefab, this);
        }
        public T AddLayer<T>(string Name, GameObject Prefab, MonoBehaviour Owner) where T : MonoBehaviour
        {
            if (Prefab.GetComponent<IWidget>() != null)
            {
                Debug.LogError($"Prefab should has Widget Class Component");
                return null;
            }


            GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
            Layers.Add(Name, SpawnedObject);
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
        public T AddLayer<T>(string Name, GameObject Prefab) where T : MonoBehaviour
        {
            return AddLayer<T>(Name, Prefab, this);
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
