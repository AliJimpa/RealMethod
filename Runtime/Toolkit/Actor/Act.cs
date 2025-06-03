namespace RealMethod
{
    public interface IActTrigger
    {
        void ActiveAct();
        void DeactiveAct();
        bool CanActiveAct(object Author);
        bool IsActivate { get; }
    }

    public abstract class Act : Command, IActTrigger
    {
        public bool IsActivate => throw new System.NotImplementedException();

        public void ActiveAct()
        {
            throw new System.NotImplementedException();
        }

        public bool CanActiveAct(object Author)
        {
            throw new System.NotImplementedException();
        }

        public void DeactiveAct()
        {
            throw new System.NotImplementedException();
        }
    }

}