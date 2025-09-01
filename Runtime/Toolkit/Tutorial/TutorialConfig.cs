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
    public abstract class TutorialConfigCore : ConfigAsset, ITutorialSpawner
    {
        [Header("Base")]
        [SerializeField]
        private string label = "T1";
        public string Label => label;
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

        // Implement ITutorialSpawner Interface
        ITutorialMessage ITutorialSpawner.InstantiateMessage(Transform parent)
        {
            var result = InstantiateMessageObject(parent);
            return result.GetComponent<ITutorialMessage>();
        }

        // Abstract Methods
        protected abstract UI_Tutorial InstantiateMessageObject(Transform parent);
    }


    [CreateAssetMenu(fileName = "Tutorial", menuName = "RealMethod/Tutorial/Config", order = 1)]
    public class TutorialConfig : TutorialConfigCore
    {
        [Header("Setting")]
        [SerializeField]
        protected UPrefab tutorialPrefab;

        // Implement TutorialConfigCore
        protected override UI_Tutorial InstantiateMessageObject(Transform parent)
        {
            return Instantiate(tutorialPrefab.asset, parent).GetComponent<UI_Tutorial>();
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
    }


}