using RealMethod.Editor;
using UnityEditor;

namespace RealMethod
{
    class RealMethodPrefab
    {
        [MenuItem("RealMethod/Tutorial", false, 10)]
        public static void AddTutorial()
        {
            RM_Create.Prefab("TutorialMessage.prefab", false);
        }
    }
}