using System.Linq;
using UnityEngine;

namespace RealMethod
{
    public static class RM_PlayerPrefs
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
        public static void DeleteVector2(this string key)
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