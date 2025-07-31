namespace RealMethod
{
    public interface IInitializable
    {
        void Initialize();
    }
    public interface IInitializableWithArgument<TArgument>
    {
        void Initialize(TArgument argument);
    }
    public interface IInitializableWithTwoArgument<TArgumentA, TArgumentB>
    {
        void Initialize(TArgumentA argumentA, TArgumentB argumentB);
    }

}


