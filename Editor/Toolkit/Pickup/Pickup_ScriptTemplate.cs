using UnityEditor;

namespace RealMethod.Editor
{
    class Pickup_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Pickup/PickupCustom", false, 80)]
        public static void CreatePickupComponent()
        {
            string Path = ScriptCreator.Create("PickupComponentTemplate.txt", "MyCustomPickup.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Pickup/Pickup3D", false, 80)]
        public static void CreatePickup3DCollider()
        {
            string Path = ScriptCreator.Create("Pickup3DTemplate.txt", "MyPicku3D.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Pickup/Pickup2D", false, 80)]
        public static void CreatePickup2DCollider()
        {
            string Path = ScriptCreator.Create("Pickup2DTemplate.txt", "MyPicku2D.cs");
        }
    }
}