using System;

namespace RealMethod
{
    public interface IFile : IIdentifier
    {
        DateTime CreateTime { get; }
        DateTime ModifiedTime { get; }
    }
}