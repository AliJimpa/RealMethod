
using UnityEngine;

namespace RealMethod
{
    public struct FactoryProduct : IFactoryAction
    {
        // Implement ICommand Interface
        bool ICommand.Initiate(Object owner)
        {
            throw new System.NotImplementedException();
        }
        void ICommand.ExecuteCommand(object Executer)
        {
            throw new System.NotImplementedException();
        }
    }

    public abstract class FactoryAsset : DataAsset
    {

    }
}
