using UnityEngine;

namespace RealMethod
{
    public abstract class Effect
    {
        // Base Variable
        private bool isRunning = false;
        public bool IsPuse => !isRunning;
        private bool isStarted = false;
        public bool IsFinish => !isStarted;
        // Cache Variable
        private Transform cacheParent = null;
        private Pose cachPose = new Pose();
        private float cacheSize = 1;

        // Public Functions
        public void Play(Transform parent, Vector3 location, Quaternion rotation, float size)
        {
            cacheParent = parent;
            cachPose = new Pose(location, rotation);
            cacheSize = size;
            PlayEffect();
        }
        public void Play(Transform parent, Pose newPose)
        {
            cacheParent = parent;
            cachPose = newPose;
            cacheSize = 1;
            PlayEffect();
        }
        public void Play(Transform parent, Vector3 Offcet)
        {
            cacheParent = parent;
            cachPose = new Pose(Offcet, Quaternion.identity);
            cacheSize = 1;
            PlayEffect();
        }
        public void Play(Transform parent, float size)
        {
            cacheParent = parent;
            cachPose = new Pose();
            cacheSize = size;
            PlayEffect();
        }
        public void Play(Transform parent)
        {
            cacheParent = parent;
            cachPose = new Pose();
            cacheSize = 1;
            PlayEffect();
        }
        public void Play(Pose newPose)
        {
            cacheParent = null;
            cachPose = newPose;
            cacheSize = 1;
            PlayEffect();
        }
        public void Play(Vector3 location, Quaternion rotation, float size)
        {
            cacheParent = null;
            cachPose = new Pose(location, rotation);
            cacheSize = size;
            PlayEffect();
        }
        public void Play(Vector3 location, Quaternion rotation)
        {
            cacheParent = null;
            cachPose = new Pose(location, rotation);
            cacheSize = 1;
            PlayEffect();
        }
        public void Play(Vector3 location, float size)
        {
            cacheParent = null;
            cachPose = new Pose(location, Quaternion.identity);
            cacheSize = size;
            PlayEffect();
        }
        public void Play(Vector3 location)
        {
            cacheParent = null;
            cachPose = new Pose(location, Quaternion.identity);
            cacheSize = 1;
            PlayEffect();
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
        }
        public void Restart()
        {
            if (isRunning)
            {
                EndEffect();
            }
            PlayEffect();
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
        private void PlayEffect()
        {
            if (!isStarted)
            {
                StartEffect();
            }
            else
            {
                if (!isRunning)
                {
                    OnHold(false);
                }
            }

            isRunning = true;
        }
        private void StartEffect()
        {
            isStarted = true;
            OnProduce(cachPose, cacheParent, cacheSize);
        }
        private void EndEffect()
        {
            isStarted = false;
            OnClear();
        }


        // Abstract Methods
        protected abstract void OnProduce(Pose pose, Transform parent, float size);
        protected abstract void OnHold(bool enable);
        protected abstract void OnClear();
        protected abstract void OnReset();
    }

}