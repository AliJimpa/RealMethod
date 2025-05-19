using UnityEditor;
using UnityEngine;

namespace RealMethod
{
    public class FolderSettingSection : ProjectSettingSection
    {
        private ProjectSettingAsset MyStorage;
        private bool isPanelMaximize = false; // Add a toggle for minimizing the panel

        // Implement Abstraction Methods
        protected override void Initialized()
        {
        }

        protected override void FirstSelected(ProjectSettingAsset Storage)
        {
            MyStorage = Storage;
        }

        protected override void Draw()
        {
            // Add a toggle button for minimizing or expanding the panel
            EditorGUILayout.BeginHorizontal();
            isPanelMaximize = EditorGUILayout.Foldout(isPanelMaximize, "Folder List", true, EditorStyles.foldoutHeader);
            if (GUILayout.Button("Create All", GUILayout.Width(80)))
            {
                foreach (var address in MyStorage.ProjectStructure)
                {
                    if (AssetDatabase.IsValidFolder(address.Path))
                    {
                        Debug.Log($"Folder exists: {address}");
                    }
                    else
                    {
                        string folderpath = address.Path;
                        string FolderAddress = string.Join("/", folderpath.Split('/')[..^1]); // Remove the last segment of the path
                        string folderName = System.IO.Path.GetFileName(folderpath); // Get the last segment of the path
                        CreateFolder(FolderAddress, folderName); // Create the folder
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            // If the panel is minimized, stop rendering the rest of the UI
            if (!isPanelMaximize)
            {
                return;
            }

            // Render the folder list
            for (int i = 0; i < MyStorage.ProjectStructure.Length; i++)
            {
                EditorGUILayout.BeginHorizontal(); // Start horizontal layout


                // Display the folder path as a text field
                MyStorage.ProjectStructure[i].Path = EditorGUILayout.TextField($"{i + 1}.{MyStorage.ProjectStructure[i].Identity}", MyStorage.ProjectStructure[i].Path);

                string ButtonName = AssetDatabase.IsValidFolder(MyStorage.ProjectStructure[i].Path) ? "Check" : "Create";
                // Add a button next to the text field
                if (GUILayout.Button(ButtonName, GUILayout.Width(60)))
                {
                    // Check if the folder exists
                    if (AssetDatabase.IsValidFolder(MyStorage.ProjectStructure[i].Path))
                    {
                        Debug.Log($"Folder exists: {MyStorage.ProjectStructure[i]}");
                    }
                    else
                    {
                        string folderpath = MyStorage.ProjectStructure[i].Path;
                        string FolderAddress = string.Join("/", folderpath.Split('/')[..^1]); // Remove the last segment of the path
                        string folderName = System.IO.Path.GetFileName(folderpath); // Get the last segment of the path
                        CreateFolder(FolderAddress, folderName); // Create the folder
                    }
                }

                EditorGUILayout.EndHorizontal(); // End horizontal layout
            }
        }

        protected override string GetTitle()
        {
            return "FolderStructure";
        }

        protected override void Fix(int Id)
        {
        }

        private void CreateFolder(string parentFolder, string newFolderName)
        {
            string folderPath = System.IO.Path.Combine(parentFolder, newFolderName).Replace("\\", "/");

            // Split the folder path into parts and create each part if it doesn't exist
            string[] folders = folderPath.Split('/');
            string currentPath = folders[0]; // Start with the root (e.g., "Assets")

            for (int i = 1; i < folders.Length; i++)
            {
                string nextFolder = folders[i];
                string nextPath = System.IO.Path.Combine(currentPath, nextFolder).Replace("\\", "/");

                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, nextFolder);
                    Debug.Log($"Created folder: {nextPath}");
                }

                currentPath = nextPath; // Move to the next folder in the hierarchy
            }
        }
    }
}