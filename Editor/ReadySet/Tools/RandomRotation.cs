using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public class RandomRotation
    {
        [MenuItem("Tools/RealMethod/Scene/Randomize Rotation %#t")] // Ctrl + Shift + R (Cmd + Shift + R on mac)
        private static void RandomizeRotation()
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                Undo.RecordObject(obj.transform, "Randomize Rotation");
                obj.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }
        }
    }
}
