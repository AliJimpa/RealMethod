using System;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class SceneReference : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    public SceneAsset SceneAsset; // Only in editor
#endif
    public string ScenePath;
    public string ScneName => System.IO.Path.GetFileNameWithoutExtension(ScenePath);

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        UpdateScenePath();
#endif
    }

    public void OnAfterDeserialize() { }

#if UNITY_EDITOR
    private void UpdateScenePath()
    {
        if (SceneAsset != null)
        {
            string newPath = AssetDatabase.GetAssetPath(SceneAsset);
            if (ScenePath != newPath)
            {
                ScenePath = newPath;
                EditorUtility.SetDirty(Selection.activeObject);
            }
        }
        else
        {
            ScenePath = string.Empty;
        }
    }
#endif

    // Implicit conversion to string
    public static implicit operator string(SceneReference sceneReference)
    {
        return sceneReference?.ScenePath;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneReference), true)]
public class CustomSceneReferencePropertyDrawer : PropertyDrawer
{
    private Texture2D checkIcon;
    private Texture2D warningIcon;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Load icons once
        if (checkIcon == null) checkIcon = EditorGUIUtility.IconContent("TestPassed").image as Texture2D;
        if (warningIcon == null) warningIcon = EditorGUIUtility.IconContent("console.warnicon").image as Texture2D;

        SerializedProperty sceneAssetProp = property.FindPropertyRelative("SceneAsset");
        SerializedProperty scenePathProp = property.FindPropertyRelative("ScenePath");

        EditorGUI.BeginProperty(position, label, property);

        // Restrict to SceneAsset type
        Rect objectFieldRect = new Rect(position.x, position.y, position.width - 20, position.height);
        EditorGUI.PropertyField(objectFieldRect, sceneAssetProp, label);

        if (sceneAssetProp.objectReferenceValue != null)
        {
            string path = AssetDatabase.GetAssetPath(sceneAssetProp.objectReferenceValue);
            scenePathProp.stringValue = path;

            bool isInBuild = IsSceneInBuild(path);

            // Icon display
            Rect iconRect = new Rect(position.x + position.width - 16, position.y + 2, 16, 16);
            GUI.DrawTexture(iconRect, isInBuild ? checkIcon : warningIcon);

            // Tooltip if not included
            if (!isInBuild)
            {
                GUIContent warningContent = new GUIContent("", "Scene is NOT in the active Build Profile.\nAdd it via File > Build Profiles.");
                EditorGUI.LabelField(iconRect, warningContent);
            }
        }
        else
        {
            scenePathProp.stringValue = string.Empty;
        }

        EditorGUI.EndProperty();
    }

    private bool IsSceneInBuild(string scenePath)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string listedScenePath = SceneUtility.GetScenePathByBuildIndex(i);
            if (listedScenePath == scenePath)
                return true;
        }
        return false;
    }
}
#endif





