using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/MusicManager")]
    public sealed class MusicManager : CompositManager<DefaulMusicState, DefaulMusicLayer>
    {
        public override void ServiceAssigned()
        {

        }

#if UNITY_EDITOR
        [ContextMenu("CreateLayer")]
        private void CreateLayer()
        {
            CreateLayers_Editor();
        }
        [ContextMenu("ClearLayer")]
        private void ClearLayer()
        {
            ClearLayers_Editor();
        }
#endif

    }

    public enum DefaulMusicLayer
    {
        Default,
        Menu,
        StartGame,
        Quest,
        Exploration,
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
        EndGame,
    }

    public sealed class DefaulMusicState : StateService<DefaulMusicLayer>
    {
        public DefaulMusicState() : base(DefaulMusicLayer.Default) // Replace default(StateList) with an actual enum value if needed
        {

        }

        // Service Methods
        protected override void OnEnd(object Author)
        {

        }

        // StateService Methods
        protected override void OnStart(object Author, DefaulMusicLayer State)
        {

        }
        public override bool CanSwitch(DefaulMusicLayer A, DefaulMusicLayer B)
        {
            return true;
        }
        protected override bool CanResetforNewWorld(World NewWorld)
        {
            return false;
        }
    }




}