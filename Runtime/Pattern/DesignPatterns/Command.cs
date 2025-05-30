using UnityEngine;

namespace RealMethod
{
    public interface ICommandExecuter
    {
        void ExecuteCommand(object Executer);
    }

    public abstract class Command : MonoBehaviour, ICommandExecuter
    {
        // Implement ICommandExecuter Interface
        void ICommandExecuter.ExecuteCommand(object Executer)
        {
            Execute(Executer);
        }

        // Abstract Methods
        public abstract bool Initiate(object Owner);
        protected abstract void Execute(object Author);
    }
    
}