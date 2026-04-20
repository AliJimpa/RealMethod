namespace RealMethod
{
    public interface ISave : IIdentifier
    {
        void OnLoaded();
        void OnSaved();
    }

    public interface ISaveSystem
    {
        bool IsExist(IFile file);
        void Save(IFile file);
        void Load(IFile file);
        void Delete(IFile file);
    }
}