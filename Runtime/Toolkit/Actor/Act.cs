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
        private bool Activing = false;


        // Implement IActTrigger Interface
        void IActTrigger.ActiveAct()
        {
            Activing = true;
            OnActive();
        }
        void IActTrigger.DeactiveAct()
        {
            Activing = false;
            OnDeactive();
        }
        public bool CanActiveAct(object Author)
        {
            if (CanActive())
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool IsActivate => Activing;


        protected abstract void OnActive();
        protected abstract void OnDeactive();
        protected abstract bool CanActive();

    }

}