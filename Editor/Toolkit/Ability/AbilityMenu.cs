using UnityEditor;

namespace RealMethod.Editor
{
    class AbilityMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;



        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Ability/AbilityAsset", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateAbilityAsset()
        {
            RM_Editor.CreateScriptTemplate("AbilityAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Ability/AbilityActionAsset", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateAbilityAction()
        {
            RM_Editor.CreateScriptTemplate("AbilityActionAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Ability/Effect", false, RM_Editor.MenuOrder.Toolkit)]
        public static void CreateEffect()
        {
            RM_Editor.CreateScriptTemplate("AbilityEffect", MenuLayer);
        }
    }
}