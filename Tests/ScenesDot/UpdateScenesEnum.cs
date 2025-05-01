using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System.IO;

namespace ScenesDot
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    class UpdateScenesEnum
    {
        //an array of the scenes in the build
        private static string[] scenes = new string[0];

        //The folder where this script (UpdteScenesEnum) is located in.
        //With help from (https://stackoverflow.com/questions/65013213/how-to-get-current-folder-from-a-c-sharp-static-class-on-unity)
        private static string ThisFolder
        {
            get
            {
                var g = AssetDatabase.FindAssets($"t:Script {nameof(UpdateScenesEnum)}");

                //the full path of this script (including the name of this script)
                string fullFilePath = AssetDatabase.GUIDToAssetPath(g[0]);

                //remove the name of this asset
                int endCut = fullFilePath.LastIndexOf('/') + 1;

                string folderName = fullFilePath.Substring(0, endCut);

                return folderName;
                //the folder where this script is located in (excluding the name of this script (UpdatesSceneEnum.cs))
                //return Path.GetFullPath(Path.Combine(fullFilePath, "../"));
            }
        }

        //The file which contains the enum of Scenes. Should be located in same folder as this script and should be called ScenesEnum.cs.
        private static string path
        {
            get
            {
                return ThisFolder + "ScenesEnum.cs";
            }
        }

        static UpdateScenesEnum()
        {
            //EditorApplication.update += Update;
        }

        //This function is continously called while in the editor
        static void Update()
        {
            //if the file to write to could not be found, then show an error
            if (File.Exists(path) == false)
            {
                Debug.LogError("Cannot find \"ScenesEnum.cs\" file in " + ThisFolder);
            }
            else
            {
                //when the Refresh happens, the script resets causing a false negative in arraysEqual (because scenes is blank)
                if (Application.isPlaying == false && arraysEqual(scenes, getAllBuildScenes()) == false)
                {
                    //Set the scenes array to be equal to the names of scenes in build
                    scenes = getAllBuildScenes();

                    if (scenes.Length == 0)
                    {
                        Debug.LogWarning("There are no scenes in Build Settings! Is this really the desired behaviour?");
                    }

                    //If the file already contains the correct scenes (the arraysEqual gave a false negative)
                    if (isFileUpToDate())
                    {
                        Debug.Log("\"Scenes\" enum is up to date");
                    }
                    //if the file is old and needs to be updated
                    else
                    {
                        //This code write all the scenes to an enum called "Scenes".
                        Debug.Log("Scenes in Build changed. Updating \"Scenes\" enum.");

                        //writes an enum with the list of scenes as each enum item into the given file
                        string textToWrite = "namespace ScenesDot\n{\n\tpublic enum Scenes\n\t{\n" + convertArrayToString(getAllBuildScenes()) + "\t}\n}";
                        File.WriteAllText(path, textToWrite);

                        //Refreshes all files that have changed (including also resetting this file)
                        AssetDatabase.Refresh();
                    }

                }

            }


        }

        /// <summary>
        /// Reads the file with scene enums and checks to see if it matches the current build scenes
        /// </summary>
        /// <returns>True if file is up to date, false otherwise</returns>
        private static bool isFileUpToDate()
        {
            //Used resources from here to learn to read from file:
            //https://bytehide.com/blog/streamreader-csharp
            using (StreamReader reader = new StreamReader(path))
            {
                string line = "";
                string allScenesInFile = "";
                int lineCounter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    //the scene names start on the fifth line after the "namespace ScenesDot", "{", "public enum Scenes" and the "{" line
                    if (lineCounter >= 4)
                    {
                        allScenesInFile += line + "\n";
                    }

                    lineCounter += 1;
                }

                //remove the two trailing next line and curly bracket from the end
                allScenesInFile = allScenesInFile.Substring(0, allScenesInFile.Length - 5);

                reader.Close();

                return (allScenesInFile == convertArrayToString(getAllBuildScenes()));
            }

        }

        /// <summary>
        /// Returns an array of the names of scenes in build settings
        /// </summary>
        /// <returns>Array of scene names in build settings</returns>
        private static string[] getAllBuildScenes()
        {
            //create a new string with length equal to the number of scenes in game
            string[] allScenesInBuild = new string[SceneManager.sceneCountInBuildSettings];

            //set the value of each item to be the name of the scene
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                allScenesInBuild[i] = convertScenePathToSceneName(EditorBuildSettings.scenes[i].path);
            }

            return allScenesInBuild;
        }


        /// <summary>
        /// Converts the full scene path into a simple name
        /// </summary>
        /// <returns>The short name of the scene</returns>
        private static string convertScenePathToSceneName(string scenePath)
        {
            //A scene path looks like this:
            //Assets/Scenes/SampleScene.unity

            // The name of the scene is listed after the last /.
            int startCut = scenePath.LastIndexOf('/') + 1;

            //the . is the character right before the end of the scene name
            int endCut = scenePath.LastIndexOf('.');

            //get the length of the scene name
            int sceneNameLength = endCut - startCut;

            string sceneName = scenePath.Substring(startCut, sceneNameLength);

            return sceneName;

        }

        /// <summary>
        /// Checks if two arrays have identical items at same indexes
        /// </summary>
        /// <param name="array1">An array</param>
        /// <param name="array2">Another array</param>
        /// <returns>True if arrays are equal, false otherwise</returns>
        private static bool arraysEqual(string[] array1, string[] array2)
        {
            //if the number of items is not equal, then they are not equal
            if (array1.Length != array2.Length)
            {
                return false;
            }

            //Length of both arrays is same.
            // Check to make sure each item is same and in same order
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }


            //if all items were the same
            return true;

        }

        /// <summary>
        /// Converts the array to a single string with next lines and commas between items
        /// </summary>
        /// <param name="array">The array to convert to a string</param>
        /// <returns>A single string ready for use in an enum</returns>
        private static string convertArrayToString(string[] array)
        {
            string returnValue = "";

            //commas are fine on the last item
            foreach (string item in array)
            {
                //inserts two tabs before the item
                returnValue += "\t\t"+item + ",\n";
            }

            return returnValue;

        }

    }
#endif

}

