using System.Linq;
using UnityEngine;

namespace RealMethod
{

    public delegate void TaskCallback(ITask provider);
    public interface ITask : IBehaviourAction, IIdentifier
    {
        // Only expose event binding — not invoking
        event TaskCallback OnTaskComplete;
        bool IsValidated { get; }
        void Initiate(TaskManager owner, Object author);
        Object GetClass();
    }

    public abstract class TaskManager : MonoBehaviour, IGameManager
    {
        [Header("Setting")]
        [SerializeField]
        private UpdateMethod updateMethod = UpdateMethod.Update;
        [SerializeField, Tooltip("after play game Didn't change this value ")]
        private bool RemoveAfterEnd = false;
        [SerializeField]
        private TaskAsset[] DefaultTasks;


        private Hictionary<ITask> Tasks;
        public int Count => Tasks.IsValid() ? Tasks.Count : 0;

        // Implement IGameManager Interface
        MonoBehaviour IGameManager.GetManagerClass()
        {
            return this;
        }
        void IGameManager.InitiateManager(bool AlwaysLoaded)
        {
            if (DefaultTasks != null)
            {
                Tasks = new Hictionary<ITask>(DefaultTasks.Length + 5);
                foreach (var task in DefaultTasks)
                {
                    if (task is ITask provider)
                    {
                        provider.Initiate(this, this);
                        provider.Start(-1);
                        if (RemoveAfterEnd)
                            provider.OnTaskComplete += AnyTaskFinished;
                        Tasks.Add(provider.NameID, provider);
                    }
                    else
                    {
                        Debug.LogWarning($"ITask not Implemented in this Task {task}");
                    }
                }
            }
            else
            {
                Tasks = new Hictionary<ITask>(5);
            }

            InitiateManager(AlwaysLoaded);
        }
        void IGameManager.InitiateService(Service service)
        {
            InitiateService(service);
        }

        // Unity Method
        private void LateUpdate()
        {
            if (updateMethod == UpdateMethod.LateUpdate)
            {
                UpdateAbility();
            }
        }
        private void Update()
        {
            if (updateMethod == UpdateMethod.Update)
            {
                UpdateAbility();
            }
        }
        private void FixedUpdate()
        {
            if (updateMethod == UpdateMethod.FixedUpdate)
            {
                UpdateAbility();
            }
        }

        // Public Functions
        public bool IsValid(string taskName)
        {
            return Tasks.ContainsKey(taskName);
        }
        public bool TryFindTask(string taskName, out ITask task)
        {
            if (Tasks.ContainsKey(taskName))
            {
                task = Tasks[taskName];
                return true;
            }
            else
            {
                task = null;
                return false;
            }
        }
        public void Add(ITask task, float duration = -1)
        {
            task.Initiate(this, this);
            task.Start(-1);
            if (RemoveAfterEnd)
                task.OnTaskComplete += AnyTaskFinished;
            Tasks.Add(task.NameID, task);
        }
        public bool Remove(string taskName)
        {
            ITask provider;
            if (TryFindTask(taskName, out provider))
            {
                if (RemoveAfterEnd)
                    provider.OnTaskComplete -= AnyTaskFinished;
                Tasks.Remove(taskName);
                return true;
            }
            else
            {
                return false;
            }
        }
        public ITask[] GetAllTasks()
        {
            return Tasks.GetValues().ToArray();
        }

        // Private Functions
        private void UpdateAbility()
        {
            foreach (var task in Tasks.GetValues())
            {
                task.Update();
            }
        }
        private void AnyTaskFinished(ITask provider)
        {
            if (RemoveAfterEnd)
            {
                if (!Remove(provider.NameID))
                {
                    Debug.LogWarning($"Can't find any task with {provider.NameID} for removing");
                }
            }
        }


        // Abstract Method
        protected abstract void InitiateManager(bool AlwaysLoaded);
        protected abstract void InitiateService(Service service);

    }

    public abstract class TaskAsset : DataAsset, ITask
    {
        [Header("Task")]
        [SerializeField]
        private string taskName;
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
        protected TaskManager Manager { get; private set; }


        // Private Variable
        private event TaskCallback onTaskComplete;
        private bool isValidated;
        private float lifetime = -1;
        private float residuary = -1;
        private bool islive = false;
        private bool isRunning = true;



        // Functions
        public void Finish()
        {
            if (islive)
                ((IBehaviourMethod)this).Stop();
        }
        private void ResetTaskValues()
        {
            islive = false;
            lifetime = -1;
            residuary = -1;
        }
        protected virtual float PreProcessDuration(float OverrideDuration)
        {
            if (OverrideDuration < 0)
            {
                return infinit ? 0 : baseDuration;
            }
            return OverrideDuration;
        }
        protected virtual bool CanUpdate()
        {
            return isRunning;
        }


        // Implement ITask Interface
        public string NameID => taskName;
        public bool IsValidated => isValidated;
        event TaskCallback ITask.OnTaskComplete
        {
            add { onTaskComplete += value; }
            remove { onTaskComplete -= value; }
        }
        void ITask.Initiate(TaskManager owner, Object author)
        {
            Author = author;
            Manager = owner;
            isValidated = true;
            OnInitiate();
        }
        Object ITask.GetClass()
        {
            return this;
        }
        // Implement IBehaviourMethod Interface
        public bool IsInfinit => lifetime == 0;
        public float RemainingTime => residuary;
        public float ElapsedTime => lifetime - residuary;
        public float NormalizedTime => residuary / lifetime;
        public bool IsFinished => !islive;
        void IBehaviourMethod.Start(float Duration)
        {
            if (isValidated)
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
        void IBehaviourMethod.Update()
        {
            // Check Initiate
            if (!isValidated)
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
        void IBehaviourMethod.Stop()
        {
            if (isValidated)
            {
                if (islive)
                {
                    islive = false;
                    OnEnd();
                    OnFinished?.Invoke(this);
                    onTaskComplete?.Invoke(this);
                }
            }
            else
            {
                Debug.LogError("First You Sould Initiate Command with ICommandInitiator");
            }
        }
        void IBehaviourMethod.Clear()
        {
            Finish();
            isValidated = false;
            Author = null;
            Manager = null;
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
            ((IBehaviourMethod)this).Start(Duration);
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
        public override void OnEditorPlay()
        {
            ResetTaskValues();
        }
#endif


    }


}