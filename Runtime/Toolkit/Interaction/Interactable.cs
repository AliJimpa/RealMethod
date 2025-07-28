using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Interaction/Interactable")]
    public class Interactable : MonoBehaviour, IInteraction
    {
        [Header("Setting")]
        [SerializeField]
        private bool Once = false;
        [SerializeField]
        private bool CheckTag = false;
        [SerializeField, ConditionalHide("CheckTag", true, false), TagSelector]
        private string TargetTag = "Untagged";
        [SerializeField]
        private bool Message = false;
        [SerializeField, ConditionalHide("Message", true, false)]
        private SendMessageOptions CheckMessageSended = SendMessageOptions.DontRequireReceiver;
        [SerializeField]
        private bool Events = true;
        [Header("Events")]
        [SerializeField, ConditionalHide("Events", true, false)]
        private UnityEvent<bool> OnHover;
        [SerializeField, ConditionalHide("Events", true, false)]
        private UnityEvent<GameObject> OnInteract;

        private bool Hovering = false;

        // Implement IInteraction Interface
        bool IInteraction.IsHover => Hovering;
        bool IInteraction.CanHover(Interactor Owner)
        {
            bool Result = enabled;
            if (CheckTag)
            {
                if (TargetTag != Owner.tag)
                    Result = false;
            }
            return Result;
        }
        void IInteraction.OnHover(Interactor Owner)
        {
            Hovering = true;
            if (Events)
                OnHover?.Invoke(Hovering);
            if (Message)
                gameObject.SendMessage("OnHover", Owner, CheckMessageSended);
        }
        void IInteraction.OnUnhover(Interactor Owner)
        {
            Hovering = false;
            if (Events)
                OnHover?.Invoke(Hovering);
            if (Message)
                gameObject.SendMessage("OnUnhover", Owner, CheckMessageSended);
        }
        bool IInteraction.CanInteract(Interactor Owner)
        {
            bool Result = enabled;
            if (CheckTag)
            {
                if (TargetTag != Owner.tag)
                    Result = false;
            }
            return Result;
        }
        void IInteraction.OnInteract(Interactor Owner)
        {
            if (Once)
                enabled = false;
            if (Events)
                OnInteract?.Invoke(Owner.gameObject);
            if (Message)
                gameObject.SendMessage("OnInteract", Owner, CheckMessageSended);
        }

        // Unity Methods
        private void OnDisable()
        {
            if (Hovering)
                ((IInteraction)this).OnUnhover(null);
        }

    }
}
