using UnityEditor;

namespace RealMethod.Editor
{
    class Upgrade_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Upgrade/ItemAsset", false, 80)]
        public static void CreateItem()
        {
            string Path = RealMethod.CreateScriptTemplate("UpgradeItemTemplate.txt", "MyUpgradeAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Upgrade/MapConfig", false, 80)]
        public static void CreateConfig()
        {
            string Path = RealMethod.CreateScriptTemplate("UpgradeConfigTemplate.txt", "MyUpgradeConfig.cs");
        }
    }
}