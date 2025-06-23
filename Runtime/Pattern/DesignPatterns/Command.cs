using UnityEngine;

namespace RealMethod
{
    // A base one-shot command
    public interface ICommandInitiator
    {
        bool Initiate(Object author, Object owner);
        T CastCommand<T>() where T : Object;
    }
    public abstract class Command : MonoBehaviour, ICommandInitiator
    {
        // Implement ICommandInitiator Interface
        bool ICommandInitiator.Initiate(Object author, Object owner)
        {
            return OnInitiate(author, owner);
        }
        T ICommandInitiator.CastCommand<T>() where T : class
        {
            return this as T;
        }

        // Abstract Methods
        protected abstract bool OnInitiate(Object author, Object owner);
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
        protected sealed override bool OnInitiate(Object author, Object owner)
        {
            if (owner is T MyOwner)
            {
                return OnInitiate(author);
            }
            else
            {
                Debug.LogError($"Command '{GetType().Name}' could not be initiated: Owner is not of type '{typeof(T).Name}'.");
                return false;
            }
        }

        // Abstract Methods
        protected abstract bool OnInitiate(Object author);
    }



    //A longer-living command with start, update, end — like an ability/effect.
    public interface ICommandLife
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
    public abstract class LifecycleCommand : Command, ICommandLife
    {
        // Public Variable
        public bool IsValidated { get; private set; }
        public object MyAuthor { get; private set; }
        public MonoBehaviour MyOwner { get; private set; }
        public System.Action<LifecycleCommand> OnStarted;
        public System.Action<LifecycleCommand> OnFinished;

        // Private Variable
        private bool hasDuration = false;
        private float lifeTime;
        private bool isRunning = false;


        // Override Methods
        protected sealed override bool OnInitiate(Object author, Object owner)
        {
            MyAuthor = author;

            if (owner is MonoBehaviour result)
            {
                MyOwner = result;
                IsValidated = true;
                OnInitiate();
                return true;
            }
            else
            {
                IsValidated = false;
                Debug.LogError($"Command '{GetType().Name}' could not be initiated: Owner is not a MonoBehaviour.");
                return false;
            }
        }

        // Methods
        protected virtual float PreProcessDuration(float StartDuration)
        {
            return StartDuration;
        }

        // Implement ILiveCommand Interface
        public float ElapsedTime => lifeTime;
        public bool IsFinished => !isRunning;
        void ICommandLife.StartCommand(float Duration)
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
        void ICommandLife.UpdateCommand()
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
                        ((ICommandLife)this).StopCommand();
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
        void ICommandLife.StopCommand()
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
    public interface ICommandBehaviour
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
    public abstract class ActionCommand : LifecycleCommand, ICommandBehaviour
    {
        // Public Variable
        public System.Action<LifecycleCommand> OnPaused;

        // Private Variable
        private bool islive = true;

        // Override Methods
        protected sealed override bool CanUpdate()
        {
            return islive;
        }


        // Implement ILiveCommand Interface
        public bool IsPaused => !islive;
        void ICommandBehaviour.PauseCommand()
        {
            islive = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        void ICommandBehaviour.ResumeCommand()
        {
            islive = true;
        }
        void ICommandBehaviour.ResetCommand(float Duration)
        {
            OnReset();
            ((ICommandLife)this).StartCommand(Duration);
        }

        // Abstract Methods
        protected abstract void OnPause();
        protected abstract void OnReset();
    }



}