using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/MusicManager")]
    public sealed class MusicManager : CompositManager<GlobalState>
    {
        [Header("MusicList")]
        [SerializeField]
        private Map<Name16, AudioClip> Clips = new Map<Name16, AudioClip>();

        // GameManager
        public override void InitiateManager(Scope owner)
        {
            base.InitiateManager(owner);
            foreach (var clip in Clips)
            {
                CreateLayer(clip.Key, clip.Value);
            }
        }

        // Unity Methods
        private void OnEnable()
        {
            Game.State.OnStateChanged += OnStateChange;
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
            Game.State.OnStateChanged -= OnStateChange;
        }

        // CompositManager Methods
        protected override bool CompairStates(GlobalState State_A, GlobalState State_B)
        {
            return State_A == State_B;
        }

        // Functions
        private void OnStateChange(GlobalState a, GlobalState b)
        {
            if (IsValidState(b))
            {
                PlayState(b);
            }
        }
    }



}