using System;
using System.Reflection;
using UnityEngine;

namespace RealMethod.Editor
{
    public class ServiceRule : CompileRule
    {
        // CompileRule Methods
        protected override void Initilized()
        {

        }
        public override RuleExecutionMode GetRuleMode()
        {
            return RuleExecutionMode.AfterCompilation;
        }
        public override Type GetBaseType()
        {
            return typeof(Service);
        }
        public override void OnCheck(Type type)
        {
            // Check if class is static
            if (type.IsAbstract && type.IsSealed)
            {
                Debug.LogError($"Service '{type.FullName}' cannot be static.");
                return;
            }

            // Check for static methods
            var methods = type.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly
            );

            foreach (var method in methods)
            {
                if (method.IsStatic)
                {
                    Debug.LogError($"Service '{type.FullName}' contains static method '{method.Name}'. Static methods are not allowed in Service classes."
                    );
                }
            }
        }


    }
}