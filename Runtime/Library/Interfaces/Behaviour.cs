namespace RealMethod
{
    public interface IBehaviour
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
    public interface IBehaviourCycle : IBehaviour , ITick
    {
        /// <summary> Called when the Behaviour starts with override Time. </summary>
        void Start(float overrideTime);
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
    public interface IBehaviourAction : IBehaviourCycle
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