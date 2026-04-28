using System;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// [Pure]
    /// float A() => 1;
    /// [Pure]
    /// float B() => A(); // allowed
    /// /////////////
    /// float Helper() => 10;  // NOT PURE
    /// [Pure]
    /// float C() => Helper(); // ❌ violation!
    /// </code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PureAttribute : Attribute
    {
        // Empty: marker attribute only
    }
}