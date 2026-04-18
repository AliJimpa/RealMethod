using UnityEditor;

namespace RealMethod.Editor
{
    class AbilityMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;



        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Ability/AbilityAsset", false, 80)]
        public static void CreateAbilityAsset()
        {
            RM_Editor.CreateScriptTemplate("AbilityAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Ability/AbilityActionAsset", false, 80)]
        public static void CreateAbilityAction()
        {
            RM_Editor.CreateScriptTemplate("AbilityActionAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Ability/Effect", false, 80)]
        public static void CreateEffect()
        {
            RM_Editor.CreateScriptTemplate("AbilityEffect", MenuLayer);
        }
    }
}