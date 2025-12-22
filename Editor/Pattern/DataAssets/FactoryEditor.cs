using System;
using System.Linq;
using UnityEditor;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(FactoryAsset), true)]
    public class FactoryEditor : UnityEditor.Editor
    {
        private FactoryAsset TargetAsset;
        private string[] actionNames;
        private object instance;

        // UnityMethods
        private void OnEnable()
        {
            TargetAsset = (FactoryAsset)target;

            // Get all types that inherit from AbilityEffect
            actionNames = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(FactoryProduct).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => t.Name)
                .OrderBy(n => n)
                .ToArray();
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw default inspector
            DrawDefaultInspector();
        }
        private void OnDisable()
        {
        }


        private void RenderClass(Type classType)
        {
            if (classType == null)
            {
                EditorGUILayout.HelpBox($"Class ({classType}) not found.", MessageType.Error);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField($"{classType}", EditorStyles.boldLabel);

                if (instance == null || instance.GetType() != classType)
                {
                    instance = Activator.CreateInstance(classType);
                }


                //DrawSerializedFields(targetType, instance);
            }
        }
    }
}