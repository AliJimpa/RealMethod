using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace RealMethod
{
    [System.Serializable]
    public enum SaveFormat
    {
        PlayerPrefs = 0,
        TEXT = 1,
        XML = 2,
        JSON = 3,
        Binary = 4,
    }
    public interface ISaveMethod
    {
        SaveFormat Format { get; }
        string Path { get; }
        BindingFlags FieldFlags { get; }
        BindingFlags PropertieFlags { get; }
    }


    public abstract class SaveManager : MonoBehaviour, IGameManager , ISaveSystem
    {
        // Actions
        public event System.Action<IFile> OnLoaded;
        public event System.Action<IFile> OnSaved;
        public event System.Action<IFile> OnDeleted;

#if UNITY_EDITOR
        public byte Logindex { get; private set; }
        public string[] DataLog { get; private set; }
#endif


        // Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        public virtual void InitiateManager(bool AlwaysLoaded)
        {
#if UNITY_EDITOR
            Logindex = 0;
            DataLog = new string[5];
#endif
        }
        public virtual void ResolveService(Service service, bool active)
        {
        }


        // Functions
        public bool IsExist(IFile file)
        {
            if (!Validate(file))
                return false;

            return IsExistFile(file, GetMethod(file));
        }
        public void Save(IFile file)
        {
            if (!Validate(file))
                return;

            if (file.GetObject() is ISave provider)
            {
                OnSave(file, GetMethod(file));
                provider.OnSaved();
                OnSaved?.Invoke(file);

#if UNITY_EDITOR
                WriteLog($"Save ({GetMethod(file)})", file);
#endif
            }
            else
            {
                Debug.LogWarning($"For saving file you should implement ISave interface in {file.GetObject()}");
                return;
            }
        }
        public void Load(IFile file)
        {
            if (!Validate(file))
                return;

            if (file.GetObject() is ISave provider)
            {
                OnLoad(file, GetMethod(file));
                provider.OnLoaded();
                OnLoaded?.Invoke(file);
#if UNITY_EDITOR
                WriteLog($"Save ({GetMethod(file)})", file);
#endif
            }
            else
            {
                Debug.LogWarning($"For loading file you should implement ISave interface in {file.GetObject()}");
                return;
            }
        }
        public void Delete(IFile file)
        {
            if (!Validate(file))
                return;

            OnDelete(file, GetMethod(file));
            OnDeleted?.Invoke(file);
#if UNITY_EDITOR
            WriteLog($"Save ({GetMethod(file)})", file);
#endif
        }

        // Methods
        protected virtual bool Validate(IFile file)
        {
            if (file == null && file.GetObject() != null)
            {
                Debug.LogError("File Does not valid");
                return false;
            }
            return true;
        }
        protected virtual ISaveMethod GetMethod(IFile file)
        {
            if (file.GetObject().HasImplementInterface(out ISaveMethod method))
            {
                return method;
            }
            else
            {
                if (this is ISaveMethod provider)
                {
                    return provider;
                }
                else
                {
                    throw new System.InvalidOperationException($"You Should Implement {typeof(ISaveMethod)}, in SameManager Class for save / load files");
                }
            }
        }


        // Abstract Mehtod
        protected abstract bool IsExistFile(IFile file, ISaveMethod Method);
        protected abstract void OnSave(IFile file, ISaveMethod Method);
        protected abstract void OnLoad(IFile file, ISaveMethod Method);
        protected abstract void OnDelete(IFile file, ISaveMethod Method);
#if UNITY_EDITOR
        private void WriteLog(string message, IFile file)
        {
            if (Application.isPlaying && DataLog != null)
            {
                if (Logindex == 0)
                {
                    DataLog[0] = $"{System.DateTime.Now} -- {file.Name} -- {message}";
                    Logindex++;
                }
                else
                {
                    DataLog[Logindex % DataLog.Length] = $"{System.DateTime.Now} -- {file.Name} -- {message}";
                    Logindex++;
                }
            }
        }
#endif
    }
    public abstract class SaveMethodManager : SaveManager, ISaveMethod
    {
        [Header("SaveMethod")]
        [SerializeField]
        private SaveFormat fromat;
        [SerializeField, ConditionalHideByEnum("fromat", 0)]
        private bool CustomPath = false;
        [SerializeField, ConditionalHide("CustomPath", true, false)]
        private string FilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        [SerializeField, ConditionalShowByEnum("fromat", SaveFormat.PlayerPrefs, SaveFormat.TEXT)]
        private BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        [SerializeField, ConditionalShowByEnum("fromat", SaveFormat.PlayerPrefs, SaveFormat.TEXT)]
        private BindingFlags PropertieFlags = BindingFlags.Default;

        // Implement ISaveMethod Interface
        SaveFormat ISaveMethod.Format => fromat;
        string ISaveMethod.Path => FilePath;
        BindingFlags ISaveMethod.FieldFlags => FieldFlags;
        BindingFlags ISaveMethod.PropertieFlags => PropertieFlags;


        // SaveManager Methods
        protected override bool IsExistFile(IFile file, ISaveMethod Method)
        {
            switch (Method.Format)
            {
                case SaveFormat.Binary:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.XML:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.JSON:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.TEXT:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.PlayerPrefs:
                    return PlayerPrefs.HasKey(file.Name);
                default:
                    Debug.LogWarning($"The {Method.Format} is not implement");
                    return false;
            }
        }
        protected override void OnSave(IFile file, ISaveMethod Method)
        {
            object fileObject = file.GetObject();

            switch (Method.Format)
            {
                case SaveFormat.Binary:
                    BinaryFormatter bf = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Suppress BinaryFormatter warning (only use in trusted context)
                    using (var stream = new FileStream(GetPath(file, Method), FileMode.Create))
                    {
                        try
                        {
                            bf.Serialize(stream, fileObject);
                        }
                        catch (System.Exception e)
                        {
                            Debug.LogError($"Failed to serialize {file.Name} to {GetPath(file, Method)}: {e}");
                            return;
                        }
                    }
#pragma warning restore SYSLIB0011
                    break;
                case SaveFormat.XML:
                    var xml = new XmlSerializer(fileObject.GetType());
                    using (var stream = new FileStream(GetPath(file, Method), FileMode.Create))
                    {
                        xml.Serialize(stream, fileObject);
                    }
                    break;
                case SaveFormat.JSON:
                    string jsoncontent = JsonUtility.ToJson(fileObject, true);
                    File.WriteAllText(GetPath(file, Method), jsoncontent);
                    break;
                case SaveFormat.TEXT:
                    List<string> lines = new List<string>();
                    FieldInfo[] T_fields = fileObject.GetFields(Method.FieldFlags);
                    PropertyInfo[] T_properties = fileObject.GetProperties(Method.PropertieFlags);
                    foreach (FieldInfo field in T_fields)
                    {
                        lines.Add($"{field.Name}={field.GetValue(file)}");
                    }
                    foreach (PropertyInfo property in T_properties)
                    {
                        lines.Add($"{property.Name}={property.GetValue(file)}");
                    }
                    File.WriteAllLines(GetPath(file, Method), lines);
                    break;
                case SaveFormat.PlayerPrefs:
                    FieldInfo[] PP_fields = fileObject.GetFields(Method.FieldFlags);
                    PropertyInfo[] PP_properties = fileObject.GetProperties(Method.PropertieFlags);
                    foreach (FieldInfo field in PP_fields)
                    {
                        PlayerPrefsSetValueByField(field, fileObject);
                    }
                    foreach (PropertyInfo property in PP_properties)
                    {
                        PlayerPrefsSetValueByProperty(property, fileObject);
                    }
                    PlayerPrefs.SetString(file.Name, System.DateTime.Now.ToString());
                    PlayerPrefs.Save();
                    break;
                default:
                    Debug.LogWarning($"The {Method} is not implement");
                    break;
            }
        }
        protected override void OnLoad(IFile file, ISaveMethod Method)
        {
            object fileObject = file.GetObject();

            switch (Method.Format)
            {
                case SaveFormat.Binary:
                    var bf = new BinaryFormatter();
#pragma warning disable SYSLIB0011
                    using (var stream = new FileStream(GetPath(file, Method), FileMode.Open))
                    {
                        fileObject = bf.Deserialize(stream);

                    }
#pragma warning restore SYSLIB0011
                    break;
                case SaveFormat.XML:
                    var xml = new XmlSerializer(file.GetType());
                    using (var stream = new FileStream(GetPath(file, Method), FileMode.Open))
                    {
                        fileObject = xml.Deserialize(stream);
                    }
                    break;
                case SaveFormat.JSON:
                    string jsoncontent;
                    try
                    {
                        jsoncontent = File.ReadAllText(GetPath(file, Method));
                        JsonUtility.FromJsonOverwrite(jsoncontent, fileObject);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"Failed to read from {GetPath(file, Method)} with exception {e}");
                        return;
                    }
                    break;
                case SaveFormat.TEXT:
                    FieldInfo[] T_fields = fileObject.GetFields(Method.FieldFlags);
                    PropertyInfo[] T_properties = fileObject.GetProperties(Method.PropertieFlags);
                    foreach (string line in File.ReadLines(GetPath(file, Method)))
                    {
                        string[] parts = line.Split("=");
                        if (parts.Length < 2) continue;
                        string lineName = parts[0];
                        string lineValue = parts[1];
                        foreach (FieldInfo info in T_fields)
                        {
                            if (info.Name == lineName)
                            {
                                object value = null;
                                System.Type type = info.FieldType;
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
                                        value = System.Enum.Parse(type, lineValue);
                                    else if (type == typeof(byte))
                                        value = byte.Parse(lineValue);
                                    else
                                        Debug.LogWarning($"Unsupported type: {type.Name}");
                                    info.SetValue(file, value);
                                }
                                catch (System.Exception ex)
                                {
                                    Debug.LogWarning($"Failed to set value for {info.Name}: {ex.Message}");
                                }
                            }
                        }
                        foreach (PropertyInfo property in T_properties)
                        {
                            if (property.Name == lineName)
                            {
                                object value = null;
                                System.Type type = property.PropertyType;
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
                                        value = System.Enum.Parse(type, lineValue);
                                    else if (type == typeof(byte))
                                        value = byte.Parse(lineValue);
                                    else
                                        Debug.LogWarning($"Unsupported type: {type.Name}");
                                    property.SetValue(file, value);
                                }
                                catch (System.Exception ex)
                                {
                                    Debug.LogWarning($"Failed to set value for {property.Name}: {ex.Message}");
                                }
                            }
                        }
                    }
                    break;
                case SaveFormat.PlayerPrefs:
                    FieldInfo[] PP_fields = fileObject.GetFields(Method.FieldFlags);
                    PropertyInfo[] PP_properties = fileObject.GetProperties(Method.PropertieFlags);
                    foreach (FieldInfo field in PP_fields)
                    {
                        PlayerPrefsGetValueByField(field, fileObject);
                    }
                    foreach (PropertyInfo property in PP_properties)
                    {
                        PlayerPrefsGetValueByProperty(property, fileObject);
                    }
                    break;
                default:
                    Debug.LogWarning($"The {Method} is not implement");
                    break;
            }
        }
        protected override void OnDelete(IFile file, ISaveMethod Method)
        {
            object fileObject = file.GetObject();

            switch (Method.Format)
            {
                case SaveFormat.Binary:
                    File.Delete(GetPath(file, Method));
                    break;
                case SaveFormat.XML:
                    File.Delete(GetPath(file, Method));
                    break;
                case SaveFormat.JSON:
                    File.Delete(GetPath(file, Method));
                    break;
                case SaveFormat.TEXT:
                    File.Delete(GetPath(file, Method));
                    break;
                case SaveFormat.PlayerPrefs:
                    FieldInfo[] PP_fields = fileObject.GetFields(Method.FieldFlags);
                    PropertyInfo[] PP_properties = fileObject.GetProperties(Method.PropertieFlags);
                    foreach (FieldInfo field in PP_fields)
                    {
                        PlayerPrefs.DeleteKey(field.Name);
                    }
                    foreach (PropertyInfo property in PP_properties)
                    {
                        PlayerPrefs.DeleteKey(property.Name);
                    }
                    break;
                default:
                    Debug.LogWarning($"The {Method} is not implement");
                    break;
            }
        }

        // Methods
        protected virtual string GetPath(IFile file, ISaveMethod method)
        {
            string filename = file.Name;
            string filetype = method.Format == SaveFormat.TEXT ? ".txt" :
            method.Format == SaveFormat.Binary ? ".RSave" :
            method.Format == SaveFormat.XML ? ".xml" :
            method.Format == SaveFormat.JSON ? ".json" : "";

            if (CustomPath)
            {
                return method.Path + "/" + filename + filetype;
            }
            else
            {
                return Application.persistentDataPath + "/" + filename + filetype;
            }
        }

        // Private Method
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
            else if (value is System.Enum enumvalue)
            {
                PlayerPrefs.SetInt(key, System.Convert.ToInt32(enumvalue));
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
        private void PlayerPrefsSetValueByProperty(PropertyInfo property, object source)
        {
            string key = property.Name;
            object value = property.GetValue(source);

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
            else if (value is System.Enum enumvalue)
            {
                PlayerPrefs.SetInt(key, System.Convert.ToInt32(enumvalue));
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
            else if (field.FieldType == typeof(System.Enum))
            {
                field.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
            else if (field.FieldType == typeof(byte))
            {
                field.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
        }
        private void PlayerPrefsGetValueByProperty(PropertyInfo property, object source)
        {
            string key = property.Name;

            if (!PlayerPrefs.HasKey(key)) return;

            if (property.PropertyType == typeof(int))
            {
                property.SetValue(source, PlayerPrefs.GetInt(key));
            }
            else if (property.PropertyType == typeof(float))
            {
                property.SetValue(source, PlayerPrefs.GetFloat(key));
            }
            else if (property.PropertyType == typeof(string))
            {
                property.SetValue(source, PlayerPrefs.GetString(key));
            }
            else if (property.PropertyType == typeof(bool))
            {
                property.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
            else if (property.PropertyType == typeof(System.Enum))
            {
                property.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
            else if (property.PropertyType == typeof(byte))
            {
                property.SetValue(source, PlayerPrefs.GetInt(key) == 1);
            }
        }

    }
    public abstract class SaveLoadManager : SaveMethodManager
    {
        [Header("SaveSetting")]
        [SerializeField]
        private bool LoadOnInitiate = true;
        [SerializeField, ConditionalHide("LoadOnInitiate", true, false)]
        private SaveFile[] DefaultFile = new SaveFile[0];

        // IGameManager
        public override void InitiateManager(bool AlwaysLoaded)
        {
            base.InitiateManager(AlwaysLoaded);

            if (LoadOnInitiate)
            {
                foreach (var file in DefaultFile)
                {
                    Load(file);
                }
            }
        }
    }

}