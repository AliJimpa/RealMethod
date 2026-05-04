using System;
using System.Collections.Generic;

namespace RealMethod
{
    /// <summary>
    /// Defines the contract for a service that can respond to lifecycle events.
    /// </summary>
    public interface IRegistrable
    {
        /// <summary>
        /// Called when the service is created.
        /// </summary>
        /// <param name="author">The object responsible for creating the service.</param>
        void OnRegister();
        /// <summary>
        /// Called when the service is deleted or destroyed.
        /// </summary>
        /// <param name="author">The object responsible for deleting the service.</param>
        void OnUnregister();
    }

    
    public abstract class GameModule : IInspectorInfo
    {
        protected static readonly Dictionary<Type, WeakReference<object>> Repository = new();

#if UNITY_EDITOR
        string IInspectorInfo.GetInfo()
        {
            return GetInspectorInfor();
        }
        protected abstract string GetInspectorInfor();
#endif
    }
}