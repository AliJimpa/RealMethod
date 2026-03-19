using UnityEditor;

namespace RealMethod.Editor
{
    class Ability_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/AbilityAsset", false, 80)]
        public static void CreateAbilityAsset()
        {
            string Path = RealMethod.CreateScriptTemplate("AbilityAssetTemplate.txt", "MyAbility.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/AbilityActionAsset", false, 80)]
        public static void CreateAbilityAction()
        {
            string Path = RealMethod.CreateScriptTemplate("AbilityActionAssetTemplate.txt", "MyAbilityAction.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Ability/Effect", false, 80)]
        public static void CreateEffect()
        {
            string Path = RealMethod.CreateScriptTemplate("AbilityEffectTemplate.txt", "MyEffect.cs");
        }
    }
}