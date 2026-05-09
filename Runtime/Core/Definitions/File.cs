namespace RealMethod
{
    // FILE
    public interface IFile : IIdentifier
    {
        string FileName { get; }
    }


    /// <summary>
    /// this is UniqueAsset that implement ISaveFile Interface with some Editor Function
    /// for testing save and load
    /// </summary>
    public abstract class FileAsset : UniqueAsset, IFile
    {
        // Implement IFile Interface
        string IFile.FileName => name;
    }

}