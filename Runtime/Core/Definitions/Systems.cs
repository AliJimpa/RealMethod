namespace RealMethod
{
    public interface IAnalyticsService : IService
    {
        void Track(string eventName, params object[] arguments);
        void LevelStart(int levelIndex);
        void LevelComplete(int levelIndex);
        void Error(string message, string stacktrace = "");
    }
    public interface ISaveService : IService
    {
        void Load();
        void Save();
    }
    public interface ICloudService : ISaveService
    {
        void Save(object data);
        void Load(System.Action<object> onLoaded);
        void Sync();
    }
    public interface IAuthService : IService
    {
        void LoginGuest();
        void LoginGoogle();
        void LoginApple();
        void Logout();
        string GetUserId();
    }
    public interface ILeaderboardService : IService
    {
        void Submit(int score);
        void GetTop(int count, System.Action<object> onResult);
        void GetAroundPlayer(System.Action<object> onResult);
    }
    public interface IRemoteConfigService : IService
    {
        void Refresh(System.Action onComplete = null);
        T Get<T>(string key, T defaultValue = default);
    }
    public interface ILogService : IService
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
    }
    public interface IStoreService : IService
    {
        void Purchase(string productId);
        void RestorePurchases();
    }
    public interface INotificationService : IService
    {
        void Schedule(string message, int delaySeconds);
        void CancelAll();
    }
    public interface ISettingsService : ISaveService
    {
        float Volume { get; set; }
        string Language { get; set; }
        int GraphicsQuality { get; set; }
    }
    public interface IDeviceService : IService
    {
        string UniqueID { get; }
        string Platform { get; }
        int Memory { get; }
    }
    public interface IDebugService2 : IService
    {
        void ShowFPS(bool active);
        void ShowToast(string message);
        void SetGodMode(bool active);
    }
    public interface IStorageService : ISaveService
    {
        /// <summary>
        /// Checks whether the specified file exists in the save storage.
        /// </summary>
        bool IsExist(IFile file);
        /// <summary>
        /// Saves the specified file to the save storage.
        /// </summary>
        void Save(IFile file);
        /// <summary>
        /// Loads the specified file from the save storage into memory.
        /// </summary>
        void Load(IFile file);
        /// <summary>
        /// Deletes the specified file from the save storage.
        /// </summary>
        void Delete(IFile file);
        /// <summary>
        /// use for adding file to filelist in savemsystem.
        /// if you want to use saveall or geting file with name
        /// </summary>
        /// <param name="file">target file you want adding</param>
        /// <returns>return true if can add</returns>
        bool AddFile(IFile file);
        /// <summary>
        /// use for removing file to filelist in savesystem
        /// </summary>
        /// <param name="file">target file you want removing</param>
        /// <returns>return true if can remove</returns>
        bool RemoveFile(IFile file);
        /// <summary>
        /// Determines whether the specified file already exists in the file list.
        /// Checks by object reference unless IFile implementations override equality.
        /// </summary>
        /// <param name="file">The file instance to check.</param>
        /// <returns>True if the file is found in the list; otherwise false.</returns>
        bool HasFile(IFile file);
        /// <summary>
        /// The file that created by savesystem for merging all file selected in sytem to one file
        /// </summary>
        IFile MainSaveFile { get; }
    }

    public interface ILogSystem
    {
        void Log(object message, UnityEngine.Object context);
        void LogWarning(object message, UnityEngine.Object context);
        void LogError(object message, UnityEngine.Object context);
        //void LogException(Exception exception, UnityEngine.Object context);
        void Assert(object message, UnityEngine.Object context);
    }


}