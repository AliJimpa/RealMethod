using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    class CoreMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Core;


        ///////////// GameObject \\\\\\\\\\\\
        [MenuItem(RM_Editor.GameObjectMenuItemPath + "PlayerStarter", false, 1)]
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
        [MenuItem(RM_Editor.GameObjectMenuItemPath + "World", false, 1)]
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
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Essential/Game", false, RM_Editor.MenuOrder.Essential_Head)]
        public static void CreateGameClass()
        {
            RM_Editor.CreateScriptTemplate("Game", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Essential/World", false, RM_Editor.MenuOrder.Essential_Body)]
        public static void CreateWorld()
        {
            RM_Editor.CreateScriptTemplate("World", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Essential/GameBridge", false, RM_Editor.MenuOrder.Essential_Body)]
        public static void CreateGameBridgeClass()
        {
            RM_Editor.CreateScriptTemplate("GameBridge", MenuLayer);
        }
        // Managers
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/EmptyManager", false, RM_Editor.MenuOrder.Manager_Head)]
        public static void CreateManager()
        {
            RM_Editor.CreateScriptTemplate("Manager", MenuLayer);
        }
        // Services
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Service/EmptyService", false, RM_Editor.MenuOrder.Service_Head)]
        public static void CreateService()
        {
            RM_Editor.CreateScriptTemplate("Service", MenuLayer);
        }
        // Assets
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Data/EmptyDataAsset", false, RM_Editor.MenuOrder.DataAsset_Head)]
        public static void CreateDataAsset()
        {
            RM_Editor.CreateScriptTemplate("DataAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Clone/EmptyCloneAsset", false, RM_Editor.MenuOrder.CloneAsset_Head)]
        public static void CreateCloneAsset()
        {
            RM_Editor.CreateScriptTemplate("CloneAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Unique/EmptyUniqueAsset", false, RM_Editor.MenuOrder.UniqueAsset_Head)]
        public static void CreateUniqueAsset()
        {
            RM_Editor.CreateScriptTemplate("UniqueAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Config/EmptyConfigAsset", false, RM_Editor.MenuOrder.ConfigAsset_Head)]
        public static void CreateConfigAsset()
        {
            RM_Editor.CreateScriptTemplate("ConfigAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/File/EmptyFileAsset", false, RM_Editor.MenuOrder.FileAsset_Head)]
        public static void CreateFileAsset()
        {
            RM_Editor.CreateScriptTemplate("FileAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Config/Game", false, RM_Editor.MenuOrder.ConfigAsset_Body)]
        public static void CreateGameConfig()
        {
            RM_Editor.CreateScriptTemplate("GameConfig", MenuLayer);
        }
        // Editor
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Editor/CompileRule", false, RM_Editor.MenuOrder.Editor)]
        public static void CreateCompileRule()
        {
            RM_Editor.CreateScriptTemplate("CompileRule", MenuLayer);
        }



    }
}