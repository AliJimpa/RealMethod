using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class ActionSlot : IAction, IActionTask, IHandleBehaviourAction
    {
        [Header("Task")]
        [SerializeField]
        private Name16 name;
        [SerializeField]
        private bool infinit = false;
        [SerializeField, ConditionalHide("infinit", true, true)]
        private float baseDuration = 5;
        public bool IsEnable { get; private set; }

        // Private Variable
        private bool islive = false;
        private bool isValidated;
        private float lifetime = -1;
        private float residuary = -1;
        private bool isRunning = true;


        // Actions Delegate
        public event Action<ActionSlot> OnStarted;
        public event Action<ActionSlot> OnPaused;
        public event Action<ActionSlot> OnResumed;
        public event Action<ActionSlot> OnFinished;


        // Implement IIdentifier Interface
        Name16 IIdentifier.NameID => GetType().ToString();
        // Implement ITask Interface
        void ITask.Active()
        {
            IsEnable = true;
            isValidated = true;
            OnEnable();
        }
        void ITask.Deactive()
        {
            IsEnable = false;
            isValidated = false;
            OnDisable();
        }
        // Implement ITask<IAction> Interface
        IAction ITask<IAction>.Controller => this;
        // Implement ITick Interface
        void ITick.Tick(float delta)
        {
            // Check Initiate
            if (!isValidated)
            {
                Debug.LogError("First You Sould Validate");
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
                if (!infinit)
                {
                    // Stop Command Teime over
                    residuary = 0;
                    Finish();
                    return;
                }
            }

            OnUpdate(delta);
        }
        // Implement IAction Interface
        void IAction.Starter()
        {
            throw new NotImplementedException();
        }
        // Implement IBehaviour Interface
        public bool IsStarted => islive;
        void IHandleBehaviour.Start()
        {
            lifetime = infinit ? 0 : baseDuration;
            if (isValidated)
            {
                if (!islive)
                {
                    residuary = lifetime;
                    OnBegin();
                    OnStarted?.Invoke(this);
                    islive = true;
                }
            }
            else
            {
                Debug.LogError("First You Sould Initiate Behaviour");
            }
        }
        void IHandleBehaviour.Stop()
        {
            if (isValidated)
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
        void IHandleBehaviour.Clear()
        {
            Finish();
            isValidated = false;
        }
        // Implement IBehaviourCycle Interface
        public bool IsInfinit => infinit;
        public float RemainingTime => residuary;
        public float ElapsedTime => lifetime - residuary;
        public float NormalizedTime => residuary / lifetime;
        public bool IsFinished => !islive;
        void IHandleBehaviourCycle.Start(float overrideTime)
        {
            lifetime = overrideTime;
            if (isValidated)
            {
                if (!islive)
                {
                    residuary = lifetime;
                    OnBegin();
                    OnStarted?.Invoke(this);
                    islive = true;
                }
            }
            else
            {
                Debug.LogError("First You Sould Initiate Behaviour");
            }
        }
        // Implement IBehaviourAction Interface
        public bool IsPaused => !isRunning;
        void IHandleBehaviourAction.Pause()
        {
            isRunning = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        void IHandleBehaviourAction.Resume()
        {
            isRunning = true;
            OnResume();
            OnResumed?.Invoke(this);
        }
        void IHandleBehaviourAction.Reset()
        {
            ResetTaskValues();
            isRunning = true;
            OnReset();
        }
        void IHandleBehaviourAction.Restart(float Duration)
        {
            ResetTaskValues();
            isRunning = true;
            OnReset();
            if (Duration > 0)
            {
                ((IHandleBehaviourCycle)this).Start(Duration);
            }
            else
            {
                ((IHandleBehaviour)this).Start();
            }
        }


        // Functions
        public void Finish()
        {
            if (islive)
                ((IHandleBehaviour)this).Stop();
        }

        // Methods
        private void ResetTaskValues()
        {
            islive = false;
            isRunning = true;
            lifetime = -1;
            residuary = -1;
        }
        protected virtual bool CanUpdate()
        {
            return isRunning;
        }
        protected virtual void Reset()
        {
            IsEnable = false;
            ResetTaskValues();
        }


        // Abstract Methods
        protected abstract void OnEnable();
        protected abstract void OnBegin();
        protected abstract void OnUpdate(float delta);
        protected abstract void OnPause();
        protected abstract void OnResume();
        protected abstract void OnReset();
        protected abstract void OnEnd();
        protected abstract void OnDisable();

    }

}