using System;
using System.IO;
using UnityEngine;

namespace RealMethod
{
    static class RM_FileSystem
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

}