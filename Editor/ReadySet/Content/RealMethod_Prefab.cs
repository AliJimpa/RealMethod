using RealMethod.Editor;
using UnityEditor;

namespace RealMethod
{
    class RealMethodPrefab
    {
        [MenuItem("GameObject/Mustard/GamePlay/Player", false, 10)]
        public static void AddPlayer()
        {
            RM_Create.Prefab("TutorialMessage.prefab" , true);
        }
    }
}