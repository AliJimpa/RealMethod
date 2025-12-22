using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "Blackboard", menuName = "RealMethod/Misc/Blackboard", order = 1)]
    public sealed class BlackboardAsset : SharedDataAsset
    {
        [Header("Blackboard")]
        [SerializeField]
        private bool ResetOnPlay = false;


#if UNITY_EDITOR
        public override void OnEditorPlay()
        {
            if (ResetOnPlay)
                base.OnEditorPlay();
        }
#endif
    }
}