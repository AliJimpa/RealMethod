using UnityEngine;

namespace RealMethod
{

    [System.Serializable]
    public class CPrefab : PrefabCore<Command>
    {

    }
    // Base Command
    public abstract class Command : MonoBehaviour, ICommand
    {
        // Implement ICommand Interface
        bool ICommand.Initiate(Object owner)
        {
            return OnInitiate(owner);
        }
        void ICommand.ExecuteCommand(object Executer)
        {
            if (CanExecute(Executer))
            {
                Execute(Executer);
            }
        }

        // Abstract Methods
        protected abstract bool OnInitiate(Object owner);
        protected abstract void Execute(object Executer);
        protected abstract bool CanExecute(object Executer);
    }


}