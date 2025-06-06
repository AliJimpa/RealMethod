using System;
using UnityEngine;

namespace RealMethod
{
    // A base one-shot command
    public interface ICommandInitiator
    {
        void Initiate(object author, MonoBehaviour owner);
        T CastCommand<T>() where T : MonoBehaviour;
        Command GetClass();
    }
    public abstract class Command : MonoBehaviour, ICommandInitiator
    {
        // Implement ICommandInitiator Interface
        void ICommandInitiator.Initiate(object author, MonoBehaviour owner)
        {
            OnInitiate(author, owner);
        }
        T ICommandInitiator.CastCommand<T>() where T : class
        {
            return this as T;
        }
        Command ICommandInitiator.GetClass()
        {
            return this;
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
            if (CanExecute(Executer))
            {
                Execute(Executer);
            }
        }

        // Abstract Methods
        protected abstract void Execute(object Owner);
        protected abstract bool CanExecute(object Owner);
    }
    // A command targeted at a specific type (e.g., a component).
    public abstract class TargetedCommand<T> : ExecutCommand where T : MonoBehaviour
    {
        public T MyOwner { get; private set; }

        // Override Methods
        protected sealed override void OnInitiate(object author, MonoBehaviour owner)
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
        /// <summary> Called every update or tick cycle. </summary>
        void UpdateCommand();
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
        protected sealed override void OnInitiate(object author, MonoBehaviour owner)
        {
            MyOwner = owner;
            MyAuthor = author;
            IsValidated = true;
            OnInitiate();
        }

        // Methods
        protected virtual float PreProcessDuration(float StartDuration)
        {
            return StartDuration;
        }

        // Implement ILiveCommand Interface
        public float ElapsedTime => lifeTime;
        public bool IsFinished => !isRunning;
        void ILiveCommand.StartCommand(float Duration)
        {
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
        void ILiveCommand.UpdateCommand()
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
                        ((ILiveCommand)this).StopCommand();
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
        void ILiveCommand.StopCommand()
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
        protected abstract void OnInitiate();
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
        protected sealed override bool CanUpdate()
        {
            return islive;
        }


        // Implement ILiveCommand Interface
        public bool IsPaused => !islive;
        void IBehaviourCommand.PauseCommand()
        {
            islive = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        void IBehaviourCommand.ResumeCommand()
        {
            islive = true;
        }
        void IBehaviourCommand.ResetCommand(float Duration)
        {
            OnReset();
            ((ILiveCommand)this).StartCommand(Duration);
        }

        // Abstract Methods
        protected abstract void OnPause();
        protected abstract void OnReset();
    }



}