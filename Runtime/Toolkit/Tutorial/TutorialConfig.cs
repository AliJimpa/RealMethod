using UnityEngine;

namespace RealMethod
{
    public enum TutorialPlacement
    {
        POINT_TO_TOP,
        POINT_TO_BOTTOM,
        POINT_TO_LEFT,
        POINT_TO_RIGHT
    }
    public abstract class BaseTutorialConfig : ConfigAsset, ITutorialSpawner
    {
        [Header("Setting")]
        [SerializeField]
        private string label = "T1";
        public string Label => label;
        [SerializeField]
        private UPrefab tutorialPrefab;
        [Header("Tutorial")]
        [SerializeField]
        private string message;
        public string Message => message;
        [SerializeField]
        private Vector3 position;
        public Vector3 Position => position;
        [SerializeField]
        private TutorialPlacement placement;
        public TutorialPlacement Placement => placement;
        [SerializeField]
        private float offset;
        public float Offset => offset;

        ITutorialMessage ITutorialSpawner.InstantiateObject(Transform parent)
        {

            return PostInstantiate(Instantiate(tutorialPrefab.asset, parent).GetComponent<ITutorialMessage>());
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tutorialPrefab.IsValid())
            {
                if (!tutorialPrefab.HasInterface<ITutorialMessage>())
                {
                    Debug.LogError($"The Prefab should have class that implemented {typeof(ITutorialMessage)}");
                }
            }
        }
#endif

        // Abstract Methods
        protected abstract ITutorialMessage PostInstantiate(ITutorialMessage provider);
    }



    [CreateAssetMenu(fileName = "Tutorial", menuName = "RealMethod/Tutorial/Config", order = 1)]
    public class TutorialConfig : BaseTutorialConfig
    {
        // Implement BaseTutorialConfig
        protected override ITutorialMessage PostInstantiate(ITutorialMessage provider)
        {
            return provider;
        }
    }


}