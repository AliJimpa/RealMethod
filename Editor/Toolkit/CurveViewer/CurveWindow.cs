using UnityEngine;
using UnityEditor;

namespace RealMethod.Editor
{
    public class CurveWindow : EditorWindow
    {
        private CurveAsset animationCurve;
        private float inputTime = 0f;
        private float inputValue = 0f;
        private float outputValue = 0f;
        private float outputTime = 0f;

        [MenuItem("Tools/RealMethod/Kit/CurveViewer")]
        public static void ShowWindow()
        {
            GetWindow<CurveWindow>("CurveViewer");
        }

        private void OnGUI()
        {
            GUILayout.Label("CurveEditor", EditorStyles.boldLabel);

            // Animation Curve Field
            EditorGUILayout.LabelField("Animation Curve:");
            animationCurve = (CurveAsset)EditorGUILayout.ObjectField("CurveAsset", animationCurve, typeof(CurveAsset), false);

            if (animationCurve == null)
            {
                EditorGUILayout.HelpBox("Please assign an AnimationCurveSO to edit.", MessageType.Warning);
                return;
            }

            // Input Fields
            EditorGUILayout.Space();
            inputTime = EditorGUILayout.FloatField("Input Time", inputTime);
            inputValue = EditorGUILayout.FloatField("Input Value", inputValue);

            // Buttons and Operations
            EditorGUILayout.Space();

            if (GUILayout.Button("Get Value by Time"))
            {
                outputValue = animationCurve.GetValue(inputTime);
                //Debug.Log($"Value at time {inputTime}: {outputValue}");
            }

            if (GUILayout.Button("Get Time by Value"))
            {
                outputTime = animationCurve.GetTime(inputValue);
                //Debug.Log($"Time at value {inputValue}: {outputTime}");
            }

            // Output Display
            EditorGUILayout.Space();
            EditorGUILayout.LabelField($"Output Value (by Time): {outputValue}");
            EditorGUILayout.LabelField($"Output Time (by Value): {outputTime}");

            // Save Changes
            if (GUI.changed)
            {
                EditorUtility.SetDirty(animationCurve);
            }
        }


    }





}