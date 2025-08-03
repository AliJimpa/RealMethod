using UnityEngine;

namespace RealMethod
{
    public enum PCGRequestResult
    {
        Success = 0,
        Failed = 1,
        Break = 2
    }

    public abstract class PCGRequest : ICommand
    {
        private PCGGenerationAsset Owner;

        // Implement ICommand Interface
        bool ICommand.Initiate(Object author, Object owner)
        {
            if (owner is PCGGenerationAsset result)
            {
                Owner = result;
                return true;
            }
            else
            {
                Debug.LogError("Owner is not a PCGGenerationAsset in PCGRequest.Initiate.");
                return false;
            }
        }
        void ICommand.ExecuteCommand(object Executer)
        {
            throw new System.NotImplementedException();
        }

        // Protected Methods
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

        // Abstract Methods
        protected abstract void Initialized();
        public abstract PCGRequestResult Process(ref PCGData Context, string StringParam, float FloatParam, Vector3 VectorParam);



    }


    public class LogRequest : PCGRequest
    {
        protected override void Initialized()
        {
        }
        public override PCGRequestResult Process(ref PCGData Context, string Labelparam, float FloatParam, Vector3 VectorParam)
        {
            Debug.Log(Context.CodeName);
            return 0;
        }
    }
    public class DisRequest : PCGRequest
    {
        protected override void Initialized()
        {
        }
        public override PCGRequestResult Process(ref PCGData Context, string Labelparam, float FloatParam, Vector3 VectorParam)
        {
            Context.Position = Vector3.one * FloatParam * Context.SelfIndex;
            return 0;
        }
    }


}