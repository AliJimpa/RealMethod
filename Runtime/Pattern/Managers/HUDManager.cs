using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;


namespace RealMethod
{
    public abstract class HUDManager : MonoBehaviour, IGameManager
    {
        private Dictionary<string, GameObject> Layers = new Dictionary<string, GameObject>();


        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            if (GetComponent<Canvas>() == null)
                Debug.LogError("The HUD Should be placed in a canvas");

            foreach (Transform child in this.transform)
            {
                Layers.Add(child.gameObject.name, child.gameObject);
                IWidget LayerWidget = child.GetComponent<IWidget>();
                if (LayerWidget != null)
                {
                    LayerWidget.InitiateWidget(this);
                }
            }
        }
        public void InitiateService(Service service)
        {
            throw new System.NotImplementedException();
        }



        //Core Methods
        public GameObject AddLayer(GameObject Prefab, string Name)
        {
            GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
            Layers.Add(Name, SpawnedObject);
            return SpawnedObject;
        }
        public IWidget AddWidget(MonoBehaviour Owner, GameObject Prefab, string Name)
        {
            if (Prefab.GetComponent<IWidget>() != null)
            {
                GameObject SpawnedObject = Instantiate(Prefab, transform.position, Quaternion.identity, transform);
                Layers.Add(Name, SpawnedObject);
                IWidget Target = SpawnedObject.GetComponent<IWidget>();
                if (Owner)
                {
                    Target.InitiateWidget(Owner);
                }
                else
                {
                    Target.InitiateWidget(this);
                }
                return Target;
            }
            else
            {
                Debug.LogWarning("Only Prefab that has Widget Class Component");
                return null;
            }
        }
        public bool RemoveLayer(string Name)
        {
            GameObject Target = Layers[Name];
            if (Target)
            {
                Destroy(Target);
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
        public T FindWidget<T>() where T : class
        {
            T Reslut = null;
            foreach (var Lay in Layers)
            {
                IWidget TargetWidget = Lay.Value.GetComponent<IWidget>();
                if (TargetWidget == null)
                    continue;

                if (TargetWidget.GetWidgetClass() is T WidgetClass)
                {
                    Reslut = WidgetClass;
                    break;
                }
            }
            return Reslut;
        }
        public bool ActiveLayer(string Name, bool Enable)
        {
            GameObject TargetLayer = Layers[Name];
            if (TargetLayer)
            {
                TargetLayer.SetActive(Enable);
                return true;
            }
            else
            {
                return false;
            }
        }
        public GameObject CreateLayer(string Name)
        {
            GameObject Result = new GameObject(Name);
            Result.AddComponent<Canvas>();
            Result.AddComponent<CanvasScaler>();
            Result.AddComponent<GraphicRaycaster>();
            Result.transform.SetParent(transform);
            Layers.Add(Name, Result);
            return Result;
        }
        public bool FadeInLayer(string Name, float Duration, bool Force)
        {
            GameObject TargetLayer = Layers[Name];
            if (TargetLayer)
            {
                CanvasGroup CG = TargetLayer.GetComponent<CanvasGroup>();
                if (!CG && Force)
                {
                    CG = TargetLayer.AddComponent<CanvasGroup>();
                }

                if (CG)
                {
                    StartCoroutine(FadeIn(CG, Duration));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public bool FadeOutLayer(string Name, float Duration, bool Force)
        {
            GameObject TargetLayer = Layers[Name];
            if (TargetLayer)
            {
                CanvasGroup CG = TargetLayer.GetComponent<CanvasGroup>();
                if (!CG && Force)
                {
                    CG = TargetLayer.AddComponent<CanvasGroup>();
                }

                if (CG)
                {
                    StartCoroutine(FadeOut(CG, Duration));
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public void SwitchLayer(string LayerA, string LayerB, float Duration)
        {
            FadeInLayer(LayerA, Duration, true);
            FadeOutLayer(LayerB, Duration, true);
        }


        //Enumerator
        private IEnumerator FadeIn(CanvasGroup canvas, float fadeDuration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvas.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);  // Increase alpha over time

                yield return null; // Wait for the next frame
            }
        }
        private IEnumerator FadeOut(CanvasGroup canvas, float fadeDuration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                canvas.alpha = Mathf.Clamp01(1 - elapsedTime / fadeDuration);  // Increase alpha over time

                yield return null; // Wait for the next frame
            }
        }

    }


    public interface IWidget
    {
        public MonoBehaviour GetWidgetClass();
        public void InitiateWidget(Object Owner);
    }
}
