namespace RealMethod
{
    public interface ITask
    {

    }

    public interface ITaskLife : ITask
    {
        /// <summary> Called when the task starts. </summary>
        void StartTask(float Duration = 0);
        /// <summary> Called every update or tick cycle. </summary>
        void UpdateTask();
        /// <summary> Called to pause the task temporarily. </summary>
        void StopTask();
        /// <summary> Called to Clear initating task. </summary>
        void ClearTask();
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

    public interface ITaskController : ITaskLife
    {
        /// <summary> Called to pause the task temporarily. </summary>
        void PauseTask();
        /// <summary> Called to resume the task after a pause. </summary>
        void ResumeTask();
        /// <summary> Called to reset the task propetries. </summary>
        void ResetTask();
        /// <summary> Resets task state (useful for pooling). </summary>
        void RestartTask(float Duration = 0);
        /// <summary> Whether the task is currently paused. </summary>
        bool IsPaused { get; }
    }

}