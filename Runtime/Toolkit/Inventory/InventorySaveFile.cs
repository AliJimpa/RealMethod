using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "InventorySaveFile", menuName = "RealMethod/Inventory/SaveFile", order = 1)]
    public class InventorySaveFile : SaveFile, IInventorySave
    {
        [Header("Inventory")]
        [SerializeField]
        private bool UsePlayerPrefs = true;
        [SerializeField]
        private string[] InventoryAssets;
        [SerializeField]
        private int[] InventoryQuantity;


        // SaveFile Methods
        protected override void OnStable(DataManager manager)
        {

        }
        protected override void OnSaved()
        {
            if (UsePlayerPrefs)
            {
                RM_PlayerPrefs.SetArray("PlayerItemsName", InventoryAssets);
                RM_PlayerPrefs.SetArray("PlayerItemsQuantity", InventoryQuantity);
                RM_PlayerPrefs.SetBool("Playerinventory", true);
            }
        }
        protected override void OnLoaded()
        {
            if (UsePlayerPrefs)
            {
                InventoryAssets = RM_PlayerPrefs.GetArray<string>("PlayerItemsName");
                InventoryQuantity = RM_PlayerPrefs.GetArray<int>("PlayerItemsQuantity");
            }
        }
        protected override void OnDeleted()
        {
            PlayerPrefs.DeleteKey("Playerinventory");
        }




        // Implement IInventorySave Interface
        public InventoryItemProperty[] ReadInventoryData(Inventory owner)
        {
            var allItems = Resources.LoadAll<ItemAsset>("");
            var itemDict = allItems.ToDictionary(item => item.name, item => item);
            List<InventoryItemProperty> Result = new List<InventoryItemProperty>();
            for (int i = 0; i < InventoryAssets.Length; i++)
            {
                if (itemDict[InventoryAssets[i]] != null)
                {
                    Result.Add(new InventoryItemProperty(itemDict[InventoryAssets[i]], InventoryQuantity[i]));
                }
                else
                {
                    Debug.LogError($"Cant Find Item from GameSetting {InventoryAssets[i]}");
                }
            }
            return Result.ToArray();
        }
        public void WriteInventoryData(Inventory owner, InventoryItemProperty[] Data)
        {
            Dictionary<string, int> Result = new Dictionary<string, int>();
            foreach (var property in Data)
            {
                Result.Add(property.Asset.name, property.Quantity);
            }
            InventoryAssets = new List<string>(Result.Keys).ToArray();
            InventoryQuantity = new List<int>(Result.Values).ToArray();
        }
        public bool IsExistInventoryData(Inventory owner)
        {
            return PlayerPrefs.HasKey("Playerinventory");
        }

    }
}