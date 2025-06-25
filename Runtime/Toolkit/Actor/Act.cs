namespace RealMethod
{
    public interface IActTrigger
    {
        void ActiveAct();
        void DeactiveAct();
        bool CanActiveAct(object Author);
        bool IsActivate { get; }
    }

    public abstract class Act : Command
    {
        
    }

}