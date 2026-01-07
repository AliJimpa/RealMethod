using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace RealMethod.Editor
{
    [System.Serializable]
    public class ClassTreeNode
    {
        public string name;
        public string toturial; // text after ->
        public string link; // text afeter https
        public List<ClassTreeNode> children = new List<ClassTreeNode>();
    }

    public class ClassViewerWindow : EditorWindow
    {
        private List<ClassTreeNode> rootNodes = new List<ClassTreeNode>();
        private Dictionary<ClassTreeNode, bool> foldoutStates = new Dictionary<ClassTreeNode, bool>();

        private TextAsset textFile;
        private Vector2 scrollPos;
        private string searchQuery = "";

        [MenuItem("Tools/RealMethod/ClassViewer")]
        public static void Open()
        {
            GetWindow<ClassViewerWindow>("ClassViewer");
        }

        // Unity Methods
        private void OnEnable()
        {
            string ClassViewPath = "Assets/Realmethod/Documentation/Information/ClassViewer.txt"; // Just for Test
            // string ClassViewPath = Path.Combine(RM_CoreEditor.Documentation, "ClassViewer.txt");
            if (!File.Exists(ClassViewPath))
            {
                Debug.LogError($"ClassView file not found: {ClassViewPath}");
                Close();
            }
            string ClassFile = File.ReadAllText(ClassViewPath);
            LoadFromText(ClassFile);
        }
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Collaps"))
            {
                foreach (var node in rootNodes)
                {
                    FoldLine(node, false);
                }
            }
            if (GUILayout.Button("Expanded"))
            {
                foreach (var node in rootNodes)
                {
                    FoldLine(node, true);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            searchQuery = EditorGUILayout.TextField("Search", searchQuery);

            EditorGUILayout.Space();

            // 🔽 Scroll View
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            foreach (var node in rootNodes)
            {
                DrawNodeWithSearch(node, 0);
            }

            EditorGUILayout.EndScrollView();
            // 🔼 Scroll View
        }

        private void LoadFromText(string text)
        {
            rootNodes.Clear();
            foldoutStates.Clear();

            string[] lines = text.Split('\n');
            Stack<ClassTreeNode> stack = new Stack<ClassTreeNode>();

            foreach (string rawLine in lines)
            {
                if (string.IsNullOrWhiteSpace(rawLine))
                    continue;

                int indent = CountIndent(rawLine);
                string trimmed = rawLine.Trim();

                string nodeName = trimmed;
                string endText = null;
                string linkText = null;

                // 🔹 Parse "-> End Text"
                string middleText = null;
                int arrowIndex = trimmed.IndexOf("->");
                if (arrowIndex >= 0)
                {
                    nodeName = trimmed.Substring(0, arrowIndex).Trim();
                    middleText = trimmed.Substring(arrowIndex + 2).Trim();
                }

                if (middleText != null)
                {
                    int linkindex = middleText.IndexOf("https");
                    if (linkindex >= 0)
                    {
                        endText = middleText.Substring(0, linkindex).Trim();
                        linkText = middleText.Substring(linkindex).Trim();
                    }
                    else
                    {
                        endText = middleText;
                    }
                }

                ClassTreeNode node = new ClassTreeNode
                {
                    name = nodeName,
                    toturial = endText,
                    link = linkText
                };

                if (indent == 0)
                {
                    rootNodes.Add(node);
                    stack.Clear();
                    stack.Push(node);
                }
                else
                {
                    while (stack.Count > indent)
                        stack.Pop();

                    stack.Peek().children.Add(node);
                    stack.Push(node);
                }
            }
        }
        private int CountIndent(string line)
        {
            int spaces = 0;
            foreach (char c in line)
            {
                if (c == ' ')
                    spaces++;
                else
                    break;
            }
            return spaces / 2; // 2 spaces = 1 level
        }

        private bool DrawNodeWithSearch(ClassTreeNode node, int indent)
        {
            // Check if node matches search
            bool nodeMatches = string.IsNullOrEmpty(searchQuery) ||
                               node.name.ToLower().Contains(searchQuery.ToLower()) ||
                               node.toturial?.ToLower().Contains(searchQuery.ToLower()) == true;

            // Check if any child matches
            bool anyChildMatches = false;
            foreach (var child in node.children)
            {
                if (NodeOrChildrenMatchSearch(child))
                {
                    anyChildMatches = true;
                    break;
                }
            }

            // If nothing matches, skip
            if (!nodeMatches && !anyChildMatches)
                return false;

            // Draw parent node
            if (!foldoutStates.ContainsKey(node))
                foldoutStates[node] = true;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indent * 20);
            foldoutStates[node] =
                EditorGUILayout.Foldout(foldoutStates[node], node.name, true);
            EditorGUILayout.EndHorizontal();

            // Show endText as link if expanded
            if (foldoutStates[node] && !string.IsNullOrEmpty(node.toturial))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(indent * 20);
                EditorGUILayout.BeginVertical("box");
                GUIStyle richLabel = new GUIStyle(EditorStyles.wordWrappedLabel);
                richLabel.richText = true;
                GUILayout.Label(node.toturial, richLabel);
                //GUILayout.Label(node.toturial, new GUIStyle(EditorStyles.wordWrappedLabel));
                if (!string.IsNullOrEmpty(node.link))
                {
                    if (GUILayout.Button("Link", EditorStyles.linkLabel))
                    {
                        OnEndTextClicked(node);
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }



            // Draw children recursively only if expanded
            if (foldoutStates[node])
            {
                foreach (var child in node.children)
                {
                    DrawNodeWithSearch(child, indent + 1);
                }
            }

            return true;
        }
        // Helper: check recursively if a node or any child matches search
        private bool NodeOrChildrenMatchSearch(ClassTreeNode node)
        {
            bool match = string.IsNullOrEmpty(searchQuery) ||
                         node.name.ToLower().Contains(searchQuery.ToLower()) ||
                         node.toturial?.ToLower().Contains(searchQuery.ToLower()) == true;

            if (match)
                return true;

            foreach (var child in node.children)
            {
                if (NodeOrChildrenMatchSearch(child))
                    return true;
            }

            return false;
        }
        private void FoldLine(ClassTreeNode node, bool result)
        {
            foldoutStates[node] = result;
            if (foldoutStates[node])
            {
                foreach (var child in node.children)
                    FoldLine(child, result);
            }
        }
        private void OnEndTextClicked(ClassTreeNode node)
        {
            if (EditorUtility.DisplayDialog("OpenLink", node.link, "Open", "Cancel"))
            {
                // 3️⃣ OR open URL (if endText is a URL)
                Application.OpenURL(node.link);
            }
            // 2️⃣ OR ping asset
            // EditorGUIUtility.PingObject(yourObject);
        }

    }
}
