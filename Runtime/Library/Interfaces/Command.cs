using UnityEngine;

namespace RealMethod
{
    // A base command you can execute immediately.
    public interface ICommand
    {
        bool Initiate(Object owner);
        void ExecuteCommand(object Executer);
    }

}