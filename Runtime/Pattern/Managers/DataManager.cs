using UnityEngine;
using System;
using System.Reflection;


namespace RealMethod
{
    public abstract class DataManager : MonoBehaviour, IGameManager
    {
        [Serializable]
        public enum SaveMethod
        {
            Binary,
            XML,
            JSON,
            YAML,
            PlayerPrefs,
            Custom,
        }


        [Header("Setting")]
        [SerializeField]
        private SaveMethod SavingMethod = SaveMethod.PlayerPrefs;
        [SerializeField]
        private bool AutoLoad = true;
        [Header("File")]
        [SerializeField]
        private SaveFile File;


        public Action<SaveFile> OnFileLoaded;
        public Action<SaveFile> OnFileSaved;



        public MonoBehaviour GetManagerClass()
        {
            return this;
        }
        public void InitiateManager(bool AlwaysLoaded)
        {
            if (File)
            {
                File.OnActive(this);
                if (AutoLoad)
                {
                    if (IsExistFile())
                        LoadFile();
                }
            }
        }
        public void InitiateService(Service service)
        {
        }


        [ContextMenu("Save")]
        public void SaveFile()
        {
            if (File == null)
            {
                Debug.LogError("File is not valid");
                return;
            }

            switch (SavingMethod)
            {
                case SaveMethod.Binary:
                    SaveBinary(File);
                    break;
                case SaveMethod.XML:
                    SaveXML(File);
                    break;
                case SaveMethod.JSON:
                    SaveJson(File);
                    break;
                case SaveMethod.YAML:
                    SaveYaml(File);
                    break;
                case SaveMethod.PlayerPrefs:
                    SavePrefs(File);
                    break;
                case SaveMethod.Custom:
                    SaveCustom(File);
                    break;
            }
            File.OnSave();
            OnFileSaved?.Invoke(File);
        }
        [ContextMenu("Load")]
        public void LoadFile()
        {
            if (File == null)
            {
                Debug.LogError("File Does not valid");
                return;
            }

            switch (SavingMethod)
            {
                case SaveMethod.Binary:
                    LoadBinary(File);
                    break;
                case SaveMethod.XML:
                    LoadXML(File);
                    break;
                case SaveMethod.JSON:
                    LoadJson(File);
                    break;
                case SaveMethod.YAML:
                    LoadYaml(File);
                    break;
                case SaveMethod.PlayerPrefs:
                    LoadPrefs(File);
                    break;
                case SaveMethod.Custom:
                    LoadCustom(File);
                    break;
            }
            File.OnLoad();
            OnFileLoaded?.Invoke(File);
        }
        public bool IsExistFile()
        {
            if (File == null)
            {
                Debug.LogError("File Does not valid");
                return false;
            }

            switch (SavingMethod)
            {
                case SaveMethod.Binary:
                    return IsExistBinary(File);
                case SaveMethod.XML:
                    return IsExistXML(File);
                case SaveMethod.JSON:
                    return IsExistJson(File);
                case SaveMethod.YAML:
                    return IsExistYaml(File);
                case SaveMethod.PlayerPrefs:
                    return IsExistPrefs(File);
                case SaveMethod.Custom:
                    return IsExistCustom(File);
                default:
                    return false;
            }
        }
        [ContextMenu("Delete")]
        public void DeleteFile()
        {
            if (File == null)
            {
                Debug.LogError("File Does not valid");
                return;
            }

            switch (SavingMethod)
            {
                case SaveMethod.Binary:
                    DeleteBinary(File);
                    break;
                case SaveMethod.XML:
                    DeleteXML(File);
                    break;
                case SaveMethod.JSON:
                    DeleteJson(File);
                    break;
                case SaveMethod.YAML:
                    DeleteYaml(File);
                    break;
                case SaveMethod.PlayerPrefs:
                    DeletePrefs(File);
                    break;
                case SaveMethod.Custom:
                    DeleteCustom(File);
                    break;
            }
            File.OnDelete();
        }

        public bool SetFile(SaveFile file)
        {
            if (file)
            {
                if (File != file)
                {
                    File?.OnDiactive(this);
                    File = file;
                    File.OnActive(this);
                }
                return true;
            }
            else
            {
                Debug.LogError("File Does not valid");
                return false;
            }
        }
        public void SaveFile(SaveFile file)
        {
            if (SetFile(file))
                SaveFile();
        }
        public void LoadFile(SaveFile file)
        {
            if (SetFile(file))
                LoadFile();
        }
        public void DeleteFile(SaveFile file)
        {
            if (SetFile(file))
                DeleteFile();
        }
        public string GetFileName()
        {
            if (File)
            {
                return File.name;
            }
            else
            {
                return "No File";
            }
        }

        // Binary
        protected virtual bool IsExistBinary(SaveFile file)
        {
            return false;
        }
        protected virtual void DeleteBinary(SaveFile file)
        {

        }
        protected virtual void SaveBinary(SaveFile file)
        {

        }
        protected virtual void LoadBinary(SaveFile file)
        {

        }
        // XML
        protected virtual bool IsExistXML(SaveFile file)
        {
            return false;
        }
        protected virtual void DeleteXML(SaveFile file)
        {

        }
        protected virtual void SaveXML(SaveFile file)
        {

        }
        protected virtual void LoadXML(SaveFile file)
        {

        }
        // JSON
        protected virtual bool IsExistJson(SaveFile file)
        {
            return false;
        }
        protected virtual void DeleteJson(SaveFile file)
        {

        }
        protected virtual void SaveJson(SaveFile file)
        {
            //Mustard.FileManager.WriteToFile(Filename, JsonUtility.ToJson(File));
        }
        protected virtual void LoadJson(SaveFile file)
        {
            //slot = new SaveFile();
            //Mustard.FileManager.LoadFromFile(Filename, out var json);
            //JsonUtility.FromJsonOverwrite(json, slot);
        }
        //YAML
        protected virtual bool IsExistYaml(SaveFile file)
        {
            return false;
        }
        protected virtual void DeleteYaml(SaveFile file)
        {

        }
        protected virtual void SaveYaml(SaveFile file)
        {

        }
        protected virtual void LoadYaml(SaveFile file)
        {

        }
        // PlayerPrefs
        protected virtual bool IsExistPrefs(SaveFile file)
        {
            if (PlayerPrefs.HasKey("LastSave"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        protected virtual void DeletePrefs(SaveFile file)
        {
            PlayerPrefs.DeleteAll();
        }
        protected virtual void SavePrefs(SaveFile file)
        {
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
        }
        protected virtual void LoadPrefs(SaveFile file)
        {
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
        }
        // Custom
        protected virtual bool IsExistCustom(SaveFile file)
        {
            return false;
        }
        protected virtual void DeleteCustom(SaveFile file)
        {

        }
        protected virtual void SaveCustom(SaveFile file)
        {

        }
        protected virtual void LoadCustom(SaveFile file)
        {

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


    public abstract class SaveFile : ScriptableObject
    {
        public Action<SaveFile> OnModify;
        public abstract void OnActive(DataManager manager);
        public abstract void OnLoad();
        public abstract void OnSave();
        public abstract void OnDelete();
        public abstract void OnClear();
        public abstract void OnDiactive(DataManager manager);

        [ContextMenu("Clear")]
        private void Clear_Editor()
        {
            OnClear();
        }

    }
}