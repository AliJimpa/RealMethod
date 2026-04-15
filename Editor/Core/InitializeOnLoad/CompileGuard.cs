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
            /// Rule runs after Unity finishes compiling scripts.
            /// Triggered by <see cref="CompilationPipeline.compilationFinished"/>.
            /// Useful for validating code structure or assets after compilation.
            /// </summary>
            AfterCompilation = 0,
            /// <summary>
            /// Rule runs on the editor update loop after the editor loads.
            /// Triggered using <see cref="EditorApplication.delayCall"/>.
            /// Useful for checks that should occur once the editor is ready.
            /// </summary>
            EditorStartup = 1,
            /// <summary>
            /// Rule runs after the editor has fully returned to Edit Mode.
            /// The editor is back to its normal editing state.
            /// </summary>
            EnteredEditMode = 2,
            /// <summary>
            /// Rule runs when the editor is about to leave Edit Mode and start entering Play Mode.
            /// This happens before Play Mode is fully active.
            /// </summary>
            ExitingEditMode = 3,
            /// <summary>
            /// Rule runs after the editor has fully entered Play Mode.
            /// Game logic is now running.
            /// </summary>
            EnteredPlayMode = 4,
            /// <summary>
            /// Rule runs when the editor is about to stop Play Mode and return to Edit Mode.
            /// Game execution is shutting down.
            /// </summary>
            ExitingPlayMode = 5,
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
        /// Determines whether this rule can perform a check using the specified
        /// execution mode and target type. Override this method in derived classes
        /// to define which modes and types the rule supports.
        /// </summary>
        /// <param name="mode">The rule execution mode being requested.</param>
        /// <param name="type">The target data type to evaluate.</param>
        /// <returns>
        /// True if the rule can check the given mode and type; otherwise, false.
        /// </returns>
        public abstract bool CanCheck(RuleExecutionMode mode, Type type);
        /// <summary>
        /// Called when CompileGuard finds a type that inherits from the rule's base type.
        /// Implement validation logic here. This method is invoked automatically for each
        /// matching type during the selected rule mode.
        /// </summary>
        /// <param name="type">
        /// The discovered type that matches <see cref="CanCheck"/>.
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
        private static CompileRule[] Rules;
        private static bool initialized;

        public static readonly Type[] DefaultRuls = new Type[3] {
        // Array of ruls to that should be run always for RealMethod
        typeof(ConfigAssetsRule),
        typeof(ServiceRule),
        typeof(PrimitveAssetsRule),

        };


        static CompileGuard()
        {
            if (!ProjectSetting.CanCompile())
            {
                return;
            }

            if (initialized) return;
            initialized = true;

            CompilationPipeline.compilationFinished += OnCompilationFinished;
            EditorApplication.delayCall += OnEditorUpdated;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        private static void OnCompilationFinished(object obj)
        {
            CheckRuls(CompileRule.RuleExecutionMode.AfterCompilation);
        }
        private static void OnEditorUpdated()
        {
            CheckRuls(CompileRule.RuleExecutionMode.EditorStartup);
        }
        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            int index = (int)state + 2;
            CheckRuls((CompileRule.RuleExecutionMode)index);
        }


        // Checking process
        private static void CheckRuls(CompileRule.RuleExecutionMode mode)
        {
            if (ProjectSetting == null)
                return;


            if (Rules == null)
            {
                Type[] RulsClass = ProjectSetting.GetCompileRules();
                if (RulsClass == null)
                {
                    Debug.LogError($"ProjectSetting didn't have Compilerule but compileguard is enable");
                    return;
                }

                Rules = new CompileRule[RulsClass.Length];
                for (int i = 0; i < RulsClass.Length; i++)
                {
                    if (RulsClass[i] == null)
                        continue;

                    if (typeof(CompileRule).IsAssignableFrom(RulsClass[i]))
                    {
                        try
                        {
                            Rules[i] = (CompileRule)Activator.CreateInstance(RulsClass[i]);
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

            // Checking
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
                    foreach (var rule in Rules)
                    {
                        if (rule == null)
                            continue;
                        if (rule.CanCheck(mode, type) == false)
                            continue;

                        rule.OnCheck(type);
                    }
                }
            }
        }
    }


}
