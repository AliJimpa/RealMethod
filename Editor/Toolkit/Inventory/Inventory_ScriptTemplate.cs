using UnityEditor;

namespace RealMethod.Editor
{
    class Inventory_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Item", false, 80)]
        public static void CreateItem()
        {
            string Path = ScriptCreator.Create("InventoryItemTemplate.txt", "MyItem.cs");
        }
    }
}