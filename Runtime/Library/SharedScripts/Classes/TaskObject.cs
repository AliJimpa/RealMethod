using UnityEngine;

namespace RealMethod
{
    // Lifetime Command
    public abstract class TaskEffect : ITaskLife
    {
        // Public Variable
        public System.Action<TaskEffect> OnStarted;
        public System.Action<TaskEffect> OnFinished;

        // Protected Variable
        protected Object MyAuthor { get; private set; }
        protected Object MyOwner { get; private set; }
        public bool IsValidated { get; private set; }

        // Private Variable
        private float lifetime = -1;
        private float residuary = -1;
        private bool islive = false;

        // Override Methods
        public TaskEffect(Object author, Object owner)
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
                ((ITaskLife)this).StopTask();
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
        void ITaskLife.StartTask(float Duration)
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
        void ITaskLife.UpdateTask()
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
        void ITaskLife.StopTask()
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
        void ITaskLife.ClearTask()
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
    public abstract class TaskAction : TaskEffect, ITaskController
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
        void ITaskController.PauseTask()
        {
            isRunning = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        void ITaskController.ResumeTask()
        {
            isRunning = true;
            OnResume();
            OnResumed?.Invoke(this);
        }
        void ITaskController.ResetTask()
        {
            ResetValues();
            isRunning = true;
            OnReset();
        }
        void ITaskController.RestartTask(float Duration)
        {
            ResetValues();
            isRunning = true;
            OnReset();
            ((ITaskLife)this).StartTask(Duration);
        }

        // Abstract Methods
        protected abstract void OnPause();
        protected abstract void OnResume();
        protected abstract void OnReset();
    }


}