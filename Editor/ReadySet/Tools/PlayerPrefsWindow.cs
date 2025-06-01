using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Microsoft.Win32;

namespace RealMethod
{
    public class PlayerPrefsWindow : EditorWindow
    {
        private Vector2 scroll;
        private Dictionary<string, string> prefs = new Dictionary<string, string>();

        private string newKey = "";
        private string newValue = "";
        private ValueType newType = ValueType.String;

        private string checkKey = "";
        private bool keyExists = false;

        private Dictionary<string, string> editedValues = new Dictionary<string, string>();

        private enum ValueType { String, Int, Float }

        [MenuItem("Tools/RealMethod/General/PlayerPrefsViewer")]
        public static void ShowWindow()
        {
            GetWindow<PlayerPrefsWindow>("PlayerPrefs Viewer");
        }

        private void OnEnable()
        {
            RefreshPrefs();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(); ////////PART Refresh
            if (GUILayout.Button("🔄 Refresh List")) RefreshPrefs();

            if (GUILayout.Button("🧨 Clear All PlayerPrefs"))
            {
                if (EditorUtility.DisplayDialog("Confirm", "Delete ALL PlayerPrefs?", "Yes", "No"))
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                    RefreshPrefs();
                }
            }

            GUILayout.Space(20);
            EditorGUILayout.LabelField("🗂️ Saved PlayerPrefs", EditorStyles.boldLabel);

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(300));
            foreach (var kvp in prefs)
            {
                EditorGUILayout.BeginHorizontal();

                // Key
                EditorGUILayout.LabelField(kvp.Key, GUILayout.Width(150));

                // Value type detection
                string value = kvp.Value;
                string typeLabel = "String";
                GUIStyle typeStyle = new GUIStyle(EditorStyles.label);

                if (int.TryParse(value, out _))
                {
                    typeLabel = "Int";
                    typeStyle.normal.textColor = Color.green;
                }
                else if (float.TryParse(value, out _))
                {
                    typeLabel = "Float";
                    typeStyle.normal.textColor = Color.red;
                }
                else
                {
                    typeStyle.normal.textColor = Color.blue;
                }

                // Type tag
                GUILayout.Label($"[{typeLabel}]", typeStyle, GUILayout.Width(50));

                // Editable field
                string Targetvalue = editedValues.ContainsKey(kvp.Key) ? kvp.Value : "Null";
                editedValues[kvp.Key] = EditorGUILayout.TextField(Targetvalue);

                // Save button
                if (GUILayout.Button("💾", GUILayout.Width(30)))
                {
                    string val = editedValues[kvp.Key];
                    if (int.TryParse(val, out int iVal))
                        PlayerPrefs.SetInt(kvp.Key, iVal);
                    else if (float.TryParse(val, out float fVal))
                        PlayerPrefs.SetFloat(kvp.Key, fVal);
                    else
                        PlayerPrefs.SetString(kvp.Key, val);

                    PlayerPrefs.Save();
                    RefreshPrefs();
                }

                // Delete button
                if (GUILayout.Button("🗑️", GUILayout.Width(30)))
                {
                    PlayerPrefs.DeleteKey(kvp.Key);
                    PlayerPrefs.Save();
                    RefreshPrefs();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            GUILayout.Space(20); ////////PART Save/Remove
            EditorGUILayout.LabelField("➕ Add / Remove PlayerPref", EditorStyles.boldLabel);

            newKey = EditorGUILayout.TextField("Key", newKey);
            newValue = EditorGUILayout.TextField("Value", newValue);
            newType = (ValueType)EditorGUILayout.EnumPopup("Type", newType);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("💾 Save Key"))
            {
                if (!string.IsNullOrEmpty(newKey))
                {
                    string TargetValue = newValue;
                    switch (newType)
                    {
                        case ValueType.String:
                            PlayerPrefs.SetString(newKey, TargetValue);
                            break;
                        case ValueType.Int:
                            if (int.TryParse(TargetValue, out int iVal))
                                PlayerPrefs.SetInt(newKey, iVal);
                            break;
                        case ValueType.Float:
                            if (float.TryParse(TargetValue, out float fVal))
                                PlayerPrefs.SetFloat(newKey, fVal);
                            break;
                    }
                    PlayerPrefs.Save();
                    RefreshPrefs();
                }
            }

            if (GUILayout.Button("🗑️ Remove Key"))
            {
                if (PlayerPrefs.HasKey(newKey))
                {
                    PlayerPrefs.DeleteKey(newKey);
                    PlayerPrefs.Save();
                    RefreshPrefs();
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(20); ////////PART Check
            EditorGUILayout.LabelField("🔍 Check If Key Exists", EditorStyles.boldLabel);
            checkKey = EditorGUILayout.TextField("Check Key", checkKey);

            if (GUILayout.Button("Check Key"))
            {
                keyExists = PlayerPrefs.HasKey(checkKey);
            }

            if (!string.IsNullOrEmpty(checkKey))
            {
                EditorGUILayout.LabelField("Exists:", keyExists ? "✅ Yes" : "❌ No");
            }
        }



        private void RefreshPrefs()
        {
            prefs.Clear();
            editedValues.Clear();

            var keys = GetAllSavedKeys();
            foreach (var key in keys)
            {
                if (PlayerPrefs.HasKey(key))
                {
                    string value = PlayerPrefs.GetString(key, "");
                    if (int.TryParse(value, out _) || float.TryParse(value, out _))
                    {
                        // Prefer int/float if it parses correctly
                        if (PlayerPrefs.GetInt(key).ToString() == value)
                            value = PlayerPrefs.GetInt(key).ToString();
                        else if (PlayerPrefs.GetFloat(key).ToString() == value)
                            value = PlayerPrefs.GetFloat(key).ToString();
                    }

                    prefs[key] = value;
                }
            }
        }

        private List<string> GetAllSavedKeys()
        {
            List<string> keys = new List<string>();

#if UNITY_EDITOR_WIN
            string companyName = Application.companyName;
            string productName = Application.productName;

            string path = $@"Software\Unity\UnityEditor\{companyName}\{productName}";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
            {
                if (key != null)
                {
                    foreach (var valueName in key.GetValueNames())
                    {
                        if (!string.IsNullOrEmpty(valueName) && !keys.Contains(valueName))
                            keys.Add(valueName);
                    }
                }
            }
#else
    // Fallback if not on Windows
#endif

            return keys;
        }

    }
}
