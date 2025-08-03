using UnityEngine;

namespace RealMethod
{

    public interface ICommand
    {
        bool Initiate(Object author, Object owner);
        void ExecuteCommand(object Executer);
    }


    // A base one-shot command you can execute immediately.

    //A longer-living command with start, update, end — like an ability/effect.
    public interface ICommandLife
    {
        /// <summary> Called when the command starts. </summary>
        void StartCommand(float Duration = 0);
        /// <summary> Called every update or tick cycle. </summary>
        void UpdateCommand();
        /// <summary> Called to pause the command temporarily. </summary>
        void StopCommand();
        /// <summary> Called to Clear initating command. </summary>
        void ClearCommand();
        /// <summary> Elapsed time since command Live. </summary>
        float RemainingTime { get; }
        // <summary> Whether the command has finished execution. </summary>
        bool IsFinished { get; }
    }
    // A longer-living command that controled
    public interface ICommandBehaviour : ICommandLife
    {
        /// <summary> Called to pause the command temporarily. </summary>
        void PauseCommand();
        /// <summary> Called to resume the command after a pause. </summary>
        void ResumeCommand();
        /// <summary> Called to reset the command propetries. </summary>
        void ResetCommand();
        /// <summary> Resets command state (useful for pooling). </summary>
        void RestartCommand(float Duration = 0);
        /// <summary> Whether the command is currently paused. </summary>
        bool IsPaused { get; }
    }

}