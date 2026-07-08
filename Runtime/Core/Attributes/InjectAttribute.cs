namespace RealMethod
{
    using System;

    [AttributeUsage(
        AttributeTargets.Field |
        AttributeTargets.Property |
        AttributeTargets.Method |
        AttributeTargets.Constructor)]
    public sealed class InjectAttribute : Attribute
    {
    }

}