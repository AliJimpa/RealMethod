using System;
using System.Reflection;
using UnityEngine;

namespace RealMethod.Editor
{
    public class PureRule : CompileRule
    {
        // CompileRule MEthods
        protected override void Initilized()
        {
            throw new NotImplementedException();
        }
        public override void OnStartCheck(RuleExecutionMode mode)
        {
        }
        public override bool CanCheck(RuleExecutionMode mode, Type type)
        {
            if (mode == RuleExecutionMode.AfterCompilation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override void OnCheck(Type type)
        {
            foreach (var method in type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly))
            {
                // Ignore methods without PureAttribute
                if (!method.IsDefined(typeof(PureAttribute), false))
                    continue;

                // Check the calls inside the method
                if (!IsMethodPure(method))
                {
                    Debug.LogError(
                        $"PURE METHOD VIOLATION in {type.FullName}.{method.Name}():\n" +
                        $"→ This method is marked [Pure] but calls an impure method!"
                    );
                }
            }
        }


        private bool IsMethodPure(MethodInfo method)
        {
            var body = method.GetMethodBody();
            if (body == null)
                return true;

            var il = body.GetILAsByteArray();
            int pos = 0;

            while (pos < il.Length)
            {
                byte op = il[pos];

                // CALL or CALLVIRT opcodes
                if (op == 0x28 || op == 0x6F)
                {
                    int metadataToken = BitConverter.ToInt32(il, pos + 1);
                    var called = method.Module.ResolveMethod(metadataToken) as MethodInfo;

                    if (called != null)
                    {
                        // If called method is not marked [Pure] → error
                        if (!called.IsDefined(typeof(PureAttribute), false))
                            return false;
                    }
                }

                pos++;
            }

            return true;
        }
        private bool IsPure(MethodInfo method)
        {
            // 1. Explicitly marked as pure
            if (method.GetCustomAttribute<PureAttribute>() != null)
                return true;

            // 2. Implicit analysis
            return IsMethodImplicitlyPure(method);
        }
        private bool IsMethodImplicitlyPure(MethodInfo method)
        {
            // Any setter = NOT pure
            if (method.IsSpecialName && method.Name.StartsWith("set_"))
                return false;

            // Inspect instructions (Cecil would be ideal, but unavailable at runtime)
            // Simple heuristic: if method writes to fields, it's impure
            var body = method.GetMethodBody();
            if (body == null)
                return true; // e.g., extern methods are assumed pure

            var il = body.GetILAsByteArray();

            // Look for IL opcodes that indicate mutation
            // stfld     = write to field
            // stsfld    = write to static field
            // call/set  = property setter
            // callvirt  = method call (we check purity recursively)

            for (int i = 0; i < il.Length; i++)
            {
                byte code = il[i];

                // stfld or stsfld (field writes)
                if (code == 0x7D || code == 0x80)
                    return false;

                // call / callvirt: check called method purity
                if (code == 0x28 || code == 0x6F)
                {
                    // Read metadata token
                    int token = BitConverter.ToInt32(il, i + 1);
                    var calledMethod = method.Module.ResolveMethod(token) as MethodInfo;

                    if (calledMethod == null)
                        continue;

                    // If called method is not pure => this method is impure
                    if (!IsPure(calledMethod))
                        return false;
                }
            }

            return true;
        }




    }
}