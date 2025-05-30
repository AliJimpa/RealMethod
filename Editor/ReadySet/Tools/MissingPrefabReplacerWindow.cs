using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{

    public class MissingPrefabReplacerWindow : EditorWindow
    {
        private GameObject prefabToCheck;
        private Dictionary<GameObject, GameObject> brokenInstances = new Dictionary<GameObject, GameObject>();

        [MenuItem("Tools/RealMethod/General/MissingPrefabReplacer")]
        public static void ShowWindow()
        {
            GetWindow<MissingPrefabReplacerWindow>("Missing Prefab Replacer");
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Select a Prefab to Scan", EditorStyles.boldLabel);
            prefabToCheck = (GameObject)EditorGUILayout.ObjectField("Target Prefab", prefabToCheck, typeof(GameObject), false);

            if (GUILayout.Button("Scan for Missing Prefabs"))
            {
                ScanPrefab();
            }

            if (brokenInstances.Count > 0)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("Missing Prefabs Found:", EditorStyles.boldLabel);

                foreach (var kvp in brokenInstances)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField("Broken Instance", kvp.Key, typeof(GameObject), true);
                    brokenInstances[kvp.Key] = (GameObject)EditorGUILayout.ObjectField("New Prefab", kvp.Value, typeof(GameObject), false);
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Replace Missing Prefabs"))
                {
                    ReplaceMissingPrefabs();
                }
            }
        }

        private void ScanPrefab()
        {
            brokenInstances.Clear();

            if (prefabToCheck == null)
            {
                Debug.LogWarning("No prefab selected.");
                return;
            }

            GameObject instance = null;
            try
            {
                instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabToCheck);
            }
            catch (System.Exception)
            {
                Debug.LogWarning("An exception occurred while instantiating the prefab.");
                throw;
            }
            var allChildren = instance.GetComponentsInChildren<Transform>(true);

            foreach (var child in allChildren)
            {
                var original = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
                if (original == null && child != instance.transform)
                {
                    brokenInstances.Add(child.gameObject, null);
                }
            }

            DestroyImmediate(instance);
            Debug.Log($"Found {brokenInstances.Count} missing prefab(s).");
        }

        private void ReplaceMissingPrefabs()
        {
            string path = AssetDatabase.GetAssetPath(prefabToCheck);
            GameObject root = PrefabUtility.LoadPrefabContents(path);

            foreach (var kvp in brokenInstances)
            {
                var brokenGO = root.transform.Find(kvp.Key.name);
                if (brokenGO == null) continue;

                GameObject replacement = kvp.Value;
                if (replacement == null) continue;

                GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(replacement);
                newInstance.transform.SetParent(brokenGO.parent);
                newInstance.transform.localPosition = brokenGO.localPosition;
                newInstance.transform.localRotation = brokenGO.localRotation;
                newInstance.transform.localScale = brokenGO.localScale;
                newInstance.name = replacement.name;

                Object.DestroyImmediate(brokenGO.gameObject);
            }

            PrefabUtility.SaveAsPrefabAsset(root, path);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log("Replacement complete!");
            brokenInstances.Clear();
        }
    }

}