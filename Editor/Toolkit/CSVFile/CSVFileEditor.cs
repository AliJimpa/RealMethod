using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(TextAsset))]
    public class CSVFileEditor : Editor
    {
        class CustomList<T> : EPS_ScriptableObjectList<T> where T : ScriptableObject
        {
            Color originalColor = GUI.backgroundColor;

            public CustomList(Editor other, string Name) : base(other, Name)
            {

            }

            protected override void RenderItemIndex(ref T Result, int Index)
            {
                if (MyList[Index] is ICSVDataTransfer csvItem)
                {
                    GUI.backgroundColor = originalColor;
                    base.RenderItemIndex(ref Result, Index);
                    GUI.backgroundColor = originalColor;
                }
                else
                {
                    GUI.backgroundColor = Color.red;
                    base.RenderItemIndex(ref Result, Index);
                    GUI.backgroundColor = originalColor;
                }
            }
        }
        enum CSVReadType
        {
            Row,
            Column
        }
        enum SheetType
        {
            GoogleSheets,
            MicrosoftOneDrive,
            Dropbox,
            CustomSheet
        }

        private bool IsActiveEditor = false;
        private string assetPath;
        private EPS_Enum<CSVReadType> ReadStyle;
        private SheetViewer TabelGUI;
        private CustomList<ScriptableObject> ObjectsPart;
        private EPS_Date LastUpdate;
        private EPS_Enum<SheetType> OnlineSheetType;
        private EPS_string SheetID;
        private bool Isdownloading = false;



        private void OnEnable()
        {
            // Find Asset Path
            TextAsset textAsset = (TextAsset)target;
            assetPath = AssetDatabase.GetAssetPath(textAsset);

            IsActiveEditor = IsCSVFile(assetPath);

            // Check file extension
            if (IsActiveEditor)
            {
                // Load the file content
                TabelGUI = new SheetViewer(textAsset);
                ReadStyle = new EPS_Enum<CSVReadType>(this, "ReadType");
                ObjectsPart = new CustomList<ScriptableObject>(this, "SObjects");
                LastUpdate = new EPS_Date(this, "LastUpdate");
                SheetID = new EPS_string(this, "SheetID");
                OnlineSheetType = new EPS_Enum<SheetType>(this, "SheetType");
                IsActiveEditor = true;
            }
            Debug.Log("CSV EditorInitiate");
        }
        public override void OnInspectorGUI()
        {
            // Show custom inspector only for .txt files
            if (IsActiveEditor)
            {
                GUI.enabled = true;
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("CSV File", EditorStyles.boldLabel);
                TabelGUI.Render();
                EditorGUILayout.Space(15);

                EditorGUILayout.LabelField("Connect To Cloude", EditorStyles.boldLabel);
                OnlineSheetType.Render();
                SheetID.Render();
                EditorGUILayout.BeginHorizontal();
                LastUpdate.Render();
                if (Isdownloading)
                {
                    EditorGUILayout.LabelField("Downloading CSV...", EditorStyles.miniLabel);
                }
                else
                {
                    if (GUILayout.Button("Download"))
                    {
                        LastUpdate.SetValue(DateTime.UtcNow);
                        DownloadCSV(SheetID.GetValue());
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(15);


                EditorGUILayout.LabelField("Export Data", EditorStyles.boldLabel);
                ReadStyle.Render();
                ObjectsPart.Render();
                if (GUILayout.Button("Export CSV Data"))
                {
                    if (ObjectsPart.GetCount() == 0)
                    {
                        Debug.LogWarning("No Object to Export");
                        return;
                    }

                    foreach (var item in ObjectsPart.GetList())
                    {
                        if (item is ICSVDataTransfer csvItem)
                        {
                            csvItem.OnBeginTransfer(GetCSVFileName());
                            switch (ReadStyle.GetValue())
                            {
                                case CSVReadType.Row:
                                    int RowIndex = 0;
                                    foreach (var csvRow in GetCSVTable())
                                    {
                                        csvItem.OnRowTransfer(csvRow, RowIndex);
                                        RowIndex++;
                                    }
                                    break;
                                case CSVReadType.Column:
                                    List<string> column = new List<string>();
                                    int length = GetColumnCount();
                                    for (int i = 0; i < length; i++)
                                    {
                                        column.Clear();
                                        foreach (var csvRow in GetCSVTable())
                                        {
                                            column.Add(csvRow[i]);
                                        }
                                        csvItem.OnColumnTransfer(column.ToArray(), i);
                                    }
                                    break;
                            }
                            csvItem.OnEndTransfer(GetCSVFileName());
                            Debug.Log("Export CSV File Was Successfully!");
                        }
                        else
                        {
                            Debug.LogError($"The object {item.name} does not implement ICSVInterface.");
                        }
                    }
                }


                serializedObject.ApplyModifiedProperties();
            }
        }

        private bool IsCSVFile(string path)
        {
            return path.EndsWith(".csv");  // Change this to ".csv" if you want to target CSV files instead
        }
        private string GetCSVFileName()
        {
            return System.IO.Path.GetFileNameWithoutExtension(assetPath);
        }
        private List<string[]> GetCSVTable()
        {
            TextAsset textAsset = (TextAsset)target;
            return CSV.ParseCSV(textAsset.text);
        }
        private int GetColumnCount()
        {
            int maxColumnSize = 0;
            foreach (string[] item in GetCSVTable())
            {
                if (item.Length > maxColumnSize)
                {
                    maxColumnSize = item.Length;
                }
            }
            return maxColumnSize;
        }
        private void ReplaceCSVContent(TextAsset csv, string content)
        {
            string path = AssetDatabase.GetAssetPath(csv);

            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllText(path, content);
                AssetDatabase.Refresh(); // Refresh the asset database to apply changes
                Debug.Log("CSV file updated: " + path);
            }
            else
            {
                Debug.LogError("Invalid CSV file path.");
            }
        }
        private string MakeURL(string SheetID)
        {
            switch (OnlineSheetType.GetValue())
            {
                case SheetType.GoogleSheets:
                    string[] parts = SheetID.Split('>');
                    if (parts.Length != 2)
                    {
                        Debug.LogWarning("Invalid Sheet ID       Use:[SheetID>SheetGID]");
                        return "";
                    }
                    return $"https://docs.google.com/spreadsheets/d/{parts[0]}/export?format=csv&gid={parts[1]}";
                case SheetType.MicrosoftOneDrive:
                    if (SheetID.Contains("edit.aspx"))
                    {
                        return SheetID.Replace("edit.aspx", "download.aspx");
                    }
                    return SheetID;
                case SheetType.Dropbox:
                    if (SheetID.Contains("?dl=0"))
                    {
                        return SheetID.Replace("?dl=0", "?dl=1");
                    }
                    return SheetID;
                case SheetType.CustomSheet:
                    return SheetID;
                default:
                    return SheetID;
            }
        }


        async void DownloadCSV(string ID)
        {
            Isdownloading = true;

            string url = MakeURL(ID);

            using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(url))
            {
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                    await Task.Yield();

                if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error downloading CSV: " + request.error);
                }
                else
                {
                    TextAsset textAsset = (TextAsset)target;
                    string result = request.downloadHandler.text;
                    ReplaceCSVContent(textAsset, result);
                    Debug.Log("CSV Downloaded Successfully!");
                }


            }

            Isdownloading = false;
        }


    }
}

