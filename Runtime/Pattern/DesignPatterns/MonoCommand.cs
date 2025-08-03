using UnityEngine;

namespace RealMethod
{
    // Base Command
    public abstract class MonoCommand : MonoBehaviour, ICommand
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
    public class CPrefab : PrefabCore<MonoCommand>
    {

    }

}