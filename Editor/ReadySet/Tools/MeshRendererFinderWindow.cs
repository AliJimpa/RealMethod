using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace RealMethod
{
    public class MeshRendererFinderWindow : EditorWindow
    {
        private List<GameObject> objectsWithMeshRenderer = new List<GameObject>();

        [MenuItem("Tools/RealMethod/General/MeshRendererFinder")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MeshRendererFinderWindow));
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Game Objects with MeshRenderer:");

            if (GUILayout.Button("Find Objects"))
            {
                FindObjectsWithMeshRenderer();
            }

            foreach (GameObject obj in objectsWithMeshRenderer)
            {
                EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
            }
        }

        private void FindObjectsWithMeshRenderer()
        {
            objectsWithMeshRenderer.Clear();
            MeshRenderer[] meshRenderers = FindObjectsByType<MeshRenderer>(FindObjectsSortMode.None);

            foreach (MeshRenderer renderer in meshRenderers)
            {
                objectsWithMeshRenderer.Add(renderer.gameObject);
            }
        }
    }

}