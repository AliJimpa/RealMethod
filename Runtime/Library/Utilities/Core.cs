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

        public class enume
        {
            public static bool AreEnumValuesEqual<T, J>(T a, J b) where T : Enum where J : Enum
            {
                return Convert.ToInt32(a) == Convert.ToInt32(b);
            }
        }
    }
}