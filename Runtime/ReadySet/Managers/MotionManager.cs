using UnityEngine;

namespace RealMethod
{
    public interface IMotion : IHandleBehaviourAction
    {
    }
    public interface IMotionTask : ITask<IMotion>
    {

    }

    public abstract class MotionAsset : CloneAsset, IMotion, IMotionTask
    {
        [Header("Task")]
        [SerializeField]
        private bool infinit = false;
        [SerializeField, ConditionalHide("infinit", true, true)]
        private float baseDuration = 5;
        public bool IsEnable { get; private set; }


        // Private Variable
        private bool islive = false;
        private bool isRunning = true;
        private float residuary = -1;
        private float lifetime = -1;


        // Implement ITask Interface
        void ITask.Active()
        {
            IsEnable = true;
            OnEnable();
        }
        void ITask.Deactive()
        {
            Finish();
            IsEnable = false;
            OnDisable();
        }
        // Implement ITask<T> Interface
        IMotion ITask<IMotion>.Controller => this;
        // Implement ITick Interface
        void ITick.Tick(float deltaTime)
        {
            // Check Initiate
            if (!IsEnable)
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

            OnUpdate(deltaTime);
        }
        // Implement IHandleBehaviour Interface
        bool IHandleBehaviour.IsStarted => islive;
        void IHandleBehaviour.Start()
        {
            lifetime = infinit ? 0 : baseDuration;
            if (IsEnable)
            {
                if (!islive)
                {
                    residuary = lifetime;
                    OnBegin();
                    islive = true;
                }
            }
            else
            {
                Debug.LogError("First You Sould Active Task");
            }
        }
        void IHandleBehaviour.Stop()
        {
            if (IsEnable)
            {
                if (islive)
                {
                    islive = false;
                    OnEnd();
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
            IsEnable = false;
        }
        // Implement IHandleBehaviourCycle Interface
        bool IHandleBehaviourCycle.IsFinished => !islive;
        bool IHandleBehaviourCycle.IsInfinit => infinit;
        float IHandleBehaviourCycle.RemainingTime => residuary;
        float IHandleBehaviourCycle.ElapsedTime => lifetime - residuary;
        float IHandleBehaviourCycle.NormalizedTime => residuary / lifetime;
        void IHandleBehaviourCycle.Start(float overrideTime)
        {
            lifetime = overrideTime;
            if (IsEnable)
            {
                if (!islive)
                {
                    residuary = lifetime;
                    OnBegin();
                    islive = true;
                }
            }
            else
            {
                Debug.LogError("First You Sould Initiate Behaviour");
            }
        }
        // Implement IHandleBehaviourAction Interface
        bool IHandleBehaviourAction.IsPaused => !isRunning;
        void IHandleBehaviourAction.Pause()
        {
            isRunning = false;
            OnPause();
        }
        void IHandleBehaviourAction.Resume()
        {
            isRunning = true;
            OnResume();
        }
        void IHandleBehaviourAction.Reset()
        {
            ResetToDefault();
            isRunning = true;
            OnReset();
        }
        void IHandleBehaviourAction.Restart(float Duration)
        {
            ResetToDefault();
            isRunning = true;
            OnReset();
            if (Duration > 0)
            {
                ((IHandleBehaviourCycle)this).Start(Duration);
            }
            else
            {
                ((IHandleBehaviourCycle)this).Start();
            }
        }


        // Method
        protected void Finish()
        {
            if (islive)
                ((IHandleBehaviour)this).Stop();
        }
        protected void Reset()
        {
            IsEnable = false;
            ResetToDefault();
        }
        private void ResetToDefault()
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


        // Abstract Methods
        protected abstract void OnEnable();
        protected abstract void OnBegin();
        protected abstract void OnUpdate(float deltaTime);
        protected abstract void OnPause();
        protected abstract void OnResume();
        protected abstract void OnReset();
        protected abstract void OnEnd();
        protected abstract void OnDisable();
    }


    [AddComponentMenu("RealMethod/Manager/MotionManager")]
    public sealed class MotionManager : TaskManager<IMotionTask, IMotion>
    {
        [Header("Motion")]
        [SerializeField]
        private MotionAsset[] DefaultMotions;



        // TickManager Methods
        protected override void InitiateManager(Scope owner)
        {
            if (DefaultMotions != null)
            {
                for (int i = 0; i < DefaultMotions.Length; i++)
                {
                    Create(DefaultMotions[i]);
                }
            }
        }
        protected override bool TryCreateNewInstance<P>(object ClassType, out P result)
        {
            if (ClassType is PrimitiveAsset asset)
            {
                result = ScriptableObject.Instantiate(asset) as P;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }
        protected override void AutoStart(IMotion Handle)
        {
            Handle.Start();
        }


    }






}