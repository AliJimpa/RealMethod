namespace RealMethod
{
    /// <summary>
    /// Represents the root interface for any system that exposes 
    /// an external control handle. 
    /// 
    /// This interface itself defines no functionality; instead, it acts 
    /// as a conceptual marker for objects whose execution, lifecycle, 
    /// or behavior can be controlled through derived interfaces such as 
    /// IHandleTrigger, IHandleLifecycle, IHandleTimed, or IHandleAction.
    /// 
    /// In other words, IHandle identifies a controllable unit that 
    /// provides a unified access point for interacting with the 
    /// underlying process, behavior, or action.
    /// </summary>
    public interface IHandle : IIdentifier
    {
    }

    public interface IHandleTrigger : IHandle
    {
        void Trigger();
        void Cancel();
    }
    public interface IHandleBehaviour : IHandle
    {
        /// <summary> Called when the Behaviour starts. </summary>
        void Start();
        /// <summary> Called to stop the Behaviour. </summary>
        void Stop();
        /// <summary> Called to Clear Behaviour. </summary>
        void Clear();
        /// <summary> Whether the Behaviour is currently started. </summary>
        bool IsStarted { get; }
    }
    public interface IHandleBehaviourCycle : IHandleBehaviour
    {
        /// <summary> Called when the Behaviour starts with override Time. </summary>
        void Start(float Duration);
        // <summary> Whether the Behaviour has finished execution. </summary>
        bool IsFinished { get; }
        /// <summary> Has Behaviour infinit lifetime. </summary>
        bool IsInfinit { get; }
        /// <summary> Remain time since Behaviour Live. </summary>
        float RemainingTime { get; }
        /// <summary> Elapsed time since Behaviour Live. </summary>
        float ElapsedTime { get; }
        /// <summary> Normalized time since Behaviour Live. </summary>
        float NormalizedTime { get; }
    }
    public interface IHandleBehaviourAction : IHandleBehaviourCycle
    {
        /// <summary> Called to pause the Behaviour temporarily. </summary>
        void Pause();
        /// <summary> Called to resume the Behaviour after a pause. </summary>
        void Resume();
        /// <summary> Called to reset the Behaviour propetries. </summary>
        void Reset();
        /// <summary> Resets Behaviour state (useful for pooling). </summary>
        void Restart(float Duration = 0);
        /// <summary> Whether the Behaviour is currently paused. </summary>
        bool IsPaused { get; }
    }


}