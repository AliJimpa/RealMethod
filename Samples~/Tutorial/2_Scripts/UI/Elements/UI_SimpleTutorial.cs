using UnityEngine;
using UnityEngine.UI;

namespace RealMethod
{
    public sealed class UI_SimpleTutorial : MonoBehaviour, ITutorialMessage
    {
        [Header("Settings")] // to control how the background image size is slightly larger than the text component
        [SerializeField, Tooltip("A fixed amount of height that will be added to the background image in all cases.")]
        private float heightPlus;
        [SerializeField]
        private float widthPlus;
        [Header("Resoureces")]
        [SerializeField]
        private Text tutorialText;
        [SerializeField]
        private RectTransform background;
        [SerializeField]
        private RectTransform wrapper;
        [SerializeField]
        private RectTransform topIcon;
        [SerializeField]
        private RectTransform bottomIcon;
        [SerializeField]
        private RectTransform leftIcon;
        [SerializeField]
        private RectTransform rightIcon;


        private const float buttonOffest = 70f;   // an offset to let the background image still cover the ok button
        private RectTransform textRect;
        private RectTransform MyCanvas;
        private TutorialPlacement MyPlacement;


        // Implement ITutorialMessage Interface
        public event ITutorialMessage.Finish OnFinished;
        public MonoBehaviour GetClass()
        {
            return this;
        }
        void ITutorialMessage.Initiate(Object author, Tutorial owner, TutorialConfig config)
        {
            MyCanvas = owner.GetComponent<RectTransform>();
            tutorialText.text = config.Message;
            textRect = tutorialText.rectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(MyCanvas, config.Position, null, out var localPoint);
            MyPlacement = config.Placement;
            InitializeMessageWithRectTransform(localPoint, config.Placement, config.Offset);
        }
        void ITutorialMessage.SetPosition(Vector3 position, bool isWorld, TutorialPlacement direction, float bufferOffset)
        {
            if (isWorld)
            {
                Vector2 screenPoint = Camera.main.WorldToScreenPoint(position);
                RectTransformUtility.ScreenPointToLocalPointInRectangle(MyCanvas, screenPoint, null, out var localPoint);
                MyPlacement = direction;
                InitializeMessageWithRectTransform(localPoint, direction, bufferOffset);
            }
            else
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(MyCanvas, position, null, out var localPoint);
                MyPlacement = direction;
                InitializeMessageWithRectTransform(localPoint, direction, bufferOffset);
            }
        }

        // Unity Method
        private void Awake()
        {
            background.GetComponent<Button>().onClick.AddListener(CloseMessage);
        }
        private void OnDestroy()
        {
            OnFinished = null; // unregister the event listeners
        }

        // Public Functions
        public void CloseMessage()
        {
            OnFinished?.Invoke();
            Destroy(gameObject);
        }

        // Private Functions
        private void InitializeMessageWithRectTransform(Vector2 localRectPoint, TutorialPlacement locationType, float bufferOffset)
        {
            // Force the text component to update, so we can get the actual size of it for later calculation
            //tutorialText.ForceMeshUpdate();
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRect);

            // update the other size and position
            var centerRectOffset = UpdateLayout(locationType, bufferOffset);
            wrapper.anchoredPosition = localRectPoint - centerRectOffset;
        }
        private Vector2 UpdateLayout(TutorialPlacement locationType, float bufferOffset)
        {
            var result = new Vector2();
            var bgRect = new Vector2(textRect.rect.width + widthPlus,
                textRect.rect.height + heightPlus + buttonOffest);
            background.sizeDelta = bgRect;

            switch (locationType)
            {
                case TutorialPlacement.POINT_TO_TOP:
                    topIcon.gameObject.SetActive(true);
                    topIcon.anchoredPosition = new Vector2(0, bgRect.y / 2 + topIcon.sizeDelta.y / 2 - buttonOffest / 2);
                    result = new Vector2(0, bgRect.y / 2 + topIcon.sizeDelta.y - buttonOffest / 2 + bufferOffset);
                    break;
                case TutorialPlacement.POINT_TO_BOTTOM:
                    bottomIcon.gameObject.SetActive(true);
                    bottomIcon.anchoredPosition = new Vector2(0, -bgRect.y / 2 - bottomIcon.sizeDelta.y / 2 - buttonOffest / 2);
                    result = new Vector2(0, -bgRect.y / 2 - bottomIcon.sizeDelta.y - buttonOffest / 2 - bufferOffset);
                    break;
                case TutorialPlacement.POINT_TO_LEFT:
                    leftIcon.gameObject.SetActive(true);
                    leftIcon.anchoredPosition = new Vector2(-bgRect.x / 2 - leftIcon.sizeDelta.x / 2, -buttonOffest / 2);
                    result = new Vector2(-bgRect.x / 2 - leftIcon.sizeDelta.x - bufferOffset, -buttonOffest / 2);
                    break;
                case TutorialPlacement.POINT_TO_RIGHT:
                    rightIcon.gameObject.SetActive(true);
                    rightIcon.anchoredPosition = new Vector2(bgRect.x / 2 + rightIcon.sizeDelta.x / 2, -buttonOffest / 2);
                    result = new Vector2(bgRect.x / 2 + rightIcon.sizeDelta.x + bufferOffset, -buttonOffest / 2);
                    break;
            }

            return result;
        }


    }
}