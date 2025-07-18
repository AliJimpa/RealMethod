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
    [System.Serializable]
    public class CPrefab : Prefab<Command>
    {

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
        /// <summary> Called to Clear initating command. </summary>
        void ClearCommand();
        /// <summary> Elapsed time since command Live. </summary>
        float RemainingTime { get; }
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
        private float lifetime = -1;
        private float residuary = -1;
        private bool islive = false;


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
        public void Finish()
        {
            if (islive)
                ((ICommandLife)this).StopCommand();
        }
        protected void ResetValues()
        {
            islive = false;
            lifetime = -1;
            residuary = -1;
        }
        protected virtual float PreProcessDuration(float StartDuration)
        {
            return StartDuration;
        }


        // Implement ILiveCommand Interface
        public bool IsInfinit => lifetime == 0;
        public float RemainingTime => residuary;
        public float ElapsedTime => lifetime - residuary;
        public float NormalizedTime => residuary / lifetime;
        public bool IsFinished => !islive;
        void ICommandLife.StartCommand(float Duration)
        {
            if (IsValidated)
            {
                if (!islive)
                {
                    lifetime = PreProcessDuration(Duration);
                    residuary = lifetime > 0 ? lifetime : 0;
                    OnBegin();
                    OnStarted?.Invoke(this);
                    islive = true;
                }
            }
            else
            {
                Debug.LogError("First You Sould Initiate Command with ICommandInitiator");
            }
        }
        void ICommandLife.UpdateCommand()
        {
            // Check Initiate
            if (!IsValidated)
            {
                Debug.LogError("First You Sould Initiate Command with ICommandInitiator");
                return;
            }

            // Check Started
            if (!islive)
            {
                return;
            }

            // Gate for Puse Updating Command
            if (!CanUpdate())
            {
                return;
            }

            // Handel Lifetime Command
            if (residuary > 0)
            {
                // Calculate Time
                residuary -= Time.deltaTime;
            }
            else
            {
                if (!IsInfinit)
                {
                    // Stop Command Teime over
                    residuary = 0;
                    Finish();
                    return;
                }
            }

            OnUpdate();
        }
        void ICommandLife.StopCommand()
        {
            if (IsValidated)
            {
                if (islive)
                {
                    islive = false;
                    OnEnd();
                    OnFinished?.Invoke(this);
                }
            }
            else
            {
                Debug.LogError("First You Sould Initiate Command with ICommandInitiator");
            }
        }
        void ICommandLife.ClearCommand()
        {
            Finish();
            IsValidated = false;
            MyAuthor = null;
            MyOwner = null;
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
        /// <summary> Called to reset the command propetries. </summary>
        void ResetCommand();
        /// <summary> Resets command state (useful for pooling). </summary>
        void RestartCommand(float Duration = 0);
        /// <summary> Whether the command is currently paused. </summary>
        bool IsPaused { get; }
    }
    ////////////// Note : You shoul implement Desable() for Pause & Update() for UpdateCommand.
    public abstract class ActionCommand : LifecycleCommand, ICommandBehaviour
    {
        // Public Variable
        public System.Action<LifecycleCommand> OnPaused;
        public System.Action<LifecycleCommand> OnResumed;

        // Private Variable
        private bool isRunning = true;

        // Override Methods
        protected override bool CanUpdate()
        {
            return isRunning;
        }

        // Implement ILiveCommand Interface
        public bool IsPaused => !isRunning;
        void ICommandBehaviour.PauseCommand()
        {
            isRunning = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        void ICommandBehaviour.ResumeCommand()
        {
            isRunning = true;
            OnResume();
            OnResumed?.Invoke(this);
        }
        void ICommandBehaviour.ResetCommand()
        {
            ResetValues();
            isRunning = true;
            OnReset();
        }
        void ICommandBehaviour.RestartCommand(float Duration)
        {
            ResetValues();
            isRunning = true;
            OnReset();
            ((ICommandLife)this).StartCommand(Duration);
        }

        // Abstract Methods
        protected abstract void OnPause();
        protected abstract void OnResume();
        protected abstract void OnReset();
    }






}