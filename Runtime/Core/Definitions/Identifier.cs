using System;

namespace RealMethod
{
    /// <summary>
    /// Represents a base contract for objects that can identify themselves.
    /// Provides access to the current instance through the <see cref="Self"/> property.
    /// This interface serves as the root for different identifier strategies.
    /// </summary>
    public interface IIdentifier
    {
        /// <summary>
        /// Gets the current instance of the implementing object.
        /// Useful for generic identifier handling where the object reference is needed.
        /// </summary>
        object Self => this;
    }
    /// <summary>
    /// Represents an identifier based on a human-readable name.
    /// The name acts as a unique or semi-unique identifier within a specific context.
    /// </summary>
    public interface INameIdentifier : IIdentifier
    {
        /// <summary>
        /// Gets the name used to identify the object.
        /// The value may be formatted using the <see cref="Slug"/> attribute.
        /// </summary>
        string SelfName { get; }
    }
    /// <summary>
    /// Represents an identifier based on a globally unique identifier (GUID).
    /// This ensures uniqueness across systems, sessions, and instances.
    /// </summary>
    public interface IGuidIdentifier : IIdentifier
    {
        /// <summary>
        /// Gets the globally unique identifier associated with the object.
        /// </summary>
        Guid SelfID { get; }
    }
}