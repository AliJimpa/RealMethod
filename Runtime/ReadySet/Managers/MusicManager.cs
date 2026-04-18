using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/MusicManager")]
    public sealed class MusicManager : CompositManager<GlobalEnum>
    {
        [Header("MusicList")]
        [SerializeField]
        private Map<Name16, AudioClip> Clips = new Map<Name16, AudioClip>();

        // GameManager
        public override void InitiateManager(bool AlwaysLoaded)
        {
            base.InitiateManager(AlwaysLoaded);
            foreach (var clip in Clips)
            {
                CreateLayer(clip.Key, clip.Value);
            }
        }

        // Unity Methods
        private void OnEnable()
        {
            Game.OnStateChanged += OnStateChange;
        }
        protected override void Start()
        {
            if (PlayOnStart)
            {
                CurrentState = Game.State;
            }
            base.Start();
        }
        private void OnDisable()
        {
            Game.OnStateChanged -= OnStateChange;
        }

        // CompositManager Methods
        protected override bool CompairStates(GlobalEnum State_A, GlobalEnum State_B)
        {
            return State_A == State_B;
        }

        // Functions
        private void OnStateChange(int stateIndex)
        {
            if (IsValidState(stateIndex))
            {
                PlayState(stateIndex);
            }
        }
    }



}