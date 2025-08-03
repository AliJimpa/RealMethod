using UnityEngine;

namespace RealMethod
{
    // A base command you can execute immediately.
    public interface ICommand
    {
        bool Initiate(Object author, Object owner);
        void ExecuteCommand(object Executer);
    }

    
    // Base Command
    public abstract class Command : MonoBehaviour, ICommand
    {
        // Implement ICommand Interface
        bool ICommand.Initiate(Object author, Object owner)
        {
            return OnInitiate(author, owner);
        }
        void ICommand.ExecuteCommand(object Executer)
        {
            if (CanExecute(Executer))
            {
                Execute(Executer);
            }
        }

        // Abstract Methods
        protected abstract bool OnInitiate(Object author, Object owner);
        protected abstract void Execute(object Owner);
        protected abstract bool CanExecute(object Owner);
    }

    [System.Serializable]
    public class CPrefab : PrefabCore<Command>
    {

    }

}