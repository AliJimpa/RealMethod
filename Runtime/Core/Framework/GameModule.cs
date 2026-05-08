using System;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Abstract base class for game modules, providing lifecycle management and integration with the Game system.
    /// A <see cref="GameModule"/> is instantiated once per module type
    /// and automatically registered by Scope"/>.
    /// Implements IBridge, IInspectorInfo, and IDisposable interfaces.
    /// </summary>
    public abstract class GameModule : IBridge, IInspectorInfo, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the GameModule class.
        /// Checks if the game is initialized and binds the module to the Game.Bridge.
        /// Calls the OnBegin method for initialization.
        /// </summary>
        public GameModule()
        {
            // Check if you game not initialized
            if (!Game.IsGameInitialized)
            {
                Debug.LogWarning($"Game doesn't initialized !");
                return;
            }
            Game.Bridge.Bind(this);
            OnBegin();
        }

        // Implement IBridge Interface
        void IBridge.OnWorldChanged(World world) => OnWorldChanged();
        void IDisposable.Dispose()
        {
            Game.Bridge.Unbind(this);
            OnEnd();
        }

        // Abstraction Methods
        protected abstract void OnBegin();
        protected abstract void OnWorldChanged();
        protected abstract void OnEnd();

#if UNITY_EDITOR
        /// <summary>
        /// Provides information for the Unity Inspector in the editor.
        /// </summary>
        /// <returns>A string containing inspector information.</returns>
        string IInspectorInfo.GetInfo()
        {
            return GetInspectorInfo();
        }
        /// <summary>
        /// Can be overridden to provide custom information for the Unity Inspector.
        /// Default implementation returns null.
        /// </summary>
        /// <returns>A string containing custom inspector information.</returns>
        protected virtual string GetInspectorInfo()
        {
            return null;
        }
#endif
    }
}