using UnityEngine;

namespace RealMethod
{
    public delegate void TaskCallback(bool enable);
    public interface ITask : ITick
    {
        void Enable(Object author);
        void Disable(Object author);
    }

    [AddComponentMenu("RealMethod/Manager/TaskManager")]
    public sealed class TaskManager : TickManager<ITask>
    {
        [Header("Task")]
        [SerializeField]
        private TaskAsset[] DefaultTasks;

        // TickManager Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
            if (Game.TryFindService(out Spawn SpawnServ))
            {
                SpawnServ.BringManager(this);
            }

            if (DefaultTasks != null)
            {
                for (int i = 0; i < DefaultTasks.Length; i++)
                {
                    Add(DefaultTasks[i], this);
                }
            }
        }
        protected override void InitiateService(Service service)
        {
            if (service is Spawn spawnservice)
            {
                spawnservice.BringManager(this);
            }
        }
        protected override bool CheckUnit(ITask unit)
        {
            return true;
        }


        // Public Functions
        public void Add(ITask task, Object author)
        {
            task.Enable(author);
            units.Add(task);
        }
        public bool Remove(ITask task, Object author)
        {
            if (units.Contains(task))
            {
                task.Disable(author);
                units.Remove(task);
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public abstract class TaskAsset : DataAsset, ITask
    {
        private event TaskCallback onTaskStatus;
        public bool IsEnable { get; private set; }

        // Implement ITask Interface
        void ITask.Enable(Object author)
        {
            IsEnable = true;
            OnTaskEnable(author);
            onTaskStatus?.Invoke(true);
        }
        void ITick.Tick(float delta)
        {
            OnTaskUpdate(delta);
        }
        void ITask.Disable(Object author)
        {
            IsEnable = false;
            OnTaskDisable(author);
            onTaskStatus?.Invoke(false);
        }

        // Abstract Methods
        protected abstract void OnTaskEnable(Object author);
        protected abstract void OnTaskUpdate(float delta);
        protected abstract void OnTaskDisable(Object author);

#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            IsEnable = false;
        }
#endif
    }
    public abstract class TaskBehaviour : TaskAsset, IBehaviourAction
    {
        [Header("Task")]
        [SerializeField]
        private bool infinit = false;
        [SerializeField, ConditionalHide("infinit", true, true)]
        private float baseDuration = 5;

        // Public Variable
        public System.Action<TaskAsset> OnStarted;
        public System.Action<TaskAsset> OnPaused;
        public System.Action<TaskAsset> OnResumed;
        public System.Action<TaskAsset> OnFinished;

        // Protected Variable
        protected Object Author { get; private set; }

        // Private Variable
        private ITick Loop;
        private bool isValidated;
        private float lifetime = -1;
        private float residuary = -1;
        private bool islive = false;
        private bool isRunning = true;


        // TaskAsset Methods
        protected sealed override void OnTaskEnable(Object author)
        {
            Author = author;
            isValidated = true;
            Loop = this;
            OnInitiate();
            ((IBehaviour)this).Start();
        }
        protected sealed override void OnTaskUpdate(float delta)
        {
            Loop.Tick(delta);
        }
        protected sealed override void OnTaskDisable(Object author)
        {
            isValidated = false;
            Author = null;
        }


        // Implement IBehaviour Interface
        public bool IsStarted => islive;
        void IBehaviour.Start()
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
        void IBehaviour.Stop()
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
        void IBehaviour.Clear()
        {
            Finish();
            isValidated = false;
            Author = null;
        }
        // Implement IBehaviourCycle Interface
        public bool IsInfinit => infinit;
        public float RemainingTime => residuary;
        public float ElapsedTime => lifetime - residuary;
        public float NormalizedTime => residuary / lifetime;
        public bool IsFinished => !islive;
        void IBehaviourCycle.Start(float overrideTime)
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
        void ITick.Tick(float deltaTime)
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

            OnUpdate();
        }
        // Implement IBehaviourAction Interface
        public bool IsPaused => !isRunning;
        void IBehaviourAction.Pause()
        {
            isRunning = false;
            OnPause();
            OnPaused?.Invoke(this);
        }
        void IBehaviourAction.Resume()
        {
            isRunning = true;
            OnResume();
            OnResumed?.Invoke(this);
        }
        void IBehaviourAction.Reset()
        {
            ResetTaskValues();
            isRunning = true;
            OnReset();
        }
        void IBehaviourAction.Restart(float Duration)
        {
            ResetTaskValues();
            isRunning = true;
            OnReset();
            if (Duration > 0)
            {
                ((IBehaviourCycle)this).Start(Duration);
            }
            else
            {
                ((IBehaviour)this).Start();
            }
        }


        // Functions
        public void Finish()
        {
            if (islive)
                ((IBehaviour)this).Stop();
        }
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


        // Abstract Methods
        protected abstract void OnInitiate();
        protected abstract void OnBegin();
        protected abstract void OnUpdate();
        protected abstract void OnPause();
        protected abstract void OnResume();
        protected abstract void OnReset();
        protected abstract void OnEnd();

#if UNITY_EDITOR
        public sealed override void OnEditorPlay()
        {
            base.OnEditorPlay();
            ResetTaskValues();
        }
#endif


    }

}