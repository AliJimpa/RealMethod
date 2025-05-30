using UnityEngine;
using UnityEditor;

namespace RealMethod.Editor
{
    public class PrefabMaterialChangerWindow : EditorWindow
    {
        private Material newMaterial; // Material to apply
        [SerializeField] private GameObject[] prefabs; // Prefabs to modify (marked as serialized)

        [MenuItem("Tools/RealMethod/General/PrefabMaterialChanger")]
        public static void ShowWindow()
        {
            GetWindow<PrefabMaterialChangerWindow>("Prefab Material Changer");
        }

        private void OnGUI()
        {
            GUILayout.Label("Change Material for GameObjects named 'Plane' in Prefabs", EditorStyles.boldLabel);

            // Input field for the new material
            newMaterial = (Material)EditorGUILayout.ObjectField("New Material", newMaterial, typeof(Material), false);

            // Input field for prefabs (with proper serialization)
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty prefabsProperty = serializedObject.FindProperty("prefabs");
            EditorGUILayout.PropertyField(prefabsProperty, new GUIContent("Prefabs"), true);
            serializedObject.ApplyModifiedProperties();

            // Change material button
            if (GUILayout.Button("Change Material"))
            {
                ChangeMaterialsInPrefabs();
            }
        }

        private void ChangeMaterialsInPrefabs()
        {
            if (newMaterial == null)
            {
                Debug.LogError("Please assign a new material!");
                return;
            }

            if (prefabs == null || prefabs.Length == 0)
            {
                Debug.LogError("Please assign at least one prefab!");
                return;
            }

            foreach (GameObject prefab in prefabs)
            {
                if (prefab == null)
                {
                    Debug.LogWarning("One of the prefabs is null. Skipping...");
                    continue;
                }

                string prefabPath = AssetDatabase.GetAssetPath(prefab);
                if (string.IsNullOrEmpty(prefabPath))
                {
                    Debug.LogWarning($"Could not find path for prefab: {prefab.name}. Skipping...");
                    continue;
                }

                // Load prefab asset and open it for editing
                GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

                // Find the GameObject named "Plane"
                Transform planeTransform = prefabInstance.transform.Find("Plane");
                if (planeTransform != null)
                {
                    Renderer renderer = planeTransform.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        Undo.RecordObject(renderer, "Change Material");
                        renderer.sharedMaterial = newMaterial;
                        Debug.Log($"Updated material on Plane in prefab: {prefab.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"GameObject 'Plane' in prefab '{prefab.name}' does not have a Renderer.");
                    }
                }
                else
                {
                    Debug.LogWarning($"GameObject 'Plane' not found in prefab '{prefab.name}'.");
                }

                // Save the changes and unload the prefab instance
                PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
                PrefabUtility.UnloadPrefabContents(prefabInstance);
            }

            Debug.Log("Material update completed for all prefabs.");
        }
    }

}