using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Toolkit/Interaction/Interactable")]
    public class Interactable : MonoBehaviour
    {
        [Header("Interactable")]
        [SerializeField]
        private bool CheckTag = false;
        [ConditionalHide("CheckTag", true, false)]
        [TagSelector]
        [SerializeField]
        private string TargetTag = "Untagged";

        [Header("Events")]
        [SerializeField]
        private UnityEvent Hover;
        [SerializeField]
        private UnityEvent UnHover;
        [SerializeField]
        private UnityEvent Interact;

        [Header("Debug")]
        [SerializeField]
        private SendMessageOptions CheckMessageSended = SendMessageOptions.DontRequireReceiver;

        private bool Hovering = false;

        public virtual bool CanHover(Interactor Owner)
        {
            bool Result = this.enabled;

            if (CheckTag)
            {
                if (TargetTag != Owner.tag)
                    Result = false;
            }
            return Result;
        }
        public virtual bool CanInteract(Interactor Owner)
        {
            bool Result = this.enabled;

            if (CheckTag)
            {
                if (TargetTag != Owner.tag)
                    Result = false;
            }
            return Result;
        }


        public virtual void Hoverd(Interactor Owner)
        {
            gameObject.SendMessage("OnHover", Owner, CheckMessageSended);
            Hovering = true;
            Hover.Invoke();
        }
        public virtual void UnHoverd(Interactor Owner)
        {
            gameObject.SendMessage("OnUnhover", Owner, CheckMessageSended);
            Hovering = false;
            UnHover.Invoke();
        }
        public virtual void Interacted(Interactor Owner)
        {
            gameObject.SendMessage("OnInteract", Owner, CheckMessageSended);
            Interact.Invoke();
        }

        private void OnDisable()
        {
            if (Hovering)
                UnHoverd(null);
        }

        public bool IsHover()
        {
            return Hovering;
        }

    }
}
