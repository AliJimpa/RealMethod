using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RealMethod.Editor
{
    public static class RM_MetaData
    {
        private static string GetMetaFilePath(string assetPath) => assetPath + ".meta";

        // 🔹 Automatically determines the mode (Text in Editor, Binary in Play Mode)
        private static bool UseBinaryMode
        {
            get
            {
                return EditorSettings.serializationMode == SerializationMode.ForceBinary;
            }
        }

        // 🔹 Save metadata (Auto-Switch)
        public static void SaveCustomMetadata(string assetPath, string key, string value)
        {
            string metaFilePath = GetMetaFilePath(assetPath);

            if (!File.Exists(metaFilePath))
            {
                Debug.LogWarning($"Meta file not found for asset: {assetPath}");
                return;
            }

            if (UseBinaryMode)
                SaveBinaryMetadata(metaFilePath, key, value);
            else
                SaveTextMetadata(metaFilePath, key, value);
        }

        // 🔹 Load metadata (Auto-Switch)
        public static string LoadCustomMetadata(string assetPath, string key)
        {
            string metaFilePath = GetMetaFilePath(assetPath);
            if (!File.Exists(metaFilePath))
            {
                Debug.LogWarning($"Meta file not found for asset: {assetPath}");
                return null;
            }

            return UseBinaryMode ? LoadBinaryMetadata(metaFilePath, key) : LoadTextMetadata(metaFilePath, key);
        }

        // 🔹 Check if metadata exists
        public static bool HasMetadata(string assetPath, string key)
        {
            string metaFilePath = GetMetaFilePath(assetPath);
            return File.Exists(metaFilePath) && File.ReadAllText(metaFilePath).Contains($"{key}:");
        }

        // 🔹 Delete metadata entry
        public static void DeleteMetadata(string assetPath, string key)
        {
            string metaFilePath = GetMetaFilePath(assetPath);
            if (!File.Exists(metaFilePath)) return;

            string[] lines = File.ReadAllLines(metaFilePath);
            File.WriteAllLines(metaFilePath, Array.FindAll(lines, line => !line.StartsWith($"{key}:")));
        }

        // 🔹 Save Text Metadata
        private static void SaveTextMetadata(string metaFilePath, string key, string value)
        {
            string[] lines = File.ReadAllLines(metaFilePath);
            bool updated = false;

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith($"{key}:"))
                {
                    lines[i] = $"{key}: {value}";
                    updated = true;
                    break;
                }
            }

            if (!updated) File.AppendAllText(metaFilePath, $"\n{key}: {value}");
            else File.WriteAllLines(metaFilePath, lines);
        }

        // 🔹 Load Text Metadata
        private static string LoadTextMetadata(string metaFilePath, string key)
        {
            foreach (string line in File.ReadAllLines(metaFilePath))
            {
                if (line.StartsWith($"{key}:"))
                    return line.Substring(key.Length + 1).Trim();
            }
            return null;
        }

        // 🔹 Save Binary Metadata
        private static void SaveBinaryMetadata(string metaFilePath, string key, string value)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(metaFilePath, FileMode.Append)))
            {
                writer.Write(key);
                writer.Write(value);
            }
        }

        // 🔹 Load Binary Metadata
        private static string LoadBinaryMetadata(string metaFilePath, string key)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(metaFilePath, FileMode.Open)))
            {
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    string storedKey = reader.ReadString();
                    string storedValue = reader.ReadString();

                    if (storedKey == key) return storedValue;
                }
            }
            return null;
        }
    }
}