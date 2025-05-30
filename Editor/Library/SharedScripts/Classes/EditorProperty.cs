using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace RealMethod.Editor
{
    // Base EditorProperty Class
    // This class is used to create custom editor properties that can be rendered in the Unity Editor.
    public abstract class EditorProperty
    {
        protected Object Owner;
        protected string PropertyName { get; private set; }
        private ErrorAction PropertyError = null;

        public EditorProperty(string _Name, Object _Owner)
        {
            PropertyName = _Name;
            Owner = _Owner;
            if (Owner == null)
            {
                Debug.LogError($"Owner is null in {PropertyName}");
                return;
            }
        }

        public byte Render()
        {
            if (PropertyError == null)
            {
                return UpdateRender();
            }
            else
            {
                PropertyError.RenderError();
                return 0;
            }
        }

        protected void Error(string message, int id)
        {
            PropertyError = new ErrorAction(message, id, FixError);
        }

        protected abstract byte UpdateRender();
        protected abstract void FixError(int Id);
    }
    public abstract class EditorProperty<T> : EditorProperty
    {
        public bool isvalid => CurrentValue != null;
        protected T CurrentValue;
        protected T CashValue;

        public EditorProperty(string Name, Object other) : base(Name, other)
        {
        }
        public EditorProperty(string Name, Object other, T DefaultValue) : base(Name, other)
        {
            CurrentValue = DefaultValue;
            CashValue = DefaultValue;
        }
        public T GetValue()
        {
            return CurrentValue;
        }
        public void SetValue(T NewValue)
        {
            CurrentValue = NewValue;
        }

    }
    public abstract class EditorPropertyList<T> : EditorProperty
    {
        protected List<T> MyList = new List<T>();

        protected EditorPropertyList(string _Name, Object _Owner) : base(_Name, _Owner)
        {
        }
        public int GetCount()
        {
            return MyList.Count;
        }
        public void AddItem(T NewValue)
        {
            MyList.Add(NewValue);
            OnCreated(NewValue);
        }
        public void RemoveItem(int Index)
        {
            if (Index >= 0 && Index < MyList.Count)
            {
                MyList.RemoveAt(Index);
                OnDeleted(Index);
            }
        }
        public T GetValue(int Index)
        {
            return MyList[Index];
        }
        public List<T> GetList()
        {
            return MyList;
        }
        public void ClearList()
        {
            MyList.Clear();
        }

        protected abstract void OnCreated(T Item);
        protected abstract void OnDeleted(int Index);
    }
    // Sample EditorProperty
    public class EP_Enum<T> : EditorProperty<T> where T : System.Enum
    {
        public EP_Enum(string Name, Object other) : base(Name, other)
        {
        }

        protected override byte UpdateRender()
        {
            CashValue = (T)EditorGUILayout.EnumPopup($"{PropertyName}:", CurrentValue);
            if (EqualityComparer<T>.Default.Equals(CashValue, CurrentValue))
            {
                return 0; // No change
            }
            else
            {
                SetValue(CashValue);
                return 1; // Changed
            }
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }

    }
    public class EP_String : EditorProperty<string>
    {
        public EP_String(string Name, Object other) : base(Name, other)
        {
        }

        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.TextField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }
    }
    public class EP_int : EditorProperty<int>
    {
        public EP_int(string Name, Object other) : base(Name, other)
        {
        }

        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.IntField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void FixError(int Id)
        {
        }
    }
    public class EP_float : EditorProperty<float>
    {
        public EP_float(string Name, Object other) : base(Name, other)
        {
        }

        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.FloatField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void FixError(int Id)
        {
        }

    }
    public class EP_bool : EditorProperty<bool>
    {
        public EP_bool(string Name, Object other) : base(Name, other)
        {
        }


        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.Toggle($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void FixError(int Id)
        {
        }

    }
    public class EP_Vector3 : EditorProperty<Vector3>
    {
        public EP_Vector3(string Name, Object other) : base(Name, other)
        {
        }

        protected override byte UpdateRender()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{PropertyName}:", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            CashValue = EditorGUILayout.Vector3Field("", CurrentValue, GUILayout.MinWidth(0));
            GUILayout.EndHorizontal();
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                SetValue(CashValue);
                return 1;
            }


        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }
    }
    public class EP_Color : EditorProperty<Color>
    {
        public EP_Color(string Name, Object other) : base(Name, other)
        {

        }


        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.ColorField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void FixError(int Id)
        {
        }


    }
    public class EP_Date : EditorProperty<System.DateTime>
    {
        public EP_Date(string Name, Object other) : base(Name, other)
        {
        }


        protected override byte UpdateRender()
        {
            EditorGUILayout.LabelField($"{PropertyName}:", CurrentValue.ToString());
            return 0;
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }


    }
    public class EP_ScriptableObject<T> : EditorProperty<T> where T : ScriptableObject
    {
        public EP_ScriptableObject(string Name, Object other) : base(Name, other)
        {
            CurrentValue = null;
            CashValue = null;
        }

        protected override byte UpdateRender()
        {
            CashValue = (T)EditorGUILayout.ObjectField($"{PropertyName}:", CurrentValue, typeof(T), false);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void FixError(int Id)
        {
        }

    }
    public class EP_Class<T> : EditorProperty<T> where T : class
    {
        public EP_Class(string Name, Object other) : base(Name, other)
        {
        }

        protected override byte UpdateRender()
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(CurrentValue);

                if (field.FieldType == typeof(int))
                {
                    int newValue = EditorGUILayout.IntField(field.Name, (int)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(float))
                {
                    float newValue = EditorGUILayout.FloatField(field.Name, (float)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(string))
                {
                    string newValue = EditorGUILayout.TextField(field.Name, (string)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(bool))
                {
                    bool newValue = EditorGUILayout.Toggle(field.Name, (bool)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType.IsEnum)
                {
                    System.Enum newValue = EditorGUILayout.EnumPopup(field.Name, (System.Enum)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(Vector3))
                {
                    Vector3 newValue = EditorGUILayout.Vector3Field(field.Name, (Vector3)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(Color))
                {
                    Color newValue = EditorGUILayout.ColorField(field.Name, (Color)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else
                {
                    EditorGUILayout.LabelField(field.Name, $"Unsupported type: {field.FieldType}");
                }
            }
            return 0;
        }
        protected override void FixError(int Id)
        {

        }
    }
    public class EP_Struct<T> : EditorProperty<T> where T : struct
    {
        public EP_Struct(string Name, Object other) : base(Name, other)
        {
        }

        protected override byte UpdateRender()
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(CurrentValue);

                if (field.FieldType == typeof(int))
                {
                    int newValue = EditorGUILayout.IntField(field.Name, (int)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(float))
                {
                    float newValue = EditorGUILayout.FloatField(field.Name, (float)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(string))
                {
                    string newValue = EditorGUILayout.TextField(field.Name, (string)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(bool))
                {
                    bool newValue = EditorGUILayout.Toggle(field.Name, (bool)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType.IsEnum)
                {
                    System.Enum newValue = EditorGUILayout.EnumPopup(field.Name, (System.Enum)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(Vector3))
                {
                    Vector3 newValue = EditorGUILayout.Vector3Field(field.Name, (Vector3)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else if (field.FieldType == typeof(Color))
                {
                    Color newValue = EditorGUILayout.ColorField(field.Name, (Color)value);
                    field.SetValue(CurrentValue, newValue);
                }
                else
                {
                    EditorGUILayout.LabelField(field.Name, $"Unsupported type: {field.FieldType}");
                }
            }
            return 0;
        }
        protected override void FixError(int Id)
        {

        }
    }
    public class EP_List<T> : EditorPropertyList<T> where T : EditorProperty
    {
        public bool EnableScrollView = true;
        public Vector2 ScrollPosition = Vector2.zero;
        public Vector2 Size = new(20, 200);

        public EP_List(string _Name, Object _Owner) : base(_Name, _Owner)
        {
        }

        public void AddItem(string Name)
        {
            AddItem((T)System.Activator.CreateInstance(typeof(T), Name, Owner));
        }

        protected override byte UpdateRender()
        {
            if (EnableScrollView)
                ScrollPosition = GUILayout.BeginScrollView(ScrollPosition, GUILayout.Height(Size.y));
            foreach (var item in MyList)
            {
                item.Render();
            }
            if (EnableScrollView)
                GUILayout.EndScrollView();
            return 0;
        }
        protected override void OnCreated(T Item)
        {
        }
        protected override void OnDeleted(int Index)
        {
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }
    }



    // Storeable
    // EditorProperty that can be stored in the MetaData
    public abstract class EP_Storeable : EditorProperty
    {
        protected bool IsDebugMode = false;
        public EP_Storeable(string _Name, Object _Owner) : base(_Name, _Owner)
        {
            OnLoad();
            OnCreated();
        }

        public bool IsValid()
        {
            return MetaDataHandler.HasMetadata(GetAssetPath(), PropertyName);
        }

        protected string GetAssetPath()
        {
            TextAsset textAsset = (TextAsset)GetMyOwner().target;
            return AssetDatabase.GetAssetPath(textAsset);
        }
        protected Editor GetMyOwner()
        {
            return (Editor)Owner;
        }
        protected void DebugPrint(string message)
        {
            if (IsDebugMode)
                Debug.Log($"{PropertyName}: {message}");
        }

        protected abstract void OnCreated();
        protected abstract void OnSave();
        protected abstract void OnLoad();

    }
    public abstract class EP_Storeable<T> : EP_Storeable
    {
        protected T CurrentValue;
        protected T CashValue;

        public EP_Storeable(Editor other, string Name) : base(Name, other)
        {
        }
        public T GetValue()
        {
            return CurrentValue;
        }
        public void SetValue(T NewValue)
        {
            CurrentValue = NewValue;
            OnSave();
        }
    }
    public abstract class EP_StoreableList<T> : EP_Storeable
    {
        protected List<T> MyList = new List<T>();

        public EP_StoreableList(Editor other, string Name) : base(Name, other)
        {
        }
        public int GetCount()
        {
            return MyList.Count;
        }
        public void AddItem(T NewValue)
        {
            MyList.Add(NewValue);
            OnSave();
        }
        public void RemoveItem(int Index)
        {
            if (Index >= 0 && Index < MyList.Count)
            {
                MyList.RemoveAt(Index);
                OnDelete(Index);
            }
        }
        public T GetValue(int Index)
        {
            return MyList[Index];
        }
        public List<T> GetList()
        {
            return MyList;
        }

        protected abstract void OnDelete(int Index);

    }
    /// Sample EP_Storeable
    public class EPS_ScriptableObjectList<T> : EP_StoreableList<T> where T : ScriptableObject
    {
        public EPS_ScriptableObjectList(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {
        }
        protected override byte UpdateRender()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                AddItem(null);
            }
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                RemoveItem(GetCount() - 1);
            }
            GUI.enabled = false;
            EditorGUILayout.IntField("Count:", GetCount());
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            T CashValue = null;
            for (int i = 0; i < MyList.Count; i++)
            {
                RenderItemIndex(ref CashValue, i);
                if (CashValue != MyList[i])
                {
                    MyList[i] = CashValue;
                    OnSave();
                    DebugPrint("Changed");
                }
            }
            return 0;

        }
        protected override void OnLoad()
        {
            MyList.Clear();
            string Path = GetAssetPath();

            int MaxCount = 0;
            int.TryParse(MetaDataHandler.LoadCustomMetadata(Path, PropertyName), out MaxCount);
            string targetpath;
            for (int i = 0; i < MaxCount; i++)
            {
                if (MetaDataHandler.HasMetadata(Path, i + "_" + PropertyName))
                {
                    MyList.Add(null);
                    targetpath = MetaDataHandler.LoadCustomMetadata(Path, i + "_" + PropertyName);
                    if (targetpath != null)
                    {
                        MyList[i] = AssetDatabase.LoadAssetAtPath<T>(targetpath);
                    }
                }
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            string Path = GetAssetPath();
            MetaDataHandler.SaveCustomMetadata(Path, PropertyName, GetCount().ToString());
            for (int i = 0; i < GetCount(); i++)
            {
                MetaDataHandler.SaveCustomMetadata(Path, i + "_" + PropertyName, GetTargetPath(MyList[i]));
            }
            DebugPrint("Saved");
        }
        protected override void OnDelete(int Index)
        {
            string Path = GetAssetPath();
            if (MetaDataHandler.HasMetadata(Path, Index + "_" + PropertyName))
            {
                MetaDataHandler.DeleteMetadata(Path, Index + "_" + PropertyName);
                DebugPrint("Deleted");
            }
            else
            {
                Debug.LogError($"No metadata found for {Index} in {PropertyName}");
            }
            MetaDataHandler.SaveCustomMetadata(Path, PropertyName, GetCount().ToString());
        }
        protected override void FixError(int Id)
        {
        }

        protected virtual void RenderItemIndex(ref T Result, int Index)
        {
            Result = (T)EditorGUILayout.ObjectField(MyList[Index], typeof(T), false);
        }

        private string GetTargetPath(T Target)
        {
            return AssetDatabase.GetAssetPath(Target);
        }




    }
    public class EPS_Enum<T> : EP_Storeable<T> where T : System.Enum
    {
        public EPS_Enum(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {
        }
        protected override byte UpdateRender()
        {
            CashValue = (T)EditorGUILayout.EnumPopup($"{PropertyName}:", CurrentValue);
            if (EqualityComparer<T>.Default.Equals(CashValue, CurrentValue))
            {
                return 0; // No change
            }
            else
            {
                DebugPrint("Changed");
                SetValue(CashValue);
                return 1; // Changed
            }

        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.GetHashCode().ToString());
            DebugPrint("Saved");
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            int Result = 0;
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                int.TryParse(MetaDataHandler.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = (T)(object)Result;
                DebugPrint(Result + "");
            }
            else
            {
                CurrentValue = (T)(object)Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void FixError(int Id)
        {
        }
    }
    public class EPS_ScriptableObject<T> : EP_Storeable<T> where T : ScriptableObject
    {
        public EPS_ScriptableObject(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        protected override byte UpdateRender()
        {
            CashValue = (T)EditorGUILayout.ObjectField($"{PropertyName}:", CurrentValue, typeof(T), false);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                DebugPrint("Changed");
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                string targetpath = MetaDataHandler.LoadCustomMetadata(Path, PropertyName);
                CurrentValue = AssetDatabase.LoadAssetAtPath<T>(targetpath);
                DebugPrint(targetpath);
            }
            else
            {
                CurrentValue = null;
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, GetTargetPath().ToString());
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
        }

        private string GetTargetPath()
        {
            return AssetDatabase.GetAssetPath(CurrentValue);
        }


    }
    public class EPS_int : EP_Storeable<int>
    {
        public EPS_int(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.IntField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                DebugPrint("Changed");
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            int Result = 0;
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                int.TryParse(MetaDataHandler.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }

        protected override void FixError(int Id)
        {
        }
    }
    public class EPS_string : EP_Storeable<string>
    {
        public EPS_string(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.TextField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                DebugPrint("Changed");
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                CurrentValue = MetaDataHandler.LoadCustomMetadata(Path, PropertyName);
                DebugPrint(CurrentValue);
            }
            else
            {
                CurrentValue = "Empty";
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue);
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue);
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }

    }
    public class EPS_float : EP_Storeable<float>
    {
        public EPS_float(Editor other, string Name) : base(other, Name)
        {
        }


        protected override void OnCreated()
        {

        }
        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.FloatField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                DebugPrint("Changed");
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            float Result = 0;
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                float.TryParse(MetaDataHandler.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
        }

    }
    public class EPS_bool : EP_Storeable<bool>
    {
        public EPS_bool(Editor other, string Name) : base(other, Name)
        {
        }


        protected override void OnCreated()
        {

        }
        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.Toggle($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                DebugPrint("Changed");
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                CurrentValue = MetaDataHandler.LoadCustomMetadata(Path, PropertyName) == "True" ? true : false;
                DebugPrint(CurrentValue.ToString());
            }
            else
            {
                CurrentValue = false;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, "False");
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue ? "True" : "False");
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
        }

    }
    public class EPS_Color : EP_Storeable<Color>
    {
        public EPS_Color(Editor other, string Name) : base(other, Name)
        {

        }


        protected override void OnCreated()
        {
        }
        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.ColorField($"{PropertyName}:", CurrentValue);
            if (CashValue == CurrentValue)
            {
                return 0;
            }
            else
            {
                DebugPrint("Changed");
                SetValue(CashValue);
                return 1;
            }
        }
        protected override void OnLoad()
        {

            string Path = GetAssetPath();
            Color Result = Color.black;
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                UnityEngine.ColorUtility.TryParseHtmlString(MetaDataHandler.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
        }


    }
    public class EPS_Date : EP_Storeable<System.DateTime>
    {
        public EPS_Date(Editor other, string Name) : base(other, Name)
        {
        }


        protected override void OnCreated()
        {
        }
        protected override byte UpdateRender()
        {
            EditorGUILayout.LabelField($"{PropertyName}:", CurrentValue.ToString());
            return 0;
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            System.DateTime Result = System.DateTime.Now;
            if (MetaDataHandler.HasMetadata(Path, PropertyName))
            {
                System.DateTime.TryParse(MetaDataHandler.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }


    }


}