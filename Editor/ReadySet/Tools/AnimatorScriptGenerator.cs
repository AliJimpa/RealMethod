using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Collections.Generic;

namespace RealMethod.Editor
{
    public class AnimatorScriptGenerator
    {
        private static string outputFolder = "Assets/GeneratedAnimatorParams"; // change to your desired folder

        [MenuItem("Tools/RealMethod/General/AnimatorParam")]
        public static void GenerateScriptsFromAnimators()
        {
            ProjectSettingAsset TargetStorage;
            if (ProjectSettingWindow.GetSettingStorage(out TargetStorage))
            {
                outputFolder = TargetStorage.FindAddres(ProjectSettingAsset.IdentityAsset.AnimatorParam).Path;
            }
            else
            {
                Debug.LogError("First Setup RealMethod in Project! [ProjectSetting>RealMethod]");
                return;
            }


            // Ensure output folder exists
            if (!AssetDatabase.IsValidFolder(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                AssetDatabase.Refresh();
            }

            // Find all AnimatorController assets
            string[] guids = AssetDatabase.FindAssets("t:AnimatorController");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                AnimatorController animator = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

                if (animator != null)
                {
                    CreateScriptForAnimator(animator);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("✅ Animator scripts generated in " + outputFolder);
        }

        private static void CreateScriptForAnimator(AnimatorController animator)
        {
            string className = animator.name.Replace(" ", "_"); // Safe class name
            string scriptPath = Path.Combine(outputFolder, className + ".cs");
            string scriptContent =
        $@"using UnityEngine;

public static class {className}
{{
public class Parameters
{{
{GetParamaterList(animator)}
{GetParameterValue(animator)}
}}
public class Layer
{{
{GetLayerList(animator)}
{GetLayerValue(animator)}
}}
public class State
{{
{GetStateList(animator)}
{GetStateValue(animator)}
}}
public class Tag
{{
{GetTagList(animator)}
{GetTagValue(animator)}
}}
public class Transitions
{{
{GetTransitionsList(animator)}
{GetTransitionsValue(animator)}
}}
}}";
            // Always overwrite
            File.WriteAllText(scriptPath, scriptContent);
            Debug.Log($"✅ Script created/updated for {className}");
        }




        private static System.Text.StringBuilder GetParamaterList(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.AppendLine($"public static readonly string[] list = new string[{animator.parameters.Length}] {{");
            foreach (var param in animator.parameters)
            {
                result.AppendLine($" \"{param.name}\",");
            }
            result.AppendLine("};");
            return result;
        }
        private static System.Text.StringBuilder GetParameterValue(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            foreach (var param in animator.parameters)
            {

                string safeParamName = param.name.Replace(" ", "_");
                result.AppendLine($"    public static readonly int {safeParamName} = Animator.StringToHash(\"{param.name}\");");
            }
            return result;
        }
        private static System.Text.StringBuilder GetLayerList(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            result.AppendLine($"public static readonly string[] list = new string[{animator.layers.Length}] {{");
            foreach (var param in animator.layers)
            {
                result.AppendLine($" \"{param.name}\",");
            }
            result.AppendLine("};");
            return result;
        }
        private static System.Text.StringBuilder GetLayerValue(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            foreach (var layer in animator.layers)
            {
                string safeLayerName = layer.name.Replace(" ", "_");
                result.AppendLine(
                    $"    public static readonly int {safeLayerName} = Animator.StringToHash(\"{layer.name}\");");
            }
            return result;
        }
        private static System.Text.StringBuilder GetStateList(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            int count = 0;
            foreach (var layer in animator.layers)
            {
                foreach (var state in layer.stateMachine.states)
                {
                    count++;
                }
            }
            result.AppendLine($"public static readonly string[] list = new string[{count}] {{");
            foreach (var layer in animator.layers)
            {
                foreach (var sta in layer.stateMachine.states)
                {
                    result.AppendLine($" \"{sta.state.name}\",");
                }
            }
            result.AppendLine("};");
            return result;
        }
        private static System.Text.StringBuilder GetStateValue(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            foreach (var layer in animator.layers)
            {
                //string layerPrefix = layer.name.Replace(" ", "_") + "_";
                foreach (var state in layer.stateMachine.states)
                {
                    string safeStateName = state.state.name.Replace(" ", "_");
                    result.AppendLine($"    public static readonly int {safeStateName} = Animator.StringToHash(\"{state.state.name}\");");
                }
            }
            return result;
        }
        private static System.Text.StringBuilder GetTagList(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            HashSet<string> uniqueTags = new HashSet<string>();
            foreach (var layer in animator.layers)
            {
                var stateMachine = layer.stateMachine;
                foreach (var state in stateMachine.states)
                {
                    if (!string.IsNullOrEmpty(state.state.tag))
                        uniqueTags.Add(state.state.tag);
                }
            }
            result.AppendLine($"public static readonly string[] list = new string[{uniqueTags.Count}] {{");
            foreach (string tag in uniqueTags)
            {
                result.AppendLine($" \"{tag}\",");
            }
            result.AppendLine("};");
            return result;
        }
        private static System.Text.StringBuilder GetTagValue(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            HashSet<string> uniqueTags = new HashSet<string>();
            foreach (var layer in animator.layers)
            {
                var stateMachine = layer.stateMachine;
                foreach (var state in stateMachine.states)
                {
                    if (!string.IsNullOrEmpty(state.state.tag))
                        uniqueTags.Add(state.state.tag);
                }
            }
            foreach (string tag in uniqueTags)
            {
                string safeTagName = tag.Replace(" ", "_");
                result.AppendLine($"    public static readonly string {safeTagName} = \"{tag}\";");
            }
            return result;
        }
        private static System.Text.StringBuilder GetTransitionsList(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();

            HashSet<string> uniqueTags = new HashSet<string>();
            foreach (var layer in animator.layers)
            {
                var stateMachine = layer.stateMachine;
                foreach (var state in stateMachine.states)
                {
                    foreach (var transition in state.state.transitions)
                    {
                        if (!string.IsNullOrEmpty(transition.name))
                            uniqueTags.Add(transition.name);
                    }
                }
            }
            result.AppendLine($"public static readonly string[] list = new string[{uniqueTags.Count}] {{");
            foreach (string transition in uniqueTags)
            {
                result.AppendLine($" \"{transition}\",");
            }
            result.AppendLine("};");
            return result;
        }
        private static System.Text.StringBuilder GetTransitionsValue(AnimatorController animator)
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder();
            HashSet<string> uniqueTags = new HashSet<string>();
            foreach (var layer in animator.layers)
            {
                var stateMachine = layer.stateMachine;
                foreach (var state in stateMachine.states)
                {
                    foreach (var transition in state.state.transitions)
                    {
                        if (!string.IsNullOrEmpty(transition.name))
                            uniqueTags.Add(transition.name);
                    }
                }
            }
            foreach (string transition in uniqueTags)
            {
                string safeTransitionName = transition.Replace(" ", "_");
                result.AppendLine($"    public static readonly string {safeTransitionName} = \"{transition}\";");
            }
            return result;
        }




    }
}