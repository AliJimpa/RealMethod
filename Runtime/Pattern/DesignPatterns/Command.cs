using System;
using UnityEngine;

namespace RealMethod
{
    // A base one-shot command
    public interface ICommandInitiator
    {
        void Initiate(object author, MonoBehaviour owner);
    }
    public abstract class Command : MonoBehaviour, ICommandInitiator
    {
        // Implement ICommandInitiator Interface
        public void Initiate(object author, MonoBehaviour owner)
        {
            OnInitiate(author, owner);
        }

        // Abstract Methods
        protected abstract void OnInitiate(object author, MonoBehaviour owner);
    }



    // A base one-shot command you can execute immediately.
    public interface ICommandExecuter
    {
        void ExecuteCommand(object Executer);
    }
    public abstract class ExecutCommand : Command, ICommandExecuter
    {
        // Implement ICommandExecuter Interface
        void ICommandExecuter.ExecuteCommand(object Executer)
        {
            if (enabled)
                Execute(Executer);
        }

        // Abstract Methods
        protected abstract void Execute(object Owner);
    }
    // A command targeted at a specific type (e.g., a component).
    public abstract class TargetedCommand<T> : ExecutCommand where T : MonoBehaviour
    {
        public T MyOwner { get; private set; }

        // Override Methods
        protected override void OnInitiate(object author, MonoBehaviour owner)
        {
            if (owner is T MyOwner)
            {
                OnInitiate(author);
            }
            else
            {
                Debug.LogWarning($"Command '{GetType().Name}' could not be initiated: Owner is not of type '{typeof(T).Name}'.");
            }
        }

        // Abstract Methods
        protected abstract void OnInitiate(object author);
    }



    //A longer-living command with start, update, end — like an ability/effect.
    public interface ILiveCommand
    {
        /// <summary> Called when the command starts. </summary>
        void StartCommand(float Duration = 0);
        /// <summary> Called to pause the command temporarily. </summary>
        void StopCommand();
        /// <summary> Elapsed time since command Live. </summary>
        float ElapsedTime { get; }
        // <summary> Whether the command has finished execution. </summary>
        bool IsFinished { get; }
    }
    ////////////// Note : You shoul implement Desable() for Stoped & Update() for UpdateCommand.
    public abstract class LifecycleCommand : Command, ILiveCommand
    {
        // Public Variable
        public bool IsValidated { get; private set; }
        public object MyAuthor { get; private set; }
        public MonoBehaviour MyOwner { get; private set; }
        public Action<LifecycleCommand> OnStarted;
        public Action<LifecycleCommand> OnFinished;

        // Private Variable
        private bool hasDuration = false;
        private float lifeTime;
        private bool isRunning = false;


        // Override Methods
        protected override void OnInitiate(object author, MonoBehaviour owner)
        {
            MyOwner = owner;
            MyAuthor = author;
            IsValidated = true;
        }

        // Methods
        protected void UpdateCommand()
        {
            if (!isRunning)
            {
                return;
            }

            if (IsValidated)
            {

                if (lifeTime > 0)
                {
                    lifeTime -= Time.deltaTime;
                }
                else
                {
                    if (hasDuration)
                    {
                        lifeTime = 0;
                        StopCommand();
                        return;
                    }
                }

                if (CanUpdate())
                {
                    OnUpdate();
                }
            }
            else
            {
                Debug.LogError("First You Sould Initiate Command with ICommandInitiator");
            }
        }
        protected virtual float PreProcessDuration(float StartDuration)
        {
            return StartDuration;
        }

        // Implement ILiveCommand Interface
        public float ElapsedTime => lifeTime;
        public bool IsFinished => !isRunning;
        public void StartCommand(float Duration)
        {
            if (!enabled)
                return;

            float NewDuration = PreProcessDuration(Duration);
            if (IsValidated)
            {
                hasDuration = NewDuration > 0;
                lifeTime = NewDuration > 0 ? NewDuration : 0;
                OnBegin();
                OnStarted?.Invoke(this);
                isRunning = true;
            }
            else
            {
                Debug.LogError("First You Sould Initiate Command with ICommandInitiator");
            }
        }
        public void StopCommand()
        {
            if (IsValidated)
            {
                isRunning = false;
                OnEnd();
                OnFinished?.Invoke(this);
            }
            else
            {
                Debug.LogError("First You Sould Initiate Command with ICommandInitiator");
            }
        }

        // Abstract Methods
        protected abstract void OnCreated();
        protected abstract void OnBegin();
        protected abstract bool CanUpdate();
        protected abstract void OnUpdate();
        protected abstract void OnEnd();
    }
    // A longer-living command that controled
    public interface IBehaviourCommand
    {
        /// <summary> Called to pause the command temporarily. </summary>
        void PauseCommand();
        /// <summary> Called to resume the command after a pause. </summary>
        void ResumeCommand();
        /// <summary> Resets command state (useful for pooling). </summary>
        void ResetCommand(float Duration);
        /// <summary> Whether the command is currently paused. </summary>
        bool IsPaused { get; }
    }
    ////////////// Note : You shoul implement Desable() for Pause & Update() for UpdateCommand.
    public abstract class ActionCommand : LifecycleCommand, IBehaviourCommand
    {
        // Public Variable
        public Action<LifecycleCommand> OnPaused;

        // Private Variable
        private bool islive = true;

        // Override Methods
        protected override bool CanUpdate()
        {
            return islive;
        }


        // Implement ILiveCommand Interface
        public bool IsPaused => !islive;
        public void PauseCommand()
        {
            islive = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        public void ResumeCommand()
        {
            if (!enabled)
                return;

            islive = true;
        }
        public void ResetCommand(float Duration)
        {
            if (!enabled)
                return;

            OnReset();
            StartCommand(Duration);
        }

        // Abstract Methods
        protected abstract void OnPause();
        protected abstract void OnReset();
    }



}