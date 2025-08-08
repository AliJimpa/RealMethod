using UnityEditor;

namespace RealMethod.Editor
{
    class Ability_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/AbilityAsset", false, 80)]
        public static void CreateAbilityAsset()
        {
            string Path = RM_Create.Script("InventoryItemTemplate.txt", "MyAbility.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/AbilityEffect", false, 80)]
        public static void CreateAbilityEffect()
        {
            string Path = RM_Create.Script("InventoryItemTemplate.txt", "MyAbilityEffect.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/AbilityAction", false, 80)]
        public static void CreateAbilityAction()
        {
            string Path = RM_Create.Script("InventoryItemTemplate.txt", "MyAbilityAction.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/Effect", false, 80)]
        public static void CreateEffect()
        {
            string Path = RM_Create.Script("InventoryItemTemplate.txt", "MyEffect.cs");
        }
    }
}