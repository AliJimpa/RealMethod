using UnityEditor;

namespace RealMethod.Editor
{
    class RPG_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/RPG/StatDefinition", false, 80)]
        public static void CreateStatDefinition()
        {
            string Path = RM_Create.Script("StatDefinitionTemplate.txt", "StatDefinition.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/RPG/StatProfile", false, 80)]
        public static void CreateStatProfile()
        {
            string Path = RM_Create.Script("StatProfileTemplate.txt", "MyProfile.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/RPG/StatBuff", false, 80)]
        public static void CreateBuffConfig()
        {
            string Path = RM_Create.Script("BuffConfigTemplate.txt", "MyBuff.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/RPG/ResourceData", false, 80)]
        public static void CreateResourceData()
        {
            string Path = RM_Create.Script("ResourceDataTemplate.txt", "MyResource.cs");
        }

    }
}