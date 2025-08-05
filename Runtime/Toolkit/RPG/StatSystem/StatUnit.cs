using UnityEngine;

namespace RealMethod
{

    [CreateAssetMenu(fileName = "StatUnit", menuName = "RealMethod/RPG/StatUnit", order = 0)]
    public class StatUnit : StatStorage
    {
        protected override bool CheckModifier(IStatModifier mod)
        {
            return true;
        }
        protected override void OnValueChanged()
        {

        }


#if UNITY_EDITOR
        public override void OnEditorPlay()
        {

        }
#endif
    }




}