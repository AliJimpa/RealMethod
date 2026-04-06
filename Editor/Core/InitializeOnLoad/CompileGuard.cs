using UnityEditor;
using UnityEditor.Compilation;
using System;
using UnityEngine;


namespace RealMethod.Editor
{
    /// <summary>
    /// Base class for creating custom compile-time or editor-time validation rules.
    /// Rules are instantiated by CompileGuard and triggered automatically depending
    /// on the selected <see cref="RuleExecutionMode"/>.
    /// </summary>
    public abstract class CompileRule
    {
        /// <summary>
        /// Defines when a <see cref="CompileRule"/> should be executed by <see cref="CompileGuard"/>.
        /// This controls which editor event triggers the rule validation.
        /// </summary>
        public enum RuleExecutionMode
        {
            /// <summary>
            /// Rule is disabled and will never be executed.
            /// </summary>
            Disable = 0,
            /// <summary>
            /// Rule runs after Unity finishes compiling scripts.
            /// Triggered by <see cref="CompilationPipeline.compilationFinished"/>.
            /// Useful for validating code structure or assets after compilation.
            /// </summary>
            AfterCompilation = 1,
            /// <summary>
            /// Rule runs on the editor update loop after the editor loads.
            /// Triggered using <see cref="EditorApplication.delayCall"/>.
            /// Useful for checks that should occur once the editor is ready.
            /// </summary>
            EditorStartup = 2
        }

        public CompileRule()
        {
            Initilized();
        }

        /// <summary>
        /// Called automatically right after the rule is constructed.
        /// Use this to initialize internal data, cache values, or set up
        /// anything needed before the rule is used by CompileGuard.
        /// </summary>
        protected abstract void Initilized();
        /// <summary>
        /// Defines when this rule should run during the editor lifecycle.
        /// The returned <see cref="RuleExecutionMode"/> determines whether
        /// CompileGuard triggers this rule during:
        /// - script compilation (CompilationPipeline), or
        /// - editor updates (EditorApplication).
        /// </summary>
        public abstract RuleExecutionMode GetRuleMode();
        /// <summary>
        /// Returns the base type that this rule should scan for.
        /// CompileGuard will call <see cref="OnCheck(Type)"/> for every type in the project
        /// that inherits from the returned base type.
        /// </summary>
        /// <returns>
        /// A Type that all target classes must derive from.
        /// </returns>
        public abstract Type GetBaseType();
        /// <summary>
        /// Called when CompileGuard finds a type that inherits from the rule's base type.
        /// Implement validation logic here. This method is invoked automatically for each
        /// matching type during the selected rule mode.
        /// </summary>
        /// <param name="type">
        /// The discovered type that matches <see cref="GetBaseType"/>.
        /// </param>
        public abstract void OnCheck(Type type);
    }


    [InitializeOnLoad]
    /// <summary>
    /// Central system that discovers and executes all CompileRule instances.
    /// It triggers rules during compilation or editor updates depending on their mode,
    /// and passes every project type that matches the rule's base type.
    /// </summary>
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
            CheckRuls(CompileRule.RuleExecutionMode.AfterCompilation);
        }
        private static void OnEditorUpdated()
        {
            CheckRuls(CompileRule.RuleExecutionMode.EditorStartup);
        }
        private static void CheckRuls(CompileRule.RuleExecutionMode mode)
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
