using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


namespace RealMethod
{
    public class SheetViewer
    {
        private Color DefaultColor = GUI.backgroundColor;
        private Vector2 CellSize = new Vector2(50, 30); // Cell size    
        private Vector2 ScrollPosCash; // Stores scroll position
        private List<string[]> CurrentTable = new List<string[]>(); // Stores the text in each cell
        private string[] ColumnDetails = new string[3]; // Column title
        public int ColumnSize { get; private set; }
        public int SelectRow = -1;

        public SheetViewer(List<string[]> tableContent)
        {
            CurrentTable.Clear();
            CurrentTable = tableContent;
            ColumnSize = GetMaxColumnSize();
            RefreshColumnDetails();
        }
        public SheetViewer(TextAsset CSVAsset)
        {
            CurrentTable.Clear();
            CurrentTable = CSV.ParseCSV(CSVAsset.text);
            ColumnSize = GetMaxColumnSize();
            RefreshColumnDetails();
        }
        public SheetViewer(out List<string[]> target)
        {
            CurrentTable.Clear();
            target = CurrentTable;
            ColumnSize = GetMaxColumnSize();
            RefreshColumnDetails();
        }

        public void Render(bool isActive = true)
        {
            // **Start Scroll View**
            ScrollPosCash = EditorGUILayout.BeginScrollView(ScrollPosCash, GUILayout.Height(400)); // Adjust height as needed

            if (!isActive)
                GUI.enabled = false;

            // **Draw Column Titles**
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(CellSize.y)); // Empty space for row numbers
                                                                         // int ColumnSize = GetMaxColumnSize();
            for (int cs = 0; cs < ColumnSize; cs++) // column size
            {
                // **Column Title**
                EditorGUILayout.LabelField(GetColumnLetter(cs), EditorStyles.boldLabel, GUILayout.Width(CellSize.x));
            }
            EditorGUILayout.EndHorizontal();

            // **Draw Grid with Row Indicators**
            for (int ri = 0; ri < CurrentTable.Count; ri++)
            {
                EditorGUILayout.BeginHorizontal();

                // **Row Number**
                EditorGUILayout.LabelField((ri + 1).ToString(), EditorStyles.boldLabel, GUILayout.Width(CellSize.y));

                var TargetRow = CurrentTable[ri];
                for (int ci = 0; ci < TargetRow.Length; ci++)
                {
                    if (ri == SelectRow)
                        GUI.color = Color.yellow;

                    TargetRow[ci] = EditorGUILayout.TextField(TargetRow[ci], GUILayout.Width(CellSize.x));

                    if (ri == SelectRow)
                        GUI.color = DefaultColor;
                }

                EditorGUILayout.EndHorizontal();
            }

            if (!isActive)
                GUI.enabled = true;

            EditorGUILayout.EndScrollView(); // **End Scroll View**

            // **Draw Column Details**
            EditorGUILayout.BeginHorizontal();
            foreach (var item in ColumnDetails)
            {
                EditorGUILayout.LabelField(item, GUILayout.Width(120));
            }
            EditorGUILayout.EndHorizontal();

        }
        public void SetGridSize(Vector2 size)
        {
            CellSize = size;
        }
        public void RefreshColumnDetails(bool Auto = false)
        {
            if (Auto)
            {
                ColumnSize = GetMaxColumnSize();
            }

            ColumnDetails[0] = $"Row: {CurrentTable.Count}";
            ColumnDetails[1] = $"Column: {ColumnSize}";
            ColumnDetails[2] = $"Cell: {CurrentTable.Count * ColumnSize}";
        }
        public Vector2 GetGridSize()
        {
            return CellSize;
        }

        private string GetColumnLetter(int index)
        {
            return ((char)('A' + index)).ToString();
        }
        private int GetMaxColumnSize()
        {
            int maxColumnSize = 0;
            foreach (string[] item in CurrentTable)
            {
                if (item.Length > maxColumnSize)
                {
                    maxColumnSize = item.Length;
                }
            }
            return maxColumnSize;
        }



    }
}
