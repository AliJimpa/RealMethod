using System;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// [Button]
    /// private void DoSomething()
    /// {
    ///     Debug.Log("DoSomething called from Inspector!");
    /// }
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public sealed class ButtonAttribute : Attribute { }
}


