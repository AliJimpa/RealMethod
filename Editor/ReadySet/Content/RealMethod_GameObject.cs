using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    class RealMethodGameObject
    {
        [MenuItem("GameObject/RealMethod/PlayerStarter", false, 10)]
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
        [MenuItem("GameObject/RealMethod/World", false, 10)]
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
    }
}