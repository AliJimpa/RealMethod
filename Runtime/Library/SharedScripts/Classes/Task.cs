using UnityEngine;

namespace RealMethod
{
    // Lifetime Command
    public abstract class TaskObject : ITask
    {
        // Public Variable
        public System.Action<TaskObject> OnStarted;
        public System.Action<TaskObject> OnFinished;

        // Protected Variable
        protected Object MyAuthor { get; private set; }
        protected Object MyOwner { get; private set; }
        public bool IsValidated { get; private set; }

        // Private Variable
        private float lifetime = -1;
        private float residuary = -1;
        private bool islive = false;

        // Override Methods
        public TaskObject(Object author, Object owner)
        {
            MyAuthor = author;
            MyOwner = owner;
            IsValidated = true;
            OnInitiate();
        }

        // Functions
        public void Finish()
        {
            if (islive)
                ((ITask)this).StopTask();
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
        void ITask.StartTask(float Duration)
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
        void ITask.UpdateTask()
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
        void ITask.StopTask()
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
        void ITask.ClearTask()
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
    public abstract class TaskAction : TaskObject, ITaskAction
    {
        // Public Variable
        public System.Action<TaskAction> OnPaused;
        public System.Action<TaskAction> OnResumed;

        // Private Variable
        private bool isRunning = true;

        protected TaskAction(Object author, Object owner) : base(author, owner)
        {

        }

        // Override Methods
        protected override bool CanUpdate()
        {
            return isRunning;
        }

        // Implement ILiveCommand Interface
        public bool IsPaused => !isRunning;
        void ITaskAction.PauseTask()
        {
            isRunning = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        void ITaskAction.ResumeTask()
        {
            isRunning = true;
            OnResume();
            OnResumed?.Invoke(this);
        }
        void ITaskAction.ResetTask()
        {
            ResetValues();
            isRunning = true;
            OnReset();
        }
        void ITaskAction.RestartTask(float Duration)
        {
            ResetValues();
            isRunning = true;
            OnReset();
            ((ITask)this).StartTask(Duration);
        }

        // Abstract Methods
        protected abstract void OnPause();
        protected abstract void OnResume();
        protected abstract void OnReset();
    }


}