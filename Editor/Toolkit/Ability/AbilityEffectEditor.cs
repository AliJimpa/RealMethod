using UnityEditor;
using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace RealMethod.Editor
{
    [CustomEditor(typeof(AbilityEffectAsset), true)]
    public class AbilityEffectEditor : UnityEditor.Editor
    {
        private EP_List<EP_Dropdown> EffectList;
        private int selectedIndex;
        private string[] abilityNames;
        private AbilityEffectAsset MyAsset;

        private void OnEnable()
        {
            MyAsset = (AbilityEffectAsset)target;

            // Get all types that inherit from AbilityEffect
            abilityNames = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(AbilityEffect).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(t => t.Name)
                .OrderBy(n => n)
                .ToArray();

            // Create List
            EffectList = new EP_List<EP_Dropdown>("Effects", this);
            if (MyAsset.effects != null)
            {
                for (int i = 0; i < MyAsset.effects.Length; i++)
                {
                    EP_Dropdown poop = new EP_Dropdown(i.ToString(), abilityNames, this);
                    poop.SetValue(abilityNames.FindIndex(MyAsset.effects[i]));
                    EffectList.AddItem(poop);
                }
            }
            EffectList.OnItemChange += OnDropdownChange;
        }
        private void OnDisable()
        {
            EffectList.OnItemChange -= OnDropdownChange;
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Draw default inspector
            DrawDefaultInspector();

            // Dropdown
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                int count = MyAsset.effects != null ? MyAsset.effects.Length : 0;
                EffectList.AddItem(new EP_Dropdown(count.ToString(), abilityNames, this));
                RefreshEffect();
            }
            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                EffectList.RemoveItem(EffectList.GetCount() - 1);
                RefreshEffect();
            }
            GUI.enabled = false;
            EditorGUILayout.IntField("Count:", EffectList.GetCount());
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            EffectList.Render();



            serializedObject.ApplyModifiedProperties();
        }


        private void OnDropdownChange(EP_Dropdown item)
        {
            if (int.TryParse(item.PropertyName, out int value))
            {
                MyAsset.effects[value] = abilityNames[item.GetValue()];
            }
            else
            {
                Debug.LogWarning($"Invalid number format, Clear {MyAsset}");
            }
        }
        private void RefreshEffect()
        {
            List<EP_Dropdown> popplist = EffectList.GetList();
            string[] result = new string[popplist.Count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = abilityNames[popplist[i].GetValue()];
            }
            MyAsset.effects = result;
        }


    }

}