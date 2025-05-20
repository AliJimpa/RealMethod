using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RealMethod
{
    public class TableViewerWindow : EditorWindow
    {
        private TableAsset CurrentTable;
        private SheetViewer MySheet;
        private List<string[]> TableList;
        private Vector2 SheetSize;


        private static TableViewerWindow window;

        private void OnEnable()
        {
            MySheet = new SheetViewer(out TableList);
            SheetSize = MySheet.GetGridSize();
            minSize = new Vector2(500, 300);
        }
        private void OnGUI()
        {
            GUILayout.Space(2);
            if (GUILayout.Button("Refresh", GUILayout.Width(55)))
            {
                CurrentTable.FillList(ref window.TableList, true);
                MySheet.RefreshColumnDetails(true);
            }
            GUILayout.Space(10);

            //Rendering
            MySheet.Render();

            //Size
            float cashgridsize = SheetSize.x;
            SheetSize.x = EditorGUILayout.Slider("Size(x):", SheetSize.x, 0f, 500f);
            if (SheetSize.x != cashgridsize)
            {
                MySheet.SetGridSize(new Vector2(SheetSize.x, MySheet.GetGridSize().y));
            }

        }
        private void OnDisable()
        {
            MySheet = null;
            TableList = null;
            SheetSize = Vector2.zero;
            CurrentTable = null;
        }

        public static void OpenWindow(TableAsset Table)
        {
            window = GetWindow<TableViewerWindow>(Table.name);
            window.CurrentTable = Table;
            window.CurrentTable.FillList(ref window.TableList, true);
            window.MySheet.RefreshColumnDetails(true);
            window.Show();
        }
        public static bool IsOpenWindow()
        {
            return window != null;
        }
        public static void CloseWindow()
        {
            if (window != null)
            {
                window.Close();
            }
        }


        // private void DrawStructFields(ref object structInstance)
        // {
        //     var fields = structInstance.GetType().GetFields();
        //     foreach (var field in fields)
        //     {
        //         object value = field.GetValue(structInstance);
        //         System.Type type = field.FieldType;

        //         EditorGUILayout.BeginHorizontal();
        //         EditorGUILayout.LabelField(field.Name, GUILayout.Width(150));

        //         if (type == typeof(int))
        //             value = EditorGUILayout.IntField((int)value);
        //         else if (type == typeof(float))
        //             value = EditorGUILayout.FloatField((float)value);
        //         else if (type == typeof(string))
        //             value = EditorGUILayout.TextField((string)value);
        //         else if (type == typeof(bool))
        //             value = EditorGUILayout.Toggle((bool)value);
        //         else if (type.IsEnum)
        //             value = EditorGUILayout.EnumPopup((System.Enum)value);
        //         else if (type == typeof(Vector2))
        //             value = EditorGUILayout.Vector2Field("", (Vector2)value);
        //         else if (type == typeof(Vector3))
        //             value = EditorGUILayout.Vector3Field("", (Vector3)value);
        //         else if (type == typeof(Vector4))
        //             value = EditorGUILayout.Vector4Field("", (Vector4)value);
        //         else if (type == typeof(GameObject))
        //             value = EditorGUILayout.ObjectField((GameObject)value, typeof(GameObject), true);
        //         else if (type == typeof(Texture2D))
        //             value = EditorGUILayout.ObjectField((Texture2D)value, typeof(Texture2D), false);
        //         else if (type == typeof(Sprite))
        //             value = EditorGUILayout.ObjectField((Sprite)value, typeof(Sprite), false);
        //         else if (type == typeof(AudioClip))
        //             value = EditorGUILayout.ObjectField((AudioClip)value, typeof(AudioClip), false);
        //         else if (typeof(ScriptableObject).IsAssignableFrom(type))
        //             value = EditorGUILayout.ObjectField((ScriptableObject)value, type, false);
        //         else
        //             EditorGUILayout.LabelField("Unsupported Type");

        //         field.SetValue(structInstance, value);
        //         EditorGUILayout.EndHorizontal();
        //     }
        // }

    }
}