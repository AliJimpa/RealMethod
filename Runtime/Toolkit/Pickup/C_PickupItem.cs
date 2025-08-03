using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/PickupItem")]
    public sealed class C_PickupItem : MonoCommand
    {
        [Header("Setting")]
        [SerializeField]
        private int quantity;
        [SerializeField]
        private bool AsNewIdentity = false;
        [SerializeField, ConditionalHide("AsNewIdentity", true, false)]
        private int capacity;
        [Header("Resource")]
        [SerializeField]
        private DataAsset items;

        private IInventoryItem itemprovider;

        // ExecutCommand Methods
        protected override bool OnInitiate(Object author, Object owner)
        {
            if (items == null)
            {
                Debug.LogWarning("Items is not valid");
                return false;
            }

            if (items is IInventoryItem itemprovider)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected override bool CanExecute(object Owner)
        {
            if (!enabled)
                return false;

            if (Owner is Component mono)
            {
                IPicker Picker = mono.GetComponent<IPicker>();
                if (Picker != null)
                {
                    Trigger mytrigger = GetComponent<Trigger>();
                    if (mytrigger)
                    {
                        return Picker.CanTake(mytrigger);
                    }
                    else
                    {
                        Debug.LogWarning("this class need Trigger behind self");
                        return false;
                    }

                }
                else
                {
                    return true;
                }
            }
            else
            {
                Debug.LogWarning("Executer should be Component");
                return false;
            }
        }
        protected override void Execute(object Owner)
        {
            if (Owner is Component mono)
            {
                Inventory inven = mono.GetComponent<Inventory>();
                if (inven)
                {
                    if (AsNewIdentity)
                    {
                        inven.AddNewItem(itemprovider, quantity, capacity);
                    }
                    else
                    {
                        inven.AddItem(itemprovider, quantity);
                    }
                }
                else
                {
                    Debug.LogWarning("Executer shoudl have Inventory Component");
                }
            }
            else
            {
                Debug.LogWarning("Executer should be Component");
            }
        }

    }
}