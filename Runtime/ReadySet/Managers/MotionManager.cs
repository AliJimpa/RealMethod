
using UnityEngine;

namespace RealMethod
{
    public interface IMotion : IBehaviourAction , ITick
    {

    }

    public sealed class MotionManager : TickManager<MotionAsset>
    {
        [Header("Motion")]
        [SerializeField]
        private MotionAsset[] DefaultMotions;

        // TickManager Methods
        protected override void InitiateManager(bool alwaysLoaded)
        {
            if (DefaultMotions != null)
            {
                for (int i = 0; i < DefaultMotions.Length; i++)
                {
                    Create(DefaultMotions[i], this);
                }
            }
        }
        protected override void InitiateService(Service service)
        {
        }
        protected override bool CheckUnit(MotionAsset unit)
        {
            return true;
        }

        public IMotion Create(MotionAsset motion, Object author)
        {
            var TargetMotion = Instantiate(motion);
            IMotion provider = TargetMotion;
            provider.Start();
            units.Add(TargetMotion);
            return provider;
        }
        public bool Delete()
        {
            return true;
        }
    }

    public abstract class MotionAsset : DataAsset, IMotion
    {
        // Implement ITick Interface
        void ITick.Tick(float deltaTime)
        {
            Debug.Log(name);
        }
        // Implement IBehaviour Interface
        bool IBehaviour.IsStarted => throw new System.NotImplementedException();
        void IBehaviour.Start()
        {
            Debug.Log("Hello");
        }
        void IBehaviour.Stop()
        {
            throw new System.NotImplementedException();
        }
        void IBehaviour.Clear()
        {
            throw new System.NotImplementedException();
        }
        // Implement IBehaviourCycle Interface
        bool IBehaviourCycle.IsFinished => throw new System.NotImplementedException();
        bool IBehaviourCycle.IsInfinit => throw new System.NotImplementedException();
        float IBehaviourCycle.RemainingTime => throw new System.NotImplementedException();
        float IBehaviourCycle.ElapsedTime => throw new System.NotImplementedException();
        float IBehaviourCycle.NormalizedTime => throw new System.NotImplementedException();
        void IBehaviourCycle.Start(float overrideTime)
        {
            throw new System.NotImplementedException();
        }
        // Implement IBehaviourAction Interface
        bool IBehaviourAction.IsPaused => throw new System.NotImplementedException();
        void IBehaviourAction.Pause()
        {
            throw new System.NotImplementedException();
        }
        void IBehaviourAction.Resume()
        {
            throw new System.NotImplementedException();
        }
        void IBehaviourAction.Reset()
        {
            throw new System.NotImplementedException();
        }
        void IBehaviourAction.Restart(float Duration)
        {
            throw new System.NotImplementedException();
        }

    }







}