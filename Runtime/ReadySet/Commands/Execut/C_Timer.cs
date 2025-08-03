using UnityEngine;
using UnityEngine.Events;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Command/Timer")]
    public sealed class C_Timer : MonoCommand
    {
        private enum UpdateMethod
        {
            FixedUpdate,
            Update,
            LateUpdate,
        }
        private enum TimerBehavior
        {
            Nothing,
            SendMessage,
            Action,
            Both,
        }


        [Header("Setting")]
        [SerializeField]
        private float Duration;
        public float duration => Duration;
        [SerializeField]
        private UpdateMethod Method = UpdateMethod.Update;
        [SerializeField]
        private TimerBehavior Behavior = TimerBehavior.Nothing;
        [Header("TimeLine")]
        [SerializeField, ShowOnly]
        private float SelectedTime;
        [SerializeField, Range(0, 1)]
        private float Timeline = 0;
        [SerializeField]
        private float[] notify;
        public int notifyCount => notify.Length;
        public int currentNotify { get; private set; } = -1;
        [Header("Events")]
        [SerializeField]
        private UnityEvent<int> Notify;
        public System.Action<C_Timer> OnNotify;


        private float Timer = 0;
        public float RemainingTime => Duration - Timer;
        public float ElapsedTime => Timer;
        public float NormalizedTime => Timer / Duration;
        private bool isRunning = true;
        public bool isPause => !isRunning;
        private bool[] NotifyStatus = null;




        // ExecutCommand Methods
        protected override bool OnInitiate(Object author, Object owner)
        {
            NotifyStatus = new bool[notify.Length];
            return true;
        }
        protected override bool CanExecute(object Owner)
        {
            return enabled;
        }
        protected override void Execute(object Owner)
        {
            StartTimer();
        }


        // Unity Method
        private void FixedUpdate()
        {
            if (isRunning && Method == UpdateMethod.FixedUpdate)
            {
                UpdateTimer();
            }
        }
        private void Update()
        {
            if (isRunning && Method == UpdateMethod.Update)
            {
                UpdateTimer();
            }
        }
        private void LateUpdate()
        {
            if (isRunning && Method == UpdateMethod.LateUpdate)
            {
                UpdateTimer();
            }
        }
        private void OnValidate()
        {
            SelectedTime = Duration * Timeline;
        }


        // Public Functions
        public void StartTimer()
        {
            Timer = 0;
            isRunning = true;
        }
        public void PlayTimer()
        {
            isRunning = true;
        }
        public void PauseTimer()
        {
            isRunning = false;
        }
        public void StopTimer()
        {
            isRunning = false;
            Timer = Duration;
            for (int i = 0; i < NotifyStatus.Length; i++)
            {
                NotifyStatus[i] = false;
            }
        }


        // Private Functions
        private void UpdateTimer()
        {
            Timer += Time.deltaTime;
            CheckNotify(Timer);
            if (Timer >= Duration)
            {
                StopTimer();
            }
#if UNITY_EDITOR
            Timeline = NormalizedTime;
#endif
        }
        private void CheckNotify(float currentTime)
        {
            for (int i = 0; i < notify.Length; i++)
            {
                if (currentTime >= notify[i])
                {
                    if (!NotifyStatus[i])
                    {
                        NotifyStatus[i] = true;
                        currentNotify = i;
                        InvokNotifyMessage();
                    }
                }
            }
        }
        private void InvokNotifyMessage()
        {
            switch (Behavior)
            {
                case TimerBehavior.SendMessage:
                    SendMessage("OnNotify", this, SendMessageOptions.RequireReceiver);
                    break;
                case TimerBehavior.Action:
                    OnNotify?.Invoke(this);
                    break;
                case TimerBehavior.Both:
                    SendMessage("OnNotify", this, SendMessageOptions.RequireReceiver);
                    break;
            }
            Notify?.Invoke(currentNotify);
        }



    }
}