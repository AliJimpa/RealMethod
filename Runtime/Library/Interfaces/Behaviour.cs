namespace RealMethod
{
    public interface IBehaviour
    {

    }
    public interface IBehaviourMethod : IBehaviour
    {
        /// <summary> Called when the task starts. </summary>
        void Start(float Duration = 0);
        /// <summary> Called every update or tick cycle. </summary>
        void Update();
        /// <summary> Called to pause the task temporarily. </summary>
        void Stop();
        /// <summary> Called to Clear initating task. </summary>
        void Clear();
        /// <summary> Has task infinit lifetime. </summary>
        bool IsInfinit { get; }
        /// <summary> Remain time since task Live. </summary>
        float RemainingTime { get; }
        /// <summary> Elapsed time since task Live. </summary>
        float ElapsedTime { get; }
        /// <summary> Normalized time since task Live. </summary>
        float NormalizedTime { get; }
        // <summary> Whether the task has finished execution. </summary>
        bool IsFinished { get; }
    }
    public interface IBehaviourAction : IBehaviourMethod
    {
        /// <summary> Called to pause the task temporarily. </summary>
        void Pause();
        /// <summary> Called to resume the task after a pause. </summary>
        void Resume();
        /// <summary> Called to reset the task propetries. </summary>
        void Reset();
        /// <summary> Resets task state (useful for pooling). </summary>
        void Restart(float Duration = 0);
        /// <summary> Whether the task is currently paused. </summary>
        bool IsPaused { get; }
    }

}