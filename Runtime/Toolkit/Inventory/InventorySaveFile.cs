using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "InventorySaveFile", menuName = "RealMethod/Inventory/SaveFile", order = 1)]
    public class InventorySaveFile : SaveFile, IInventoryStorage
    {
        [Header("Inventory")]
        [SerializeField, ReadOnly, TextArea]
        protected string Description = "This Save file include IInventoryStorage for store data by Inventory for saving inventory items";
        [SerializeField]
        private bool UsePlayerPrefs = true;
        [Header("Storage")]
        [SerializeField]
        private List<string> ItemsName = new List<string>(3);
        [SerializeField]
        private List<int> ItemsQuantity = new List<int>(3);
        [SerializeField]
        private List<int> ItemsCapacity = new List<int>(3);


        // SaveFile Methods
        protected override void OnStable(DataManager manager)
        {

        }
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
            {
                RM_PlayerPrefs.SetArray("ItemsName", ItemsName.ToArray());
                RM_PlayerPrefs.SetArray("ItemsQuantity", ItemsQuantity.ToArray());
                RM_PlayerPrefs.SetArray("ItemsCapacity", ItemsCapacity.ToArray());
            }
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
            {
                ItemsName = RM_PlayerPrefs.GetArray<string>("ItemsName").ToList();
                ItemsQuantity = RM_PlayerPrefs.GetArray<int>("ItemsQuantity").ToList();
                ItemsCapacity = RM_PlayerPrefs.GetArray<int>("ItemsCapacity").ToList();
            }
        }
        protected override void OnDeleted()
        {
            if (UsePlayerPrefs)
            {
                ItemsName = null;
                ItemsQuantity = null;
                ItemsCapacity = null;
            }
        }


        // Implement IInventorySave Interface
        void IStorage.StorageCreated(Object author)
        {
        }
        void IStorage.StorageLoaded(Object author)
        {
        }
        void IInventoryStorage.CreateItem(InventoryItemProperty item)
        {
            ItemsName.Add(item.Name);
            ItemsQuantity.Add(item.Quantity);
            ItemsCapacity.Add(item.Capacity);
        }
        void IInventoryStorage.DestroyItem(string name)
        {
            int Target = GetIndexItem(name);
            ItemsName.RemoveAt(Target);
            ItemsQuantity.RemoveAt(Target);
            ItemsCapacity.RemoveAt(Target);
        }
        void IInventoryStorage.UpdateQuantity(string name, int amount)
        {
            int Target = GetIndexItem(name);
            if (amount != 0)
            {
                ItemsQuantity[Target] += amount;
            }
            else
            {
                ItemsQuantity[Target] = 0;
            }

        }
        void IInventoryStorage.UpdateCapacity(int value)
        {
            int Target = GetIndexItem(name);
            ItemsCapacity[Target] = value;
        }
        InventoryItemProperty[] IInventoryStorage.GetItems()
        {
            // Load All Assets
            var allItems = Resources.LoadAll<ItemAsset>("Inventory");
            var itemDict = allItems.ToDictionary(item => item.Title, item => item);
            List<InventoryItemProperty> Result = new List<InventoryItemProperty>();
            for (int i = 0; i < ItemsName.Count; i++)
            {
                if (itemDict[ItemsName[i]] != null)
                {
                    Result.Add(new InventoryItemProperty(itemDict[ItemsName[i]], ItemsQuantity[i], ItemsCapacity[i]));
                }
                else
                {
                    Debug.LogError($"Cant Find Item from GameSetting {ItemsName[i]}");
                }
            }
            return Result.ToArray();
        }
        void IInventoryStorage.Clear()
        {
            ItemsName.Clear();
            ItemsQuantity.Clear();
            ItemsCapacity.Clear();
        }

        // Private Functions
        private int GetIndexItem(string name)
        {
            return ItemsName.IndexOf(name);
        }


    }
}