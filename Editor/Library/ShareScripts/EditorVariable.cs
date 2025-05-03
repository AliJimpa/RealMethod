using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace RealMethod
{
    public abstract class EditorVariable
    {
        public bool HideDebug = true;
        protected Editor Owner;
        protected string MyName;


        public EditorVariable(Editor other, string Name)
        {
            Owner = other;
            MyName = Name;
            OnLoad();
            OnCreated();
            DebugPrint("Editor Variable Loaded or Created");
        }

        public bool IsValid()
        {
            return MetaDataHandler.HasMetadata(GetAssetPath(), MyName);
        }

        protected string GetAssetPath()
        {
            TextAsset textAsset = (TextAsset)Owner.target;
            return AssetDatabase.GetAssetPath(textAsset);
        }
        protected void DebugPrint(string Message)
        {
            if (!HideDebug)
                Debug.Log($"[{this}]-[{MyName}]: {Message}");
        }

        protected abstract void OnCreated();
        public abstract byte OnRender();
        protected abstract void OnSave();
        protected abstract void OnLoad();

    }
    public abstract class EditorVariable<T> : EditorVariable
    {
        protected T CurrentValue;
        protected T CashValue;

        public EditorVariable(Editor other, string Name) : base(other, Name)
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
    public abstract class EditorList<T> : EditorVariable
    {
        protected List<T> MyList = new List<T>();

        protected EditorList(Editor other, string Name) : base(other, Name)
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



    /// Example of a custom editor variable
    public class EV_Enum<T> : EditorVariable<T> where T : System.Enum
    {
        public EV_Enum(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {
        }
        public override byte OnRender()
        {
            CashValue = (T)EditorGUILayout.EnumPopup($"{MyName}:", CurrentValue);
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
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue.GetHashCode().ToString());
            DebugPrint("Saved");
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            int Result = 0;
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                int.TryParse(MetaDataHandler.LoadCustomMetadata(Path, MyName), out Result);
                CurrentValue = (T)(object)Result;
                DebugPrint(Result + "");
            }
            else
            {
                CurrentValue = (T)(object)Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
    }
    public class EV_ScriptableObject<T> : EditorVariable<T> where T : ScriptableObject
    {
        public EV_ScriptableObject(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        public override byte OnRender()
        {
            CashValue = (T)EditorGUILayout.ObjectField($"{MyName}:", CurrentValue, typeof(T), false);
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
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                string targetpath = MetaDataHandler.LoadCustomMetadata(Path, MyName);
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
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, GetTargetPath().ToString());
            DebugPrint("Saved");
        }
        private string GetTargetPath()
        {
            return AssetDatabase.GetAssetPath(CurrentValue);
        }
    }
    public class EV_int : EditorVariable<int>
    {
        public EV_int(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        public override byte OnRender()
        {
            CashValue = EditorGUILayout.IntField($"{MyName}:", CurrentValue);
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
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                int.TryParse(MetaDataHandler.LoadCustomMetadata(Path, MyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
    }
    public class EL_ScriptableObject<T> : EditorList<T> where T : ScriptableObject
    {
        public EL_ScriptableObject(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {
        }
        public override byte OnRender()
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
            int.TryParse(MetaDataHandler.LoadCustomMetadata(Path, MyName), out MaxCount);
            string targetpath;
            for (int i = 0; i < MaxCount; i++)
            {
                if (MetaDataHandler.HasMetadata(Path, i + "_" + MyName))
                {
                    MyList.Add(null);
                    targetpath = MetaDataHandler.LoadCustomMetadata(Path, i + "_" + MyName);
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
            MetaDataHandler.SaveCustomMetadata(Path, MyName, GetCount().ToString());
            for (int i = 0; i < GetCount(); i++)
            {
                MetaDataHandler.SaveCustomMetadata(Path, i + "_" + MyName, GetTargetPath(MyList[i]));
            }
            DebugPrint("Saved");
        }
        protected override void OnDelete(int Index)
        {
            string Path = GetAssetPath();
            if (MetaDataHandler.HasMetadata(Path, Index + "_" + MyName))
            {
                MetaDataHandler.DeleteMetadata(Path, Index + "_" + MyName);
                DebugPrint("Deleted");
            }
            else
            {
                Debug.LogError($"No metadata found for {Index} in {MyName}");
            }
            MetaDataHandler.SaveCustomMetadata(Path, MyName, GetCount().ToString());
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
    public class EV_string : EditorVariable<string>
    {
        public EV_string(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        public override byte OnRender()
        {
            CashValue = EditorGUILayout.TextField($"{MyName}:", CurrentValue);
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
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                CurrentValue = MetaDataHandler.LoadCustomMetadata(Path, MyName);
                DebugPrint(CurrentValue);
            }
            else
            {
                CurrentValue = "Empty";
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue);
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue);
            DebugPrint("Saved");
        }
    }
    public class EV_float : EditorVariable<float>
    {
        public EV_float(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        public override byte OnRender()
        {
            CashValue = EditorGUILayout.FloatField($"{MyName}:", CurrentValue);
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
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                float.TryParse(MetaDataHandler.LoadCustomMetadata(Path, MyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }
    }
    public class EV_bool : EditorVariable<bool>
    {
        public EV_bool(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {

        }
        public override byte OnRender()
        {
            CashValue = EditorGUILayout.Toggle($"{MyName}:", CurrentValue);
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
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                CurrentValue = MetaDataHandler.LoadCustomMetadata(Path, MyName) == "True" ? true : false;
                DebugPrint(CurrentValue.ToString());
            }
            else
            {
                CurrentValue = false;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, "False");
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue ? "True" : "False");
            DebugPrint("Saved");
        }
    }
    public class EV_Color : EditorVariable<Color>
    {
        public EV_Color(Editor other, string Name) : base(other, Name)
        {

        }

        protected override void OnCreated()
        {
        }
        public override byte OnRender()
        {
            CashValue = EditorGUILayout.ColorField($"{MyName}:", CurrentValue);
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
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                UnityEngine.ColorUtility.TryParseHtmlString(MetaDataHandler.LoadCustomMetadata(Path, MyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }


    }
    public class EV_Date : EditorVariable<System.DateTime>
    {
        public EV_Date(Editor other, string Name) : base(other, Name)
        {
        }

        protected override void OnCreated()
        {
        }
        public override byte OnRender()
        {
            EditorGUILayout.LabelField($"{MyName}:", CurrentValue.ToString());
            return 0;
        }
        protected override void OnLoad()
        {
            string Path = GetAssetPath();
            System.DateTime Result = System.DateTime.Now;
            if (MetaDataHandler.HasMetadata(Path, MyName))
            {
                System.DateTime.TryParse(MetaDataHandler.LoadCustomMetadata(Path, MyName), out Result);
                CurrentValue = Result;
                DebugPrint(Result.ToString());
            }
            else
            {
                CurrentValue = Result;
                MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, Result.ToString());
                DebugPrint("null");
            }
            DebugPrint("Loaded");
        }
        protected override void OnSave()
        {
            MetaDataHandler.SaveCustomMetadata(GetAssetPath(), MyName, CurrentValue.ToString());
            DebugPrint("Saved");
        }

    }




}