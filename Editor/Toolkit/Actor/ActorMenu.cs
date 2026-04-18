using UnityEditor;

namespace RealMethod.Editor
{
    class ActorMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Toolkit;


        [MenuItem(RM_Editor.ScriptMenuItemPath + "Toolkit/Actor/Act", false, 80)]
        public static void CreateActCommand()
        {
            RM_Editor.CreateScriptTemplate("Act", MenuLayer);
        }
    }
}