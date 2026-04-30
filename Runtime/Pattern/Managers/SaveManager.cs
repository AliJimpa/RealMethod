using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using UnityEngine;
using System.Linq;
using System.ComponentModel;

namespace RealMethod
{
    [System.Serializable]
    public enum SaveFileStructure
    {
        [DescriptionEnum("Always read/write one fixed file")]
        SingleFile,
        [DescriptionEnum("Each save item is saved separately")]
        MultiFile,
        [DescriptionEnum("Save multiple items but merge them into one output file")]
        MergedFile
    }
    [System.Serializable]
    public enum SaveFormat
    {
        [DescriptionEnum("Didn't Store any data just call OnSave&OnLoad Event")]
        None = 0,
        PlayerPrefs = 1,
        TEXT = 2,
        XML = 3,
        JSON = 4,
        Binary = 5,
        Custom = 6,
    }
    public interface ISaveMethod
    {
        SaveFormat Format { get; }
        string Path { get; }
        BindingFlags FieldFlags { get; }
        BindingFlags PropertieFlags { get; }
    }
    public interface IMergeFile : ISaveFile
    {
        void Write(string fileName, string variableName, System.Type variableType, object variableValue);
        object Read(string fileName, string variableName, System.Type variableType);
    }



    public abstract class SaveManager : MonoBehaviour, IGameManager, ISaveSystem
    {
        [Header("Mode")]
        [SerializeField]
        protected SaveFileStructure Mode = SaveFileStructure.MultiFile;
        [Space]
        [SerializeField, ConditionalShowByEnum("Mode", SaveFileStructure.MergedFile)]
        private BindingFlags MergedFieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        [SerializeField, ConditionalShowByEnum("Mode", SaveFileStructure.MergedFile)]
        private BindingFlags MergedPropertieFlags = BindingFlags.Default;
        public bool IsInSingleFile => Mode == SaveFileStructure.SingleFile;
        // Actions
        public event System.Action<IFile> OnLoaded;
        public event System.Action<IFile> OnSaved;
        public event System.Action<IFile> OnDeleted;





        // Implement IGameManager Interface
        public virtual void InitiateManager(Scope owner)
        {
            if (Mode != SaveFileStructure.MultiFile)
            {
                IFile provider = CreateMainFile();
                ISaveMethod method = GetMethod(provider);
                if (method.Format == SaveFormat.None)
                {
                    OnLoad(provider, method);
                }
                else
                {
                    if (IsExistFile(provider, method))
                    {
                        OnLoad(provider, method);
                    }
                }
            }
        }


        // Implement ISaveSystem Interface
        /// <summary>
        /// The file that created by savesystem for merging all file selected in sytem to one file or SingleFile
        /// </summary>
        public IFile MainSaveFile
        {
            get
            {
                if (Mode == SaveFileStructure.MultiFile)
                {
                    Debug.LogWarning("MainSaveFile Just created in SingleFile & MergedFile");
                    return null;
                }
                else
                {
                    return GetMainFile<IFile>();
                }
            }
        }
        public bool IsExist(IFile file)
        {
            if (!CanContinue("check isExist"))
                return false;

            if (!Validate(file))
                return false;

            return IsExistFile(file, GetMethod(file));
        }
        public void Save(IFile file)
        {
            if (!CanContinue("Save"))
                return;

            if (!Validate(file))
                return;

            try
            {
                OnSave(file, GetMethod(file));
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
            file.FileObject.InvokeSaveEvent();
            OnSaved?.Invoke(file);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            WriteLog($"Save({GetMethod(file).Format}) Class({file.FileObject.GetType()})");
#endif
        }
        public void Load(IFile file)
        {
            if (!CanContinue("Load"))
                return;

            if (!Validate(file))
                return;

            try
            {
                OnLoad(file, GetMethod(file));
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
            file.FileObject.InvokeLoadEvent();
            OnLoaded?.Invoke(file);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            WriteLog($"Load({GetMethod(file).Format}) Class({file.FileObject.GetType()})");
#endif
        }
        public void Delete(IFile file)
        {
            if (!Validate(file))
                return;

            try
            {
                OnDelete(file, GetMethod(file));
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
            OnDeleted?.Invoke(file);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            WriteLog($"Delete({GetMethod(file).Format}) Class({file.FileObject.GetType()})");
#endif
        }
        [ContextMenu("Save")]
        void ISaveSystem.SaveAll()
        {
            IFile[] files = GetAllFiles();
            if (files != null)
            {
                switch (Mode)
                {
                    case SaveFileStructure.SingleFile:
                        if (!Validate(MainSaveFile))
                            return;

                        try
                        {
                            OnSave(MainSaveFile, GetMethod(MainSaveFile));
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(ex);
                            return;
                        }
                        MainSaveFile.FileObject.InvokeSaveEvent();
                        OnSaved?.Invoke(MainSaveFile);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        WriteLog($"Save[SingleFile]({GetMethod(MainSaveFile).Format}) Class({MainSaveFile.FileObject.GetType()})");
#endif
                        break;
                    case SaveFileStructure.MultiFile:
                        foreach (var file in files)
                        {
                            Save(file);
                        }
                        break;
                    case SaveFileStructure.MergedFile:
                        foreach (var file in files)
                        {
                            WriteToMergeFile(file, MergedFieldFlags, MergedPropertieFlags);
                        }


                        if (!Validate(MainSaveFile))
                            return;

                        try
                        {
                            OnSave(MainSaveFile, GetMethod(MainSaveFile));
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(ex);
                            return;
                        }
                        MainSaveFile.FileObject.InvokeSaveEvent();
                        OnSaved?.Invoke(MainSaveFile);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        WriteLog($"Save[MergedFile]({GetMethod(MainSaveFile).Format}) Class({MainSaveFile.FileObject.GetType()})");
#endif
                        break;
                }
            }
        }
        [ContextMenu("Load")]
        void ISaveSystem.LoadAll()
        {
            IFile[] files = GetAllFiles();
            if (files != null)
            {
                switch (Mode)
                {
                    case SaveFileStructure.SingleFile:
                        if (!Validate(MainSaveFile))
                            return;

                        try
                        {
                            OnLoad(MainSaveFile, GetMethod(MainSaveFile));
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(ex);
                            return;
                        }
                        MainSaveFile.FileObject.InvokeLoadEvent();
                        OnLoaded?.Invoke(MainSaveFile);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        WriteLog($"Load({GetMethod(MainSaveFile).Format}) Class({MainSaveFile.FileObject.GetType()})");
#endif
                        break;
                    case SaveFileStructure.MultiFile:
                        foreach (var file in files)
                        {
                            Save(file);
                        }
                        break;
                    case SaveFileStructure.MergedFile:
                        foreach (var file in files)
                        {
                            ReadFromMergeFile(file, MergedFieldFlags, MergedPropertieFlags);
                        }


                        if (!Validate(MainSaveFile))
                            return;

                        try
                        {
                            OnLoad(MainSaveFile, GetMethod(MainSaveFile));
                        }
                        catch (System.Exception ex)
                        {
                            Debug.LogError(ex);
                            return;
                        }
                        MainSaveFile.FileObject.InvokeLoadEvent();
                        OnLoaded?.Invoke(MainSaveFile);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        WriteLog($"Load({GetMethod(MainSaveFile).Format}) Class({MainSaveFile.FileObject.GetType()})");
#endif
                        break;
                }
            }
        }
        /// <summary>
        /// use for adding file to filelist in savemsystem.
        /// if you want to use saveall or geting file with name
        /// </summary>
        /// <param name="file">target file you want adding</param>
        /// <returns>return true if can add</returns>
        public abstract bool AddFile(IFile file);
        /// <summary>
        /// use for removing file to filelist in savesystem
        /// </summary>
        /// <param name="file">target file you want removing</param>
        /// <returns>return true if can remove</returns>
        public abstract bool RemoveFile(IFile file);
        /// <summary>
        /// Determines whether the specified file already exists in the file list.
        /// Checks by object reference unless IFile implementations override equality.
        /// </summary>
        /// <param name="file">The file instance to check.</param>
        /// <returns>True if the file is found in the list; otherwise false.</returns>
        public abstract bool HasFile(IFile file);


        // Methods
        protected virtual bool CanContinue(string message)
        {
            if (Mode == SaveFileStructure.MultiFile)
            {
                return true;
            }
            Debug.LogError($"You can't {message} when SaveMode is {Mode}");
            return false;
        }
        protected virtual bool Validate(IFile file)
        {
            if (file == null && file.FileObject != null)
            {
                Debug.LogError("File Does not valid");
                return false;
            }
            return true;
        }
        protected virtual ISaveMethod GetMethod(IFile file)
        {
            if (file != null && file.FileObject.HasImplementInterface(out ISaveMethod method))
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
        protected virtual void WriteToMergeFile(IFile file, BindingFlags fieldFlags, BindingFlags propertyFlags)
        {
            object FileObject = file.FileObject;
            IMergeFile mergefile = GetMainFile<IMergeFile>();
            foreach (var field in FileObject.GetFields(fieldFlags))
            {
                mergefile.Write(file.FileName, field.Name, field.FieldType, field.GetValue(FileObject));
            }
            foreach (var propery in FileObject.GetProperties(propertyFlags))
            {
                mergefile.Write(file.FileName, propery.Name, propery.PropertyType, propery.GetValue(FileObject));
            }
        }
        protected virtual void ReadFromMergeFile(IFile file, BindingFlags fieldFlags, BindingFlags propertyFlags)
        {
            object FileObject = file.FileObject;
            IMergeFile mergefile = GetMainFile<IMergeFile>();
            foreach (var field in FileObject.GetFields(fieldFlags))
            {
                field.SetValue(FileObject, mergefile.Read(file.FileName, field.Name, field.FieldType));
            }
            foreach (var propery in FileObject.GetProperties(propertyFlags))
            {
                propery.SetValue(FileObject, mergefile.Read(file.FileName, propery.Name, propery.PropertyType));
            }
        }


        // Abstract Mehtod
        protected abstract bool IsExistFile(IFile file, ISaveMethod Method);
        protected abstract void OnSave(IFile file, ISaveMethod Method);
        protected abstract void OnLoad(IFile file, ISaveMethod Method);
        protected abstract void OnDelete(IFile file, ISaveMethod Method);
        protected abstract IFile CreateMainFile();
        protected abstract T GetMainFile<T>() where T : IFile;
        public abstract IFile[] GetAllFiles();


#if UNITY_EDITOR || DEVELOPMENT_BUILD
        protected virtual void WriteLog(string message)
        {
            Debug.Log($"{System.DateTime.Now} -- {message}");
        }
#endif
    }
    public abstract class SaveManager_Method : SaveManager, ISaveMethod
    {
        protected enum SaveState
        {
            IsExist = 0,
            Save = 1,
            Load = 2,
            Delete = 3
        }
        [Header("SaveMethod")]
        [SerializeField]
        private SaveFormat fromat;
        [SerializeField, ConditionalHideByEnum("fromat", SaveFormat.None, SaveFormat.PlayerPrefs)]
        private bool CustomPath = false;
        [SerializeField, ConditionalHideByEnum("fromat", SaveFormat.None, SaveFormat.PlayerPrefs), ConditionalHide("CustomPath", true, false)]
        private string FilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        [SerializeField, ConditionalShowByEnum("fromat", SaveFormat.PlayerPrefs, SaveFormat.TEXT, SaveFormat.Custom)]
        private BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        [SerializeField, ConditionalShowByEnum("fromat", SaveFormat.PlayerPrefs, SaveFormat.TEXT, SaveFormat.Custom)]
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
                case SaveFormat.None:
                    Debug.LogWarning("SaveSystem can't check IsExistFile with None format");
                    return false;
                case SaveFormat.Binary:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.XML:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.JSON:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.TEXT:
                    return File.Exists(GetPath(file, Method));
                case SaveFormat.PlayerPrefs:
                    return PlayerPrefs.HasKey(file.FileName);
                case SaveFormat.Custom:
                    return CustomSavefile(file, Method, SaveState.IsExist);
                default:
                    Debug.LogWarning($"The {Method.Format} is not implement");
                    return false;
            }
        }
        protected override void OnSave(IFile file, ISaveMethod Method)
        {
            object fileObject = file.FileObject;

            switch (Method.Format)
            {
                case SaveFormat.None:
                    if (fileObject is not ISave)
                    {
                        Debug.LogError($"Your file({fileObject}) with name({file.FileName}) should implement ISave Interface");
                    }
                    break;
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
                            Debug.LogError($"Failed to serialize {file.FileName} to {GetPath(file, Method)}: {e}");
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
                        Set_PlayerPrefsInfo(field, fileObject);
                    }
                    foreach (PropertyInfo property in PP_properties)
                    {
                        Set_PlayerPrefsInfo(property, fileObject);
                    }
                    PlayerPrefs.SetString(file.FileName, System.DateTime.Now.ToString());
                    PlayerPrefs.Save();
                    break;
                case SaveFormat.Custom:
                    CustomSavefile(file, Method, SaveState.Save);
                    break;
                default:
                    Debug.LogWarning($"The {Method.Format} is not implement");
                    break;
            }
        }
        protected override void OnLoad(IFile file, ISaveMethod Method)
        {
            object fileObject = file.FileObject;

            switch (Method.Format)
            {
                case SaveFormat.None:
                    if (fileObject is not ISave)
                    {
                        Debug.LogError($"Your file({fileObject}) with name({file.FileName}) should implement ISave Interface");
                    }
                    break;
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
                        Get_PlayerPrefsInfo(field, fileObject);
                    }
                    foreach (PropertyInfo property in PP_properties)
                    {
                        Get_PlayerPrefsInfo(property, fileObject);
                    }
                    break;
                case SaveFormat.Custom:
                    CustomSavefile(file, Method, SaveState.Load);
                    break;
                default:
                    Debug.LogWarning($"The {Method.Format} is not implement");
                    break;
            }
        }
        protected override void OnDelete(IFile file, ISaveMethod Method)
        {
            object fileObject = file.FileObject;

            switch (Method.Format)
            {
                case SaveFormat.None:
                    Debug.LogWarning("SaveSystem can't delete with None format");
                    break;
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
                case SaveFormat.Custom:
                    CustomSavefile(file, Method, SaveState.Delete);
                    break;
                default:
                    Debug.LogWarning($"The {Method.Format} is not implement");
                    break;
            }
        }

        // Methods
        protected virtual string GetPath(IFile file, ISaveMethod method)
        {
            string filename = file.FileName;
            string filetype = GetFileType(method);

            if (CustomPath)
            {
                return method.Path + "/" + filename + filetype;
            }
            else
            {
                return Application.persistentDataPath + "/" + filename + filetype;
            }
        }
        protected virtual string GetFileType(ISaveMethod method)
        {
            return method.Format == SaveFormat.TEXT ? ".txt" :
            method.Format == SaveFormat.Binary ? ".RSave" :
            method.Format == SaveFormat.XML ? ".xml" :
            method.Format == SaveFormat.JSON ? ".json" : "";
        }

        // Private Method
        private void Set_PlayerPrefsInfo(MemberInfo info, object source)
        {
            string key = info.Name;
            object value = GetValueInfo(info, source);

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
            else if (value is Vector2 v2Value)
            {
                RM_Save.SetVector2(key, v2Value);
            }
            else if (value is Vector3 v3Value)
            {
                RM_Save.SetVector3(key, v3Value);
            }
            else if (value is Quaternion quatValue)
            {
                RM_Save.SetQuaternion(key, quatValue);
            }
            else if (value is Transform transValue)
            {
                RM_Save.SetTransform(key, transValue);
            }
            else if (value is UniqueAsset asset)
            {
                RM_Save.SetAsset(key, asset);
            }
            else if (value is IList<int> Intlist)
            {
                RM_Save.SetArray(key, Intlist.ToArray());
            }
            else if (value is IList<float> FloatList)
            {
                RM_Save.SetArray(key, FloatList.ToArray());
            }
            else if (value is IList<string> StringList)
            {
                RM_Save.SetArray(key, StringList.ToArray());
            }
            else if (value is IList<bool> BoolList)
            {
                RM_Save.SetArray(key, BoolList.ToArray());
            }
            else if (value is IList<System.Enum> EnumList)
            {
                RM_Save.SetArray(key, EnumList.ToArray());
            }
            else if (value is IList<byte> ByteList)
            {
                RM_Save.SetArray(key, ByteList.ToArray());
            }
            else if (value is IList<Vector3> V3List)
            {
                RM_Save.SetArray(key, V3List.ToArray());
            }
            else if (value is IList<Vector2> V2List)
            {
                RM_Save.SetArray(key, V2List.ToArray());
            }
            else if (value is int[] IntArray)
            {
                RM_Save.SetArray(key, IntArray);
            }
            else if (value is float[] FloatArray)
            {
                RM_Save.SetArray(key, FloatArray);
            }
            else if (value is string[] StringArray)
            {
                RM_Save.SetArray(key, StringArray);
            }
            else if (value is bool[] BoolArray)
            {
                RM_Save.SetArray(key, BoolArray);
            }
            else if (value is System.Enum[] EnumArray)
            {
                RM_Save.SetArray(key, EnumArray);
            }
            else if (value is byte[] ByteArray)
            {
                RM_Save.SetArray(key, ByteArray);
            }
            else if (value is Vector3[] V3Array)
            {
                RM_Save.SetArray(key, V3Array);
            }
            else if (value is Vector2[] V2Array)
            {
                RM_Save.SetArray(key, V2Array);
            }
            else
            {
                Debug.LogWarning($"Unsupported type for PlayerPrefs: {value?.GetType().Name} (Key: {key})");
            }

        }
        private void Get_PlayerPrefsInfo(MemberInfo info, object source)
        {
            string key = info.Name;
            System.Type type = GetInfoType(info);

            if (!PlayerPrefs.HasKey(key)) return;

            if (type == typeof(int))
            {
                SetValueInfo(info, source, PlayerPrefs.GetInt(key));
            }
            else if (type == typeof(float))
            {
                SetValueInfo(info, source, PlayerPrefs.GetFloat(key));
            }
            else if (type == typeof(string))
            {
                SetValueInfo(info, source, PlayerPrefs.GetString(key));
            }
            else if (type == typeof(bool))
            {
                SetValueInfo(info, source, PlayerPrefs.GetInt(key) == 1);
            }
            else if (type == typeof(System.Enum))
            {
                SetValueInfo(info, source, PlayerPrefs.GetInt(key));
            }
            else if (type == typeof(byte))
            {
                SetValueInfo(info, source, PlayerPrefs.GetInt(key));
            }
            else if (type == typeof(Vector2))
            {
                SetValueInfo(info, source, RM_Save.GetVector2(key));
            }
            else if (type == typeof(Vector3))
            {
                SetValueInfo(info, source, RM_Save.GetVector3(key));
            }
            else if (type == typeof(Quaternion))
            {
                SetValueInfo(info, source, RM_Save.GetQuaternion(key));
            }
            else if (type == typeof(Transform))
            {
                Transform t = (Transform)GetValueInfo(info, source);
                RM_Save.GetTransform(key, t);
            }
            else if (type == typeof(UniqueAsset))
            {
                SetValueInfo(info, source, RM_Save.GetAsset<UniqueAsset>(key));
            }
            else if (type == typeof(List<int>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<int>(key).ToList());
            }
            else if (type == typeof(List<float>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<float>(key).ToList());
            }
            else if (type == typeof(List<string>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<string>(key).ToList());
            }
            else if (type == typeof(List<bool>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<bool>(key).ToList());
            }
            else if (type == typeof(List<System.Enum>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<System.Enum>(key).ToList());
            }
            else if (type == typeof(List<byte>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<byte>(key).ToList());
            }
            else if (type == typeof(List<Vector3>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<Vector3>(key).ToList());
            }
            else if (type == typeof(List<Vector2>))
            {
                SetValueInfo(info, source, RM_Save.GetArray<Vector2>(key).ToList());
            }
            else if (type == typeof(int[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<int>(key));
            }
            else if (type == typeof(float[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<float>(key));
            }
            else if (type == typeof(string[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<string>(key));
            }
            else if (type == typeof(bool[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<bool>(key));
            }
            else if (type == typeof(System.Enum[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<System.Enum>(key));
            }
            else if (type == typeof(byte[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<byte>(key));
            }
            else if (type == typeof(Vector2[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<Vector2>(key));
            }
            else if (type == typeof(Vector3[]))
            {
                SetValueInfo(info, source, RM_Save.GetArray<Vector3>(key));
            }
        }
        private object GetValueInfo(MemberInfo info, object source)
        {
            if (info is FieldInfo field)
            {
                return field.GetValue(source);
            }
            else if (info is PropertyInfo property)
            {
                return property.GetValue(source);
            }
            else
            {
                Debug.LogError("Something wrong MemeberInfo has not correct valid");
                return null;
            }
        }
        private void SetValueInfo<T>(MemberInfo info, object source, T value)
        {
            if (info is FieldInfo field)
            {
                field.SetValue(source, value);
            }
            else if (info is PropertyInfo property)
            {
                property.SetValue(source, value);
            }
            else
            {
                Debug.LogError("Something wrong MemeberInfo has not correct valid");
                return;
            }
        }
        private System.Type GetInfoType(MemberInfo info)
        {
            if (info is FieldInfo field)
            {
                return field.FieldType;
            }
            else if (info is PropertyInfo property)
            {
                return property.PropertyType;
            }
            else
            {
                Debug.LogError("Something wrong MemeberInfo has not correct valid");
                return null;
            }
        }


        // Abstract Mehtod
        protected abstract bool CustomSavefile(IFile file, ISaveMethod Method, SaveState state);


#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [ContextMenu("PrintPath")]
        private void PrintPath()
        {
            if (fromat == SaveFormat.PlayerPrefs)
            {
                Debug.Log("PlayerPrefs store by OS [Not Specefic location]");
                return;
            }

            string filetype = fromat == SaveFormat.TEXT ? ".txt" :
            fromat == SaveFormat.Binary ? ".RSave" :
            fromat == SaveFormat.XML ? ".xml" :
            fromat == SaveFormat.JSON ? ".json" : "";

            if (CustomPath)
            {
                Debug.Log(FilePath + "/FileName" + filetype);
            }
            else
            {
                Debug.Log(Application.persistentDataPath + "/FileName" + filetype);
            }
        }
#endif

    }
    public abstract class SaveManager_Storage : SaveManager_Method
    {
        [Header("Details")]
        [SerializeField, ConditionalShowByEnum("Mode", SaveFileStructure.SingleFile)]
        private SaveAsset SingeFileAsset;
        [SerializeField, ConditionalShowByEnum("Mode", SaveFileStructure.MergedFile)]
        private SoftType<IMergeFile> MergeFileClass;
        private object MySaveFile;
        public readonly List<IFile> FileList = new List<IFile>(3);


        // SaveManager Methods
        protected override IFile CreateMainFile()
        {
            if (Mode == SaveFileStructure.MergedFile)
            {
                MySaveFile = System.Activator.CreateInstance(MergeFileClass);
            }
            if (Mode == SaveFileStructure.SingleFile)
            {
                if (SingeFileAsset != null)
                {
                    MySaveFile = ScriptableObject.CreateInstance(SingeFileAsset.GetType());
                }
                else
                {
                    Debug.LogWarning("SingleFileAsset is not valid");
                    // Mode = SaveFileStructure.MultiFile;
                }
            }

            if (MySaveFile is IFile provider)
            {
                return provider;
            }
            else
            {
                Debug.LogError("Your MainFile should implement IFile interface");
                return null;
            }
        }
        protected override T GetMainFile<T>()
        {
            return (T)MySaveFile;
        }
        public override bool HasFile(IFile file)
        {
            return FileList.Contains(file);
        }
        public override bool AddFile(IFile file)
        {
            if (file == null)
            {
                WriteLog($"File not valid");
                return false;
            }

            if (FileList.Contains(file))// reference comparison
            {
                WriteLog($"File({file.FileName}) already added");
                return false;
            }

            if (Mode == SaveFileStructure.MultiFile)
            {
                if (IsExist(file))
                {
                    Load(file);
                }
            }

            FileList.Add(file);
            return true;
        }
        public override bool RemoveFile(IFile file)
        {
            if (file == null)
            {
                WriteLog($"File not valid");
                return false;
            }

            return FileList.Remove(file); // removes same reference
        }
        public override IFile[] GetAllFiles()
        {
            return FileList.ToArray();
        }
    }

}