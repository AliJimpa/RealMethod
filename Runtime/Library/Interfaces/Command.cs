using UnityEngine;

namespace RealMethod
{
    // A base command you can execute immediately.
    public interface ICommand
    {
        bool Initiate(Object author, Object owner);
        void ExecuteCommand(object Executer);
    }
}