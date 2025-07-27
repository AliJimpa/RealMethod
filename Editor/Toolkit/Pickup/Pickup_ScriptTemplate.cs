using UnityEditor;

namespace RealMethod.Editor
{
    class Pickup_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Pickup/Pickup3D", false, 80)]
        public static void CreatePickup3D()
        {
            string Path = RM_ScriptTemplate.Create("Pickup3DTemplate.txt", "MyPickup.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Pickup/Pickup2D", false, 80)]
        public static void CreatePickup2D()
        {
            string Path = RM_ScriptTemplate.Create("Pickup2DTemplate.txt", "MyPickup.cs");
        }
    }
}