using UnityEditor;
using UnityEditor.Compilation;
using System;
using UnityEngine;


namespace RealMethod.Editor
{
    public abstract class CompileRule
    {
        public enum RuleMode
        {
            Disable = 0,
            CompilationPipeline = 1,
            EditorApplication = 2
        }

        public CompileRule()
        {
            Initilized();
        }


        protected abstract void Initilized();
        public abstract RuleMode GetRuleMode();
        public abstract Type GetBaseType();
        public abstract void OnCheck(Type type);
    }


    [InitializeOnLoad]
    public static class CompileGuard
    {
        private static ProjectSettingAsset ProjectSetting_Cache;
        private static ProjectSettingAsset ProjectSetting
        {
            get
            {
                if (ProjectSetting_Cache == null)
                {
                    if (!RM_Editor.TryGetSettingAsset(out ProjectSetting_Cache))
                    {
                        return null;
                    }
                }
                return ProjectSetting_Cache;
            }
        }
        private static System.Reflection.Assembly[] Assemblies;
        private static CompileRule[] Ruls;

        public static Type[] DefaultRuls = new Type[1] {
        // Array of ruls to that should be run always for RealMethod
        typeof(ConfigAssetsRule),
        };


        static CompileGuard()
        {
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            EditorApplication.delayCall += OnEditorUpdated;
        }

        private static void OnCompilationFinished(object obj)
        {
            CheckRuls(CompileRule.RuleMode.CompilationPipeline);
        }
        private static void OnEditorUpdated()
        {
            CheckRuls(CompileRule.RuleMode.EditorApplication);
        }
        private static void CheckRuls(CompileRule.RuleMode mode)
        {
            if (ProjectSetting == null)
                return;

            if (Ruls == null)
            {
                Type[] RulsClass = ProjectSetting.GetCompileRules();
                if (RulsClass == null)
                    return;

                Ruls = new CompileRule[RulsClass.Length];
                for (int i = 0; i < RulsClass.Length; i++)
                {
                    if (RulsClass[i] == null)
                        continue;

                    if (typeof(CompileRule).IsAssignableFrom(RulsClass[i]))
                    {
                        try
                        {
                            Ruls[i] = (CompileRule)Activator.CreateInstance(RulsClass[i]);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Failed to instantiate {RulsClass[i]}: {ex.Message}.");
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError($"Type {RulsClass[i]} is not assignable to CompileRule.");
                        return;
                    }
                }
            }

            if (Assemblies == null)
            {
                Assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            Type[] types;
            foreach (var assembly in Assemblies)
            {
                try
                {
                    types = assembly.GetTypes();
                }
                catch
                {
                    continue;
                }

                foreach (var type in types)
                {
                    if (Ruls == null)
                        return;
                        
                    foreach (var rule in Ruls)
                    {
                        if (rule == null)
                            continue;
                        if (rule.GetRuleMode() != mode)
                            continue;

                        if (type.IsSubclassOf(rule.GetBaseType()))
                        {
                            rule.OnCheck(type);
                        }
                    }
                }
            }
        }
    }


}
