
namespace RealMethod
{
    public interface IFactoryEditorAccess
    {

    }
    public interface IFactoryAction
    {
        void Initiate();
        void Execute();
    }

    public struct FactoryProduct : IFactoryAction
    {
        public void Initiate()
        {
            throw new System.NotImplementedException();
        }
        public void Execute()
        {
            throw new System.NotImplementedException();
        }
    }
    
    public abstract class FactoryAsset : DataAsset
    {

    }
}
