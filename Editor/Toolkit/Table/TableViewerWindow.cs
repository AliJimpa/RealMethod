using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace RealMethod
{
    public class TableViewerWindow : EditorWindow
    {
        private TableAsset CurrentTable;
        private TableAsset CashTable;
        private TableViewer Chart;
        private List<string[]> TableData;
        private float GridSize;


        private static TableViewerWindow window;

        void OnEnable()
        {
            Chart = new TableViewer(out TableData);
            GridSize = Chart.GetGridSize().x;

        }

        [MenuItem("Tools/RealMethod/Table Viewer")]
        public static void ShowWindow()
        {
            window = GetWindow<TableViewerWindow>("Table Viewer");
            window.minSize = new Vector2(500, 300);
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            CashTable = (TableAsset)EditorGUILayout.ObjectField("Table", CurrentTable, typeof(TableAsset), false);
            if (GUILayout.Button("Refresh"))
            {
                UpdateTable();
            }
            EditorGUILayout.EndHorizontal();

            if (CashTable != CurrentTable)
            {
                CurrentTable = CashTable;
                UpdateTable();
            }

            //Rendering
            Chart.Render();

            EditorGUILayout.Space(25);

            foreach (var item in TableData)
            {
                Debug.Log("---> " + item.Length);
            }

            //Size
            float cashgridsize = GridSize;
            GridSize = EditorGUILayout.Slider("Size(x):", GridSize, 0f, 500f);
            if (GridSize != cashgridsize)
            {
                Chart.SetGridSize(new Vector2(GridSize, Chart.GetGridSize().y));
            }

        }

        private void UpdateTable()
        {
            // Create New Table
            if (CurrentTable)
            {
                CurrentTable.FilStringLisst(ref TableData, true);
                Chart.RefreshColumnDetails(true);
                Debug.Log("Valid");
            }
            else
            {
                TableData.Clear();
                Chart.RefreshColumnDetails(true);
            }
            Debug.Log("UpdateTable");
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