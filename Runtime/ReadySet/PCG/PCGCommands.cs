using UnityEngine;

namespace RealMethod
{
    public class LogCommand : PCGCommand
    {
        public override void Initialized()
        {
        }

        public override PCGCommandResult Process(ref PCGData Context, string Labelparam, float Radiusparam, Vector3 Locationparam)
        {
            Debug.Log(Context.CodeName);
            return 0;
        }
    }

    public class Dis : PCGCommand
    {
        public override void Initialized()
        {
        }

        public override PCGCommandResult Process(ref PCGData Context, string Labelparam, float Radiusparam, Vector3 Locationparam)
        {
            Context.Position = Vector3.one * 100 * Context.Index;
            return 0;
        }
    }


}