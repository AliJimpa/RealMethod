using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace RealMethod.Editor
{

    public class FindDependencyInjectionWindow : EditorWindow
    {
        private GameObject targetPrefab;
        private string targetPrefabGUID;
        private Dictionary<string, List<string>> fileReferences = new Dictionary<string, List<string>>();

        [MenuItem("Tools/RealMethod/General/FindDependencyInjection")]
        public static void ShowWindow()
        {
            GetWindow<FindDependencyInjectionWindow>("FindDependencyInjection");
        }

        private void OnGUI()
        {
            GUILayout.Label("Select Your Asset", EditorStyles.boldLabel);

            targetPrefab = (GameObject)EditorGUILayout.ObjectField("Target Asset", targetPrefab, typeof(GameObject), false);

            if (GUILayout.Button("Find Asset References"))
            {
                FindPrefabReferences();
            }

            GUILayout.Space(10);

            if (fileReferences.Count > 0)
            {
                foreach (var file in fileReferences)
                {
                    GUILayout.Label("File: " + file.Key, EditorStyles.boldLabel);

                    foreach (var line in file.Value)
                    {
                        GUILayout.Label(line);
                    }

                    GUILayout.Space(10);
                }
            }
        }

        private void FindPrefabReferences()
        {
            fileReferences.Clear();

            if (targetPrefab == null)
            {
                Debug.LogWarning("Please assign a target asset.");
                return;
            }

            targetPrefabGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(targetPrefab));

            // Get all files in the project
            string[] allFiles = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);

            foreach (string filePath in allFiles)
            {
                // Only check relevant file types
                if (filePath.EndsWith(".meta") || filePath.EndsWith(".unity") || filePath.EndsWith(".prefab"))
                {
                    string[] fileLines = File.ReadAllLines(filePath);
                    foreach (string line in fileLines)
                    {
                        if (line.Contains(targetPrefabGUID))
                        {
                            string relativePath = "Assets" + filePath.Replace(Application.dataPath, "").Replace("\\", "/");
                            string fileName = Path.GetFileName(relativePath);

                            if (!fileReferences.ContainsKey(fileName))
                            {
                                fileReferences[fileName] = new List<string>();
                            }
                            fileReferences[fileName].Add(relativePath);
                        }
                    }
                }
            }
        }
    }

}