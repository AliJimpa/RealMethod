using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/MusicManager")]
    public sealed class MusicManager : CompositManager<DefaulMusicState, DefaulMusicLayer>
    {
        [Header("Music")]
        [SerializeField]
        private AudioClip[] CreateLayers;
        [SerializeField]
        private bool AddServiceOnInitiate = false;

        protected override void OnInitiate(bool AlwaysLoaded)
        {
            foreach (var clip in CreateLayers)
            {
                CreateLayer(clip);
            }
            if (AddServiceOnInitiate)
                Game.AddService<DefaulMusicState>(this);
        }
        protected override void MusicStateAssigned()
        {
        }

    }

    public enum DefaulMusicLayer
    {
        Default,
        Menu,
        StartGame,
        Level,
        Cutscene,
        Victory,
        Defeat,
        GameOver,
        Tutorial,
        Loading,
        Exploration,
        Combat,
        Dialogue,
        Quest,
        Fight,
        BossFight,
        Puzzle,
        Hint,
        Mission,
        Story,
        Chase,
        Damaged,
        Racing,
        Pause,
        Inventory,
        EndGame,
    }

    public sealed class DefaulMusicState : StateService<DefaulMusicLayer>
    {
        public DefaulMusicState() : base(DefaulMusicLayer.Default) // Replace default(StateList) with an actual enum value if needed
        {

        }

        // Service Methods
        protected override void OnStart(object Author)
        {
        }
        protected override void OnEnd(object Author)
        {

        }

        // StateService Methods
        protected override DefaulMusicLayer DefaultState()
        {
            return DefaulMusicLayer.Default;
        }
        public override bool CanSwitch(DefaulMusicLayer A, DefaulMusicLayer B)
        {
            if (A == B)
            {
                return false;
            }
            return true;
        }
        protected override bool CanResetforNewWorld(World NewWorld)
        {
            return false;
        }

    }




}