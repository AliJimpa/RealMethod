using UnityEditor;

namespace RealMethod.Editor
{
    class Pickup_ScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Toolkit/Pickup/Pickup", false, 80)]
        public static void CreatePickupComponent()
        {
            string Path = ScriptCreator.Create("PickupTemplate .txt", "MyPickup.cs");
        }
    }
}