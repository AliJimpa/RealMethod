using UnityEditor;
using System;
using System.Linq;
using RealMethod.Editor;

namespace RealMethod
{
    [CustomEditor(typeof(AbilityEffectAsset), true)]
    public class AbilityEffectEditor : UnityEditor.Editor
    {
        
        //private EP_List<X> Effects;
        private string[] effectTypeNames;
        private Type[] effectTypes;
        private int selectedIndex;

        private void OnEnable()
        {
            //Effects = new EP_List<EP_String>("Effects", this);
            effectTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(AbilityEffect)) && !t.IsAbstract)
                .ToArray();

            effectTypeNames = effectTypes.Select(t => t.Name).ToArray();
           
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // Ability container = (Ability)target;

            // Show popup to select effect type to add
            selectedIndex = EditorGUILayout.Popup("Effect Type", selectedIndex, effectTypeNames);
            //Effects.Render();

            // if (GUILayout.Button("Add New Effect"))
            // {
            //     Type selectedType = effectTypes[selectedIndex];
            //     AbilityEffect newEffect = (AbilityEffect)Activator.CreateInstance(selectedType);

            //     if (container.effects == null)
            //         //container.effects = new AbilityEffect[0];

            //     // Create a new list, add new effect, and assign back as array
            //     //var effectsList = container.effects.ToList();
            //     //effectsList.Add(newEffect);
            //     container.effects = effectsList.ToArray();

            //     EditorUtility.SetDirty(container);
            // }

            // EditorGUILayout.Space();
            // EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);

            // // Show all effects in array
            // if (container.effects != null)
            // {
            //     for (int i = 0; i < container.effects.Length; i++)
            //     {
            //         var effect = container.effects[i];

            //         if (effect == null) continue;

            //         EditorGUILayout.BeginVertical(GUI.skin.box);

            //         EditorGUILayout.BeginHorizontal();
            //         EditorGUILayout.LabelField($"{effect.GetType().Name} (Effect #{i})", EditorStyles.boldLabel);

            //         // Button to remove this effect
            //         if (GUILayout.Button("Remove", GUILayout.MaxWidth(60)))
            //         {
            //             var effectsList = container.effects.ToList();
            //             effectsList.RemoveAt(i);
            //             container.effects = effectsList.ToArray();

            //             EditorUtility.SetDirty(container);
            //             break; // Exit loop since array changed
            //         }
            //         EditorGUILayout.EndHorizontal();

            //         // Draw effect's own inspector
            //         //UnityEditor.Editor effectEditor = CreateEditor(container.effects[i]);
            //         //effectEditor.OnInspectorGUI();

            //         EditorGUILayout.EndVertical();
            //         EditorGUILayout.Space();
            //     }
            // }
            // else
            // {
            //     EditorGUILayout.LabelField("No effects added.");
            // }
        }
    }

}