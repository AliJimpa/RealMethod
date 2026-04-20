using System;

namespace RealMethod
{
    public interface IFile : IIdentifier
    {
        string Name { get; }
        DateTime CreateTime { get; }
        DateTime ModifiedTime { get; }
        object GetObject();
    }
}