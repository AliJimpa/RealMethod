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
    public abstract class TutorialConfigCore : ConfigAsset, IItem, ITutorialSpawner
    {
        [Header("Config")]
        [SerializeField]
        private string label = "T1";
        [Header("Tutorial")]
        [SerializeField]
        private string title;
        public string Title => title;
        [SerializeField, TextArea]
        private string message;
        public string Message => message;
        [SerializeField]
        private Texture2D icon;
        [SerializeField]
        private Vector3 startPosition;
        public Vector3 StartPosition => startPosition;
        [SerializeField]
        private TutorialPlacement placement;
        public TutorialPlacement Placement => placement;
        [SerializeField]
        private float offset;
        public float Offset => offset;


        // Implement IIdentifier Interface
        public string NameID => label;
        // Implement IItem Interface
        public Texture2D Icon => icon;
        public Sprite GetSpriteIcon()
        {
            return GetSpriteIcon(new Rect(0, 0, icon.width, icon.height),
                   new Vector2(0.5f, 0.5f)
               );
        }
        public Sprite GetSpriteIcon(Rect rect, Vector2 pivot)
        {
            if (icon != null)
            {
                return Sprite.Create(icon, rect, pivot);
            }
            else
            {
                Debug.LogWarning("ItemAsset: Icon is not assigned for item '" + label + "'.");
                return null;
            }
        }
        // Implement IItemData Interface
        public PrimitiveAsset GetAsset()
        {
            return this;
        }
        // Implement ITutorialSpawner Interface
        ITutorialMessage ITutorialSpawner.InstantiateMessage(Transform parent)
        {
            var result = InstantiateMessageObject(parent);
            return result.GetComponent<ITutorialMessage>();
        }


        // Abstract Methods
        protected abstract UI_TutorialUnit InstantiateMessageObject(Transform parent);

#if UNITY_EDITOR
        void IItem.ChangeItemName(string newLabel)
        {
            label = newLabel;
        }
#endif
    }


    [CreateAssetMenu(fileName = "Tutorial", menuName = "RealMethod/Tutorial/Config", order = 1)]
    public class TutorialConfig : TutorialConfigCore
    {
        [Header("Setting")]
        [SerializeField]
        protected UPrefab tutorialPrefab;

        // Implement TutorialConfigCore
        protected override UI_TutorialUnit InstantiateMessageObject(Transform parent)
        {
            return Instantiate(tutorialPrefab.asset, parent).GetComponent<UI_TutorialUnit>();
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