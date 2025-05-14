using UnityEngine;

namespace RealMethod
{
    public enum PCGCommandResult
    {
        Success = 0,
        Failed = 1,
        Break = 2
    }

    public abstract class PCGCommand
    {
        private PCGGenerationAsset Owner;
        protected void Print(string message, bool iserror)
        {
            if (iserror)
            {
                Debug.LogError($"{Owner.name}>{this.GetType().Name}: {message}");
            }
            else
            {
                Debug.LogWarning($"{Owner.name}>{this.GetType().Name}: {message}");
            }
        }
        public void Initialize(PCGGenerationAsset owner)
        {
            Owner = owner;
        }

        public abstract void Initialized();
        public abstract PCGCommandResult Process(ref PCGData Context, string StringParam, float FloatParam, Vector3 VectorParam);
    }


    public class LogCommand : PCGCommand
    {
        public override void Initialized()
        {
        }

        public override PCGCommandResult Process(ref PCGData Context, string Labelparam, float FloatParam, Vector3 VectorParam)
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

        public override PCGCommandResult Process(ref PCGData Context, string Labelparam, float FloatParam, Vector3 VectorParam)
        {
            Context.Position = Vector3.one * FloatParam * Context.DataIndex;
            return 0;
        }
    }

}