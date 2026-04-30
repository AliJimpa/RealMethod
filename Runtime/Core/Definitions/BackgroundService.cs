namespace RealMethod
{
    public interface IAnalyticsService
    {
        void Track(string eventName, params object[] arguments);
        void LevelStart(int levelIndex);
        void LevelComplete(int levelIndex);
        void Error(string message, string stacktrace = "");
    }
    public interface ICloudService
    {
        void Save(object data);
        void Load(System.Action<object> onLoaded);
        void Sync();
    }
    public interface IAuthService
    {
        void LoginGuest();
        void LoginGoogle();
        void LoginApple();
        void Logout();
        string GetUserId();
    }
    public interface ILeaderboardService
    {
        void Submit(int score);
        void GetTop(int count, System.Action<object> onResult);
        void GetAroundPlayer(System.Action<object> onResult);
    }
    public interface IRemoteConfigService
    {
        void Refresh(System.Action onComplete = null);
        T Get<T>(string key, T defaultValue = default);
    }
    public interface ILogService
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
    }
    public interface IStoreService
    {
        void Purchase(string productId);
        void RestorePurchases();
    }
    public interface INotificationService
    {
        void Schedule(string message, int delaySeconds);
        void CancelAll();
    }
    public interface ISettingsService
    {
        float Volume { get; set; }
        string Language { get; set; }
        int GraphicsQuality { get; set; }

        void Load();
        void Save();
    }
    public interface IDeviceService
    {
        string UniqueID { get; }
        string Platform { get; }
        int Memory { get; }
    }
    public interface IDebugService
    {
        void ShowFPS(bool active);
        void ShowToast(string message);
        void SetGodMode(bool active);
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