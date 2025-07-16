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
        private void CreateLayers_Editor()
        {
            if (Layers != null && Layers.Length > 0)
            {
                for (int i = 0; i < Layers.Length; i++)
                {
                    if (Layers[i].source == null)
                    {
                        GameObject layerobject = new GameObject($"Layer_{Layers[i].layer}");
                        Layers[i].SetSource(layerobject.AddComponent<AudioSource>());
                        layerobject.transform.SetParent(transform);
                    }
                }
            }
        }
        [ContextMenu("ClearLayer")]
        private void ClearLayers_Editor()
        {
            if (Layers != null && Layers.Length > 0)
            {
                foreach (var layer in Layers)
                {
                    if (layer.source.gameObject != null)
                        DestroyImmediate(layer.source.gameObject);
                }
            }
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
        protected override DefaulMusicLayer DefaultState()
        {
            return DefaulMusicLayer.Default;
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