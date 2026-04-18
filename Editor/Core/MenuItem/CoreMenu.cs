using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    class CoreMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Core;


        ///////////// GameObject \\\\\\\\\\\\
        [MenuItem(RM_Editor.GameObjectMenuItemPath + "PlayerStarter", false, 10)]
        static void CreatePlayerStarter(MenuCommand menuCommand)
        {
            // Create a new GameObject
            GameObject go = new GameObject("PlayerStarter");

            // Optional: add components
            go.AddComponent<PlayerStarterComponent>();

            // Place it in the scene, parented if needed
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Register the creation in Undo system (so Ctrl+Z works)
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            // Select the new GameObject
            Selection.activeObject = go;
        }
        [MenuItem(RM_Editor.GameObjectMenuItemPath + "World", false, 10)]
        static void CreateWorld(MenuCommand menuCommand)
        {
            // Create a new GameObject
            GameObject go = new GameObject("World");

            // Optional: add components
            go.AddComponent<DefaultWorld>();

            // Place it in the scene, parented if needed
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);

            // Register the creation in Undo system (so Ctrl+Z works)
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);

            // Select the new GameObject
            Selection.activeObject = go;
        }


        ////////////// Scripts \\\\\\\\\\\\
        // Essentials
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Essential/Game", false, 80)]
        public static void CreateGameClass()
        {
            RM_Editor.CreateScriptTemplate("Game", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Essential/World", false, 80)]
        public static void CreateWorld()
        {
            RM_Editor.CreateScriptTemplate("World", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Essential/GameBridge", false, 80)]
        public static void CreateGameBridgeClass()
        {
            RM_Editor.CreateScriptTemplate("GameBridge", MenuLayer);
        }
        // Managers
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/EmptyManager", false, 80)]
        public static void CreateManager()
        {
            RM_Editor.CreateScriptTemplate("Manager", MenuLayer);
        }
        // Services
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Service/EmptyService", false, 80)]
        public static void CreateService()
        {
            RM_Editor.CreateScriptTemplate("Service", MenuLayer);
        }
        // Assets
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Data/EmptyDataAsset", false, 80)]
        public static void CreateDataAsset()
        {
            RM_Editor.CreateScriptTemplate("DataAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Clone/EmptyCloneAsset", false, 80)]
        public static void CreateCloneAsset()
        {
            RM_Editor.CreateScriptTemplate("CloneAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Unique/EmptyUniqueAsset", false, 80)]
        public static void CreateUniqueAsset()
        {
            RM_Editor.CreateScriptTemplate("UniqueAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Config/EmptyConfigAsset", false, 80)]
        public static void CreateConfigAsset()
        {
            RM_Editor.CreateScriptTemplate("ConfigAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/File/EmptyFileAsset", false, 80)]
        public static void CreateFileAsset()
        {
            RM_Editor.CreateScriptTemplate("FileAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Config/Game", false, 80)]
        public static void CreateGameConfig()
        {
            RM_Editor.CreateScriptTemplate("GameConfig", MenuLayer);
        }
        // Editor
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Editor/CompileRule", false, 80)]
        public static void CreateCompileRule()
        {
            RM_Editor.CreateScriptTemplate("CompileRule", MenuLayer);
        }



    }
}