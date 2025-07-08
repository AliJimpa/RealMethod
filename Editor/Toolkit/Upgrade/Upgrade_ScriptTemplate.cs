using UnityEditor;

namespace RealMethod.Editor
{
    class Upgrade_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Upgrade/Item", false, 80)]
        public static void CreateItem()
        {
            string Path = ScriptCreator.Create("UpgradeItemTemplate.txt", "MyUpgradeItem.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Upgrade/Config", false, 80)]
        public static void CreateConfig()
        {
            string Path = ScriptCreator.Create("UpgradeConfigTemplate.txt", "MyConfig.cs");
        }
    }
}