using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public enum SaveMethod
    {
        PlayerPrefs,
        TEXT,
        XML,
        JSON,
        Binary,
    }

    [AddComponentMenu("RealMethod/Manager/SaveManager")]
    public sealed class SaveManager : DataManager
    {
        [Header("Behavior")]
        [SerializeField] private SaveMethod format;
        public SaveMethod Format => format;
        [SerializeField, HideInInspectorByEnum("format", 0)]
        private bool CustomPath = false;
        [SerializeField, ConditionalHide("CustomPath", true, false)]
        private string FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        [SerializeField, ShowInInspectorByEnum("format", 0, 1)]
        private bool PublicField = true;
        [SerializeField, ShowInInspectorByEnum("format", 0, 1)]
        private bool ProtectedField = false;
        [SerializeField, ShowInInspectorByEnum("format", 0, 1)]
        private bool PrivateField = false;
        [SerializeField, ShowInInspectorByEnum("format", 0, 1)]
        private bool SerializeField = false;


        public override void InitiateService(Service service)
        {

        }


        protected override bool IsExist(SaveFile file)
        {
            switch (Format)
            {
                case SaveMethod.Binary:
                    return File.Exists(GetPath(file));
                case SaveMethod.XML:
                    return File.Exists(GetPath(file));
                case SaveMethod.JSON:
                    return File.Exists(GetPath(file));
                case SaveMethod.TEXT:
                    return File.Exists(GetPath(file));
                case SaveMethod.PlayerPrefs:
                    return PlayerPrefs.HasKey(file.name);
                default:
                    Debug.LogWarning($"The {Format} is not implement");
                    return false;
            }
        }
        protected override void OnSaveFile(SaveFile file)
        {
            switch (Format)
            {
                case SaveMethod.Binary:
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Suppress BinaryFormatter warning (only use in trusted context)
                    using (var stream = new FileStream(GetPath(file), FileMode.Create))
                    {
                        try
                        {
                            bf.Serialize(stream, file);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Failed to serialize {file.name} to {GetPath(file)}: {e}");
                            return;
                        }
                    }
#pragma warning restore SYSLIB0011
                    break;
                case SaveMethod.XML:
                    var xml = new XmlSerializer(file.GetType());
                    using (var stream = new FileStream(GetPath(file), FileMode.Create))
                    {
                        xml.Serialize(stream, file);
                    }
                    break;
                case SaveMethod.JSON:
                    string jsoncontent = JsonUtility.ToJson(file, true);
                    File.WriteAllText(GetPath(file), jsoncontent);
                    break;
                case SaveMethod.TEXT:
                    List<string> lines = new List<string>();
                    foreach (FieldInfo field in ScanFieldsFile(file))
                    {
                        lines.Add($"{field.Name}={field.GetValue(file)}");
                    }
                    File.WriteAllLines(GetPath(file), lines);
                    break;
                case SaveMethod.PlayerPrefs:
                    foreach (FieldInfo field in ScanFieldsFile(file))
                    {
                        PlayerPrefsSetValueByField(field, file);
                    }
                    PlayerPrefs.SetString(file.name, DateTime.Now.ToString());
                    PlayerPrefs.Save();
                    break;
                default:
                    Debug.LogWarning($"The {Format} is not implement");
                    break;
            }
            WriteLog($"Save ({Format})", file);
        }
        protected override void OnLoadFile(SaveFile file)
        {
            switch (Format)
            {
                case SaveMethod.Binary:
                    var bf = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                    using (var stream = new FileStream(GetPath(file), FileMode.Open))
                    {
                        file = (SaveFile)bf.Deserialize(stream);
                    }
#pragma warning restore SYSLIB0011
                    break;
                case SaveMethod.XML:
                    var xml = new XmlSerializer(file.GetType());
                    using (var stream = new FileStream(GetPath(file), FileMode.Open))
                    {
                        file = (SaveFile)xml.Deserialize(stream);
                    }
                    break;
                case SaveMethod.JSON:
                    string jsoncontent;
                    try
                    {
                        jsoncontent = File.ReadAllText(GetPath(file));
                        JsonUtility.FromJsonOverwrite(jsoncontent, file);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed to read from {GetPath(file)} with exception {e}");
                        return;
                    }
                    break;
                case SaveMethod.TEXT:
                    FieldInfo[] fields = ScanFieldsFile(file);
                    foreach (string line in File.ReadLines(GetPath(file)))
                    {
                        string[] parts = line.Split("=");
                        if (parts.Length < 2) continue;
                        string lineName = parts[0];
                        string lineValue = parts[1];
                        foreach (FieldInfo info in fields)
                        {
                            if (info.Name == lineName)
                            {
                                object value = null;
                                Type type = info.FieldType;
                                try
                                {
                                    if (type == typeof(int))
                                        value = int.Parse(lineValue);
                                    else if (type == typeof(float))
                                        value = float.Parse(lineValue);
                                    else if (type == typeof(bool))
                                        value = bool.Parse(lineValue);
                                    else if (type == typeof(string))
                                        value = lineValue;
                                    else if (type.IsEnum)
                                        value = Enum.Parse(type, lineValue);
                                    else if (type == typeof(byte))
                                        value = byte.Parse(lineValue);
                                    else
                                        Debug.LogWarning($"Unsupported type: {type.Name}");
                                    info.SetValue(file, value);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogWarning($"Failed to set value for {info.Name}: {ex.Message}");
                                }
                            }
                        }
                    }
                    break;
                case SaveMethod.PlayerPrefs:
                    foreach (FieldInfo field in ScanFieldsFile(file))
                    {
                        PlayerPrefsGetValueByField(field, file);
                    }
                    break;
                default:
                    Debug.LogWarning($"The {Format} is not implement");
                    break;
            }
            WriteLog($"Load ({Format})", file);
        }
        protected override void OnDelete(SaveFile file)
        {
            switch (Format)
            {
                case SaveMethod.Binary:
                    File.Delete(GetPath(file));
                    break;
                case SaveMethod.XML:
                    File.Delete(GetPath(file));
                    break;
                case SaveMethod.JSON:
                    File.Delete(GetPath(file));
                    break;
                case SaveMethod.TEXT:
                    File.Delete(GetPath(file));
                    break;
                case SaveMethod.PlayerPrefs:
                    foreach (FieldInfo field in ScanFieldsFile(file))
                    {
                        PlayerPrefs.DeleteKey(field.Name);
                    }
                    PlayerPrefs.DeleteKey(file.name);
                    break;
                default:
                    Debug.LogWarning($"The {Format} is not implement");
                    break;
            }
            WriteLog($"Delete ({Format})", file);
        }


        private string GetPath(SaveFile file)
        {
            string filename = file.name;
            string filetype = Format == SaveMethod.TEXT ? ".txt" :
            Format == SaveMethod.Binary ? ".RSave" :
            Format == SaveMethod.XML ? ".xml" :
            Format == SaveMethod.JSON ? ".json" : "";

            if (CustomPath)
            {
                return FilePath + "/" + filename + filetype;
            }
            else
            {
                return Application.persistentDataPath + "/" + filename + filetype;
            }
        }
        private FieldInfo[] ScanFieldsFile(SaveFile file)
        {
            List<FieldInfo> Result = new List<FieldInfo>();
            Type type = file.GetType();
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

            FieldInfo[] fields = type.GetFields(flags);

            foreach (FieldInfo field in fields)
            {
                if (field.IsPublic)
                {
                    if (PublicField)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
                if (field.IsFamily)
                {
                    if (ProtectedField)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
                if (field.IsPrivate)
                {
                    if (PrivateField)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
                if (SerializeField)
                {
                    if (field.GetCustomAttribute<SerializeField>() != null)
                    {
                        Result.Add(field);
                        continue;
                    }
                }
            }

            return Result.ToArray();
        }
        private void PlayerPrefsSetValueByField(FieldInfo field, object source)
        {
            string key = field.Name;
            object value = field.GetValue(source);

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
            else if (value is Enum enumvalue)
            {
                PlayerPrefs.SetInt(key, Convert.ToInt32(enumvalue));
            }
            else if (value is byte bytevalue)
            {
                PlayerPrefs.SetInt(key, bytevalue);
            }
            else
            {
                Debug.LogWarning($"Unsupported type for PlayerPrefs: {value?.GetType().Name} (Key: {key})");
            }
        }
        private void PlayerPrefsGetValueByField(FieldInfo field, object source)
        {
            string key = field.Name;

            if (!PlayerPrefs.HasKey(key)) return;

            if (field.FieldType == typeof(int))
            {
                field.SetValue(source, PlayerPrefs.GetInt(key));
            }
            else if (field.FieldType == typeof(float))
            {
                field.SetValue(source, PlayerPrefs.GetFloat(key));
            }
            else if (field.FieldType == typeof(string))
            {
                field.SetValue(source, PlayerPrefs.GetString(key));
            }
            else if (field.FieldType == typeof(bool))
            {
                field.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
            else if (field.FieldType == typeof(Enum))
            {
                field.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
            else if (field.FieldType == typeof(byte))
            {
                field.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
        }

    }
}