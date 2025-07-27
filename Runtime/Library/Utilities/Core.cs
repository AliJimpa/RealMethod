using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RealMethod
{
    public static class RM_Core
    {
        public class input
        {
            public static bool IsTouchOverUI(GraphicRaycaster HUD_graphicRaycaster, Vector2 touchPosition)
            {
                // Create a pointer event data from the touch position
                PointerEventData pointerEventData = new PointerEventData(null);
                pointerEventData.position = touchPosition;

                // Create a list to hold the results
                List<RaycastResult> results = new List<RaycastResult>();

                // Raycast using the GraphicRaycaster and pointer event data
                HUD_graphicRaycaster.Raycast(pointerEventData, results);

                // Check if we hit any UI elements
                return results.Count > 0;
            }
        }

        public class enumerables
        {
            public static T GetGameObject<T>(IEnumerable<T> items, int index)
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index must be non-negative.");

                // Convert to array or list if you need index access
                var itemList = items as IList<T> ?? new List<T>(items);

                if (index >= itemList.Count)
                    throw new ArgumentOutOfRangeException(nameof(index), "Index exceeds the collection count.");

                return itemList[index];
            }
        }

        public class file
        {
            public static bool WriteToFile(string contents, string fullPath)
            {
                try
                {
                    File.WriteAllText(fullPath, contents);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to write to {fullPath} with exception {e}");
                    return false;
                }
            }
            public static bool WriteToFile(string contents, string fileName, string ext)
            {
                var fullPath = Path.Combine(Application.persistentDataPath, fileName + ext);
                return WriteToFile(fullPath, contents);
            }

            public static bool ReadFromFile(string fullPath, out string result)
            {
                if (!File.Exists(fullPath))
                {
                    result = string.Empty;
                    return false;
                }

                try
                {
                    result = File.ReadAllText(fullPath);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to read from {fullPath} with exception {e}");
                    result = "";
                    return false;
                }
            }
            public static bool ReadFromFile(string fileName, string ext, out string result)
            {
                var fullPath = Path.Combine(Application.persistentDataPath, fileName + ext);
                return ReadFromFile(fullPath, out result);
            }

            public static bool MoveFile(string fullPath_A, string fullPath_B)
            {
                try
                {
                    if (File.Exists(fullPath_B))
                    {
                        File.Delete(fullPath_B);
                    }

                    if (!File.Exists(fullPath_A))
                    {
                        return false;
                    }

                    File.Move(fullPath_A, fullPath_B);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to move file from {fullPath_A} to {fullPath_B} with exception {e}");
                    return false;
                }

                return true;
            }
            public static bool MoveFile(string fileName_A, string fileName_B, string ext)
            {
                var fullPath_A = Path.Combine(Application.persistentDataPath, fileName_A + ext);
                var fullPath_B = Path.Combine(Application.persistentDataPath, fileName_B + ext);
                return MoveFile(fullPath_A, fullPath_B);
            }
        }

        public class expression
        {
            public static string GetVariableName<T>(Expression<Func<T>> expression)
            {
                if (expression.Body is MemberExpression memberExpression)
                {
                    return memberExpression.Member.Name;
                }
                throw new ArgumentException("Expression is not a valid member expression.");
            }
        }

        public  class prefs
        {
            // Boolean
            public static void SetBool(string key, bool value)
            {
                PlayerPrefs.SetInt(key, value ? 1 : 0);
            }
            public static bool GetBool(string key, bool defaultValue = false)
            {
                return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
            }

            // Vector3
            public static void SetVector3(string key, Vector3 value)
            {
                PlayerPrefs.SetFloat(key + "_V3x", value.x);
                PlayerPrefs.SetFloat(key + "_V3y", value.y);
                PlayerPrefs.SetFloat(key + "_V3z", value.z);
            }
            public static Vector3 GetVector3(string key, Vector3 defaultValue = default)
            {
                if (!PlayerPrefs.HasKey(key + "_V3x")) return defaultValue;

                float x = PlayerPrefs.GetFloat(key + "_V3x");
                float y = PlayerPrefs.GetFloat(key + "_V3y");
                float z = PlayerPrefs.GetFloat(key + "_V3z");
                return new Vector3(x, y, z);
            }
            public static bool HasVector3(string key)
            {
                return PlayerPrefs.HasKey(key + "_V3x");
            }
            public static void DeleteVector3(string key)
            {
                PlayerPrefs.DeleteKey(key + "_V3x");
                PlayerPrefs.DeleteKey(key + "_V3y");
                PlayerPrefs.DeleteKey(key + "_V3z");
                PlayerPrefs.DeleteKey(key + "_V3w");
            }

            // Vector2
            public static void SetVector2(string key, Vector2 value)
            {
                PlayerPrefs.SetFloat(key + "_V2x", value.x);
                PlayerPrefs.SetFloat(key + "_V2y", value.y);
            }
            public static Vector2 GetVector2(string key, Vector2 defaultValue = default)
            {
                if (!PlayerPrefs.HasKey(key + "_V2x")) return defaultValue;

                float x = PlayerPrefs.GetFloat(key + "_V2x");
                float y = PlayerPrefs.GetFloat(key + "_V2y");
                return new Vector2(x, y);
            }
            public static bool HasVector2(string key)
            {
                return PlayerPrefs.HasKey(key + "_V2x");
            }
            public static void DeleteVector2(string key)
            {
                PlayerPrefs.DeleteKey(key + "_V2x");
                PlayerPrefs.DeleteKey(key + "_V2y");
                PlayerPrefs.DeleteKey(key + "_V2z");
                PlayerPrefs.DeleteKey(key + "_V2w");
            }

            // Quaternion
            public static void SetQuaternion(string key, Quaternion value)
            {
                PlayerPrefs.SetFloat(key + "_Qx", value.x);
                PlayerPrefs.SetFloat(key + "_Qy", value.y);
                PlayerPrefs.SetFloat(key + "_Qz", value.z);
                PlayerPrefs.SetFloat(key + "_Qw", value.w);
            }
            public static Quaternion GetQuaternion(string key, Quaternion defaultValue = default)
            {
                if (!PlayerPrefs.HasKey(key + "_Qx")) return defaultValue;

                float x = PlayerPrefs.GetFloat(key + "_Qx");
                float y = PlayerPrefs.GetFloat(key + "_Qy");
                float z = PlayerPrefs.GetFloat(key + "_Qz");
                float w = PlayerPrefs.GetFloat(key + "_Qw");
                return new Quaternion(x, y, z, w);
            }
            public static bool HasQuaternion(string key)
            {
                return PlayerPrefs.HasKey(key + "_Qx");
            }
            public static void DeleteQuaternion(string key)
            {
                PlayerPrefs.DeleteKey(key + "_Qx");
                PlayerPrefs.DeleteKey(key + "_Qy");
                PlayerPrefs.DeleteKey(key + "_Qz");
                PlayerPrefs.DeleteKey(key + "_Qw");
            }

            // Transform
            public static void SetTransform(string key, Transform transform, bool includeScale = false)
            {
                // Save Position
                PlayerPrefs.SetFloat(key + "_pos_x", transform.position.x);
                PlayerPrefs.SetFloat(key + "_pos_y", transform.position.y);
                PlayerPrefs.SetFloat(key + "_pos_z", transform.position.z);

                // Save Rotation
                PlayerPrefs.SetFloat(key + "_rot_x", transform.rotation.x);
                PlayerPrefs.SetFloat(key + "_rot_y", transform.rotation.y);
                PlayerPrefs.SetFloat(key + "_rot_z", transform.rotation.z);
                PlayerPrefs.SetFloat(key + "_rot_w", transform.rotation.w);

                if (includeScale)
                {
                    PlayerPrefs.SetFloat(key + "_scale_x", transform.localScale.x);
                    PlayerPrefs.SetFloat(key + "_scale_y", transform.localScale.y);
                    PlayerPrefs.SetFloat(key + "_scale_z", transform.localScale.z);
                }

                PlayerPrefs.Save();
            }
            public static void GetTransform(string key, Transform transform, bool includeScale = false)
            {
                if (!PlayerPrefs.HasKey(key + "_pos_x")) return; // check if saved

                // Load Position
                float px = PlayerPrefs.GetFloat(key + "_pos_x");
                float py = PlayerPrefs.GetFloat(key + "_pos_y");
                float pz = PlayerPrefs.GetFloat(key + "_pos_z");

                // Load Rotation
                float rx = PlayerPrefs.GetFloat(key + "_rot_x");
                float ry = PlayerPrefs.GetFloat(key + "_rot_y");
                float rz = PlayerPrefs.GetFloat(key + "_rot_z");
                float rw = PlayerPrefs.GetFloat(key + "_rot_w");

                transform.position = new Vector3(px, py, pz);
                transform.rotation = new Quaternion(rx, ry, rz, rw);

                if (includeScale && PlayerPrefs.HasKey(key + "_scale_x"))
                {
                    float sx = PlayerPrefs.GetFloat(key + "_scale_x");
                    float sy = PlayerPrefs.GetFloat(key + "_scale_y");
                    float sz = PlayerPrefs.GetFloat(key + "_scale_z");
                    transform.localScale = new Vector3(sx, sy, sz);
                }
            }
            public static bool HasTransform(string key)
            {
                return PlayerPrefs.HasKey(key + "_pos_x");
            }
            public static void DeleteTransform(string key)
            {
                string[] suffixes = {
            "_pos_x", "_pos_y", "_pos_z",
            "_rot_x", "_rot_y", "_rot_z", "_rot_w",
            "_scale_x", "_scale_y", "_scale_z"
        };

                foreach (var suffix in suffixes)
                {
                    PlayerPrefs.DeleteKey(key + suffix);
                }
            }

            // Array
            public static void SetArray<T>(string key, T[] array)
            {
                string joined = string.Join("|", array); // Use a delimiter unlikely to appear in your strings
                PlayerPrefs.SetString(key, joined);
                PlayerPrefs.Save();
            }
            public static T[] GetArray<T>(string key)
            {
                if (!PlayerPrefs.HasKey(key)) return new T[0];

                string joined = PlayerPrefs.GetString(key);
                return joined.Split('|').Select(s => (T)System.Convert.ChangeType(s, typeof(T))).ToArray(); // Convert each string to T
            }
        }

    }
}