using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;


namespace RealMethod.Editor
{
    /// <summary>
    /// Base class responsible for rendering a property in the Unity Editor
    /// and handling potential validation errors with optional fix actions.
    /// </summary>
    public abstract class EditorProperty
    {
        /// <summary>
        /// Holds the current error state for this property.
        /// If not null, the property will render an error instead of its normal UI.
        /// </summary>
        private ErrorAction PropertyError = null;


        /// <summary>
        /// The Unity object that owns this property.
        /// Usually a MonoBehaviour, ScriptableObject, or other UnityEngine.Object.
        /// </summary>
        public Object Owner { get; private set; }
        /// <summary>
        /// Name of the property being rendered.
        /// Used for identification and debugging.
        /// </summary>
        public string PropertyName { get; private set; }


        /// <summary>
        /// Creates a new editor property renderer.
        /// </summary>
        /// <param name="_Name">Name of the property.</param>
        /// <param name="_Owner">Object that owns the property.</param>
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


        /// <summary>
        /// Renders the property in the inspector.
        /// If the property has an error, the error UI is rendered instead.
        /// </summary>
        /// <returns>
        /// A byte result defined by the implementation (usually used for change flags or state).
        /// </returns>
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
        /// <summary>
        /// Registers an error for this property and provides a fix callback.
        /// When an error exists, normal rendering is replaced with an error UI.
        /// </summary>
        /// <param name="message">Error message shown in the inspector.</param>
        /// <param name="id">Identifier used to determine which fix action to apply.</param>
        protected void Error(string message, int id)
        {
            PropertyError = new ErrorAction(message, id, FixError);
        }


        /// <summary>
        /// Implemented by derived classes to render the property UI.
        /// Called only when no error is active.
        /// </summary>
        protected abstract byte UpdateRender();
        /// <summary>
        /// Called when the user activates the fix action from an error message.
        /// </summary>
        /// <param name="Id">Identifier of the error that should be fixed.</param>
        protected abstract void FixError(int Id);
    }
    /// <summary>
    /// Generic editor property wrapper that stores and manages a value of type <typeparamref name="T"/>.
    /// 
    /// This class extends <see cref="EditorProperty"/> and adds value storage, 
    /// comparison operators, and implicit conversion for easier usage in editor code.
    /// 
    /// It is typically used by custom inspector systems to track a property's
    /// current value and optionally compare against a cached value to detect changes.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    public abstract class EditorProperty<T> : EditorProperty
    {
        /// <summary>
        /// Returns true if the current value is valid (not null).
        /// Mainly useful for reference types.
        /// </summary>
        public bool isvalid => CurrentValue != null;
        /// <summary>
        /// Current value of the property.
        /// This is the active value used by the editor UI.
        /// </summary>
        protected T CurrentValue;
        /// <summary>
        /// Cached value used for change tracking or restoring previous state.
        /// (Likely intended as "CacheValue".)
        /// </summary>
        protected T CashValue;

        /// <summary>
        /// Creates a new property without assigning a default value.
        /// </summary>
        /// <param name="Name">Name of the property.</param>
        /// <param name="other">Owner Unity object.</param>
        public EditorProperty(string Name, Object other) : base(Name, other)
        {
        }
        /// <summary>
        /// Creates a new property with a default value.
        /// The default is applied to both the current and cached values.
        /// </summary>
        /// <param name="Name">Name of the property.</param>
        /// <param name="other">Owner Unity object.</param>
        /// <param name="DefaultValue">Initial value assigned to the property.</param>
        public EditorProperty(string Name, Object other, T DefaultValue) : base(Name, other)
        {
            CurrentValue = DefaultValue;
            CashValue = DefaultValue;
        }
        /// <summary>
        /// Returns the current value of the property.
        /// </summary>
        public T GetValue()
        {
            return CurrentValue;
        }
        /// <summary>
        /// Updates the current value of the property.
        /// </summary>
        /// <param name="NewValue">New value to assign.</param>
        public void SetValue(T NewValue)
        {
            CurrentValue = NewValue;
        }

        // Operators
        public static implicit operator T(EditorProperty<T> property)
        {
            return property != null ? property.CurrentValue : default;
        }
        public static bool operator ==(EditorProperty<T> a, EditorProperty<T> b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if (a is null || b is null)
                return false;

            return EqualityComparer<T>.Default.Equals(a.CurrentValue, b.CurrentValue);
        }
        public static bool operator !=(EditorProperty<T> a, EditorProperty<T> b)
        {
            return !(a == b);
        }
        public static bool operator ==(EditorProperty<T> a, T b)
        {
            if (a is null)
                return false;

            return EqualityComparer<T>.Default.Equals(a.CurrentValue, b);
        }
        public static bool operator !=(EditorProperty<T> a, T b)
        {
            return !(a == b);
        }

        // Overrides
        /// <summary>
        /// Determines equality between this property and another object.
        /// Supports comparison with both EditorProperty&lt;T&gt; and raw values of type T.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is EditorProperty<T> other)
                return EqualityComparer<T>.Default.Equals(CurrentValue, other.CurrentValue);

            if (obj is T value)
                return EqualityComparer<T>.Default.Equals(CurrentValue, value);

            return false;
        }
        public override int GetHashCode()
        {
            return CurrentValue?.GetHashCode() ?? 0;
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
    public class EP_Dropdown : EditorProperty<int>
    {
        private string[] Options;

        public EP_Dropdown(string Name, string[] options, Object other) : base(Name, other)
        {
            Options = options;
        }

        protected override byte UpdateRender()
        {
            CashValue = EditorGUILayout.Popup($"{PropertyName}:", CurrentValue, Options);
            if (CashValue == CurrentValue)
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
    public class EP_Asset<T> : EditorProperty<T> where T : PrimitiveAsset
    {
        public EP_Asset(string Name, Object other) : base(Name, other)
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
    public class EP_Variable<T> : EditorProperty<T> where T : class
    {
        public EP_Variable(string Name, Object other) : base(Name, other)
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

        public System.Action<T> OnItemChange;

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
                if (item.Render() == 1)
                {
                    OnItemChange?.Invoke(item);
                }
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
            return RM_MetaData.HasMetadata(GetAssetPath(), PropertyName);
        }

        protected string GetAssetPath()
        {
            TextAsset textAsset = (TextAsset)GetMyOwner().target;
            return AssetDatabase.GetAssetPath(textAsset);
        }
        protected UnityEditor.Editor GetMyOwner()
        {
            return (UnityEditor.Editor)Owner;
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

        public EP_Storeable(UnityEditor.Editor other, string Name) : base(Name, other)
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

        public EP_StoreableList(UnityEditor.Editor other, string Name) : base(Name, other)
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
    public class EPS_AssetList<T> : EP_StoreableList<T> where T : PrimitiveAsset
    {
        public EPS_AssetList(UnityEditor.Editor other, string Name) : base(other, Name)
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
            int.TryParse(RM_MetaData.LoadCustomMetadata(Path, PropertyName), out MaxCount);
            string targetpath;
            for (int i = 0; i < MaxCount; i++)
            {
                if (RM_MetaData.HasMetadata(Path, i + "_" + PropertyName))
                {
                    MyList.Add(null);
                    targetpath = RM_MetaData.LoadCustomMetadata(Path, i + "_" + PropertyName);
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
            RM_MetaData.SaveCustomMetadata(Path, PropertyName, GetCount().ToString());
            for (int i = 0; i < GetCount(); i++)
            {
                RM_MetaData.SaveCustomMetadata(Path, i + "_" + PropertyName, GetTargetPath(MyList[i]));
            }
            DebugPrint("Saved");
        }
        protected override void OnDelete(int Index)
        {
            string Path = GetAssetPath();
            if (RM_MetaData.HasMetadata(Path, Index + "_" + PropertyName))
            {
                RM_MetaData.DeleteMetadata(Path, Index + "_" + PropertyName);
                DebugPrint("Deleted");
            }
            else
            {
                Debug.LogError($"No metadata found for {Index} in {PropertyName}");
            }
            RM_MetaData.SaveCustomMetadata(Path, PropertyName, GetCount().ToString());
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
        public EPS_Enum(UnityEditor.Editor other, string Name) : base(other, Name)
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
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.GetHashCode().ToString());
            DebugPrint("Saved");
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            int Result = 0;
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                int.TryParse(RM_MetaData.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = (T)(object)Result;
                DebugPrint(Result + "");
            }
            else
            {
                CurrentValue = (T)(object)Result;
                RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void FixError(int Id)
        {
        }
    }
    public class EPS_Asset<T> : EP_Storeable<T> where T : PrimitiveAsset
    {
        public EPS_Asset(UnityEditor.Editor other, string Name) : base(other, Name)
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
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                string targetpath = RM_MetaData.LoadCustomMetadata(Path, PropertyName);
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
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, GetTargetPath().ToString());
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
        public EPS_int(UnityEditor.Editor other, string Name) : base(other, Name)
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
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                int.TryParse(RM_MetaData.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }

        protected override void FixError(int Id)
        {
        }
    }
    public class EPS_string : EP_Storeable<string>
    {
        public EPS_string(UnityEditor.Editor other, string Name) : base(other, Name)
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
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                CurrentValue = RM_MetaData.LoadCustomMetadata(Path, PropertyName);
                DebugPrint(CurrentValue);
            }
            else
            {
                CurrentValue = "Empty";
                RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue);
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue);
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }

    }
    public class EPS_float : EP_Storeable<float>
    {
        public EPS_float(UnityEditor.Editor other, string Name) : base(other, Name)
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
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                float.TryParse(RM_MetaData.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
        }

    }
    public class EPS_bool : EP_Storeable<bool>
    {
        public EPS_bool(UnityEditor.Editor other, string Name) : base(other, Name)
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
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                CurrentValue = RM_MetaData.LoadCustomMetadata(Path, PropertyName) == "True" ? true : false;
                DebugPrint(CurrentValue.ToString());
            }
            else
            {
                CurrentValue = false;
                RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, "False");
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue ? "True" : "False");
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
        }

    }
    public class EPS_Color : EP_Storeable<Color>
    {
        public EPS_Color(UnityEditor.Editor other, string Name) : base(other, Name)
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
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                UnityEngine.ColorUtility.TryParseHtmlString(RM_MetaData.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
        }


    }
    public class EPS_Date : EP_Storeable<System.DateTime>
    {
        public EPS_Date(UnityEditor.Editor other, string Name) : base(other, Name)
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
            if (RM_MetaData.HasMetadata(Path, PropertyName))
            {
                System.DateTime.TryParse(RM_MetaData.LoadCustomMetadata(Path, PropertyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            RM_MetaData.SaveCustomMetadata(GetAssetPath(), PropertyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
        protected override void FixError(int Id)
        {
            throw new System.NotImplementedException();
        }


    }


}