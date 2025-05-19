using System;
using System.Reflection;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public enum SaveMethod
    {
        Binary,
        XML,
        JSON,
        YAML,
        PlayerPrefs,
    }
    
    [AddComponentMenu("RealMethod/Manager/SaveManager")]
    public sealed class SaveManager : DataManager<SaveMethod>
    {
        public override void InitiateService(Service service)
        {

        }


        protected override bool IsExist(SaveFile file)
        {
            switch (Method)
            {
                case SaveMethod.Binary:
                    return false;
                case SaveMethod.XML:
                    return false;
                case SaveMethod.JSON:
                    return false;
                case SaveMethod.YAML:
                    return false;
                case SaveMethod.PlayerPrefs:
                    if (PlayerPrefs.HasKey("LastSave"))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    Debug.LogWarning($"The {Method} is not implement");
                    return false;
            }
        }
        protected override void OnSaveFile(SaveFile file)
        {
            switch (Method)
            {
                case SaveMethod.Binary:
                    break;
                case SaveMethod.XML:
                    break;
                case SaveMethod.JSON:
                    //Mustard.FileManager.WriteToFile(Filename, JsonUtility.ToJson(File));
                    break;
                case SaveMethod.YAML:
                    break;
                case SaveMethod.PlayerPrefs:
                    PlayerPrefs.SetString("LastSave", DateTime.Now.ToString());
                    Type objectType = file.GetType();
                    foreach (var field in objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (field.IsDefined(typeof(SerializeField), true) || field.IsPublic)
                        {
                            SaveValueToPlayerPrefs(field.Name, field.GetValue(file));
                        }
                    }
                    foreach (var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (property.CanRead && property.CanWrite)
                        {
                            SaveValueToPlayerPrefs(property.Name, property.GetValue(file));
                        }
                    }
                    PlayerPrefs.Save();
                    Debug.Log("All variables saved to PlayerPrefs!");
                    break;
                default:
                    break;
            }
        }
        protected override void OnLoadFile(SaveFile file)
        {
            switch (Method)
            {
                case SaveMethod.Binary:
                    break;
                case SaveMethod.XML:
                    break;
                case SaveMethod.JSON:
                    //slot = new SaveFile();
                    //Mustard.FileManager.LoadFromFile(Filename, out var json);
                    //JsonUtility.FromJsonOverwrite(json, slot);
                    break;
                case SaveMethod.YAML:
                    break;
                case SaveMethod.PlayerPrefs:
                    Type objectType = file.GetType();
                    foreach (var field in objectType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (field.IsDefined(typeof(SerializeField), true) || field.IsPublic)
                        {
                            LoadFieldFromPlayerPrefs(file, field);
                        }
                    }
                    foreach (var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        if (property.CanRead && property.CanWrite)
                        {
                            LoadPropertyFromPlayerPrefs(file, property);
                        }
                    }
                    string Date = PlayerPrefs.GetString("LastSave");
                    Debug.Log($"All variables loaded from PlayerPrefs!  <{Date}>");
                    break;
                default:
                    break;
            }
        }
        protected override void OnDelete(SaveFile file)
        {
            switch (Method)
            {
                case SaveMethod.Binary:
                    break;
                case SaveMethod.XML:
                    break;
                case SaveMethod.JSON:
                    break;
                case SaveMethod.YAML:
                    break;
                case SaveMethod.PlayerPrefs:
                    PlayerPrefs.DeleteAll();
                    break;
                default:
                    break;
            }
        }



        private void SaveValueToPlayerPrefs(string key, object value)
        {
            if (value is int intValue)
            {
                PlayerPrefs.SetInt(key, intValue);
            }
            else if (value is float floatValue)
            {
                PlayerPrefs.SetFloat(key, floatValue);
            }
            else if (value is string stringValue)
            {
                PlayerPrefs.SetString(key, stringValue);
            }
            else if (value is bool boolValue)
            {
                PlayerPrefs.SetInt(key, boolValue ? 1 : 0);
            }
            else
            {
                Debug.LogWarning($"Unsupported type for PlayerPrefs: {value?.GetType().Name} (Key: {key})");
            }
        }
        private void LoadFieldFromPlayerPrefs(SaveFile File, FieldInfo field)
        {
            string key = field.Name;

            if (!PlayerPrefs.HasKey(key)) return;

            if (field.FieldType == typeof(int))
            {
                field.SetValue(File, PlayerPrefs.GetInt(key));
            }
            else if (field.FieldType == typeof(float))
            {
                field.SetValue(File, PlayerPrefs.GetFloat(key));
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(File, PlayerPrefs.GetString(key));
            }
            else if (field.FieldType == typeof(bool))
            {
                field.SetValue(File, PlayerPrefs.GetInt(key) == 1);
            }
        }
        private void LoadPropertyFromPlayerPrefs(SaveFile File, PropertyInfo property)
        {
            string key = property.Name;

            if (!PlayerPrefs.HasKey(key)) return;

            if (property.PropertyType == typeof(int))
            {
                property.SetValue(File, PlayerPrefs.GetInt(key));
            }
            else if (property.PropertyType == typeof(float))
            {
                property.SetValue(File, PlayerPrefs.GetFloat(key));
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(File, PlayerPrefs.GetString(key));
            }
            else if (property.PropertyType == typeof(bool))
            {
                property.SetValue(File, PlayerPrefs.GetInt(key) == 1);
            }
        }


    }
}