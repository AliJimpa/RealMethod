using System;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public abstract class Effect
    {
        private Transform MyParent = null;
        public Transform prent => MyParent;
        protected bool hasParent => MyParent != null;
        private Pose effectPose = new Pose();
        protected bool hasPose { get; private set; } = false;
        public Pose pose => effectPose;
        private bool isRunning = false;
        public bool IsPuse => !isRunning;
        private bool isStarted = true;
        public bool IsFinish => !isStarted;

        // Public Functions
        public void Play(Transform parent, Vector3 Offcet)
        {
            MyParent = parent;
            effectPose.position = Offcet;
            hasPose = true;
            Play();
        }
        public void Player(Transform parent)
        {
            MyParent = parent;
            Play();
        }
        public void Play(Pose newPose)
        {
            effectPose = newPose;
            hasPose = true;
            Play();
        }
        public void Play(Vector3 location, Quaternion rotation)
        {
            effectPose.position = location;
            effectPose.rotation = rotation;
            hasPose = true;
            Play();
        }
        public void Play(Vector3 location)
        {
            effectPose.position = location;
            hasPose = true;
            Play();
        }
        public void Puse()
        {
            isRunning = false;
            OnHold(true);
        }
        public void Stop()
        {
            EndEffect();
            isRunning = false;
            hasPose = false;
        }
        public void Restart()
        {
            if (isRunning)
            {
                EndEffect();
            }
            Play();
        }
        public void Reset()
        {
            if (isRunning)
            {
                EndEffect();
            }
            OnReset();
        }


        // Private Functions
        private void Play()
        {
            if (!isStarted)
            {
                StartEffect();
            }

            if (!isRunning)
            {
                OnHold(false);
            }

            isRunning = true;
        }
        private void StartEffect()
        {
            isStarted = true;
            OnProduce();
        }
        private void EndEffect()
        {
            isStarted = false;
            OnClear();
        }


        // Abstract Methods
        protected abstract void OnProduce();
        protected abstract void OnHold(bool enable);
        protected abstract void OnClear();
        protected abstract void OnReset();
    }

}