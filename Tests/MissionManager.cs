using System;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Represents the possible outcomes of a Game process.
    /// </summary>
    public enum GameProcess
    {
        /// <summary>
        /// The process finished successfully.
        /// </summary>
        Success,
        /// <summary>
        /// The process failed.
        /// </summary>
        Failure,
        /// <summary>
        /// The process was cancelled before completion.
        /// </summary>
        Cancelled
    }

    public class MissionManager2 : MonoBehaviour, IGameManager
    {
        /// <summary>
        /// Invoked when the process finishes.
        /// Process in your game take define with yourelf.
        /// (for example: show win screen, game over UI, load next level, etc).
        /// </summary>
        public static event Action<GameProcess> OnCompleted;

        public void InitiateManager(Scope owner)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Attempts to invoke 
        /// </summary>
        public void Complete(GameProcess result)
        {
            OnCompleted.Invoke(result);
        }


    }
}