namespace RealMethod
{
    public abstract class BackgroundService : Service
    {
        public override void OnRegister(object Author)
        {
            throw new System.NotImplementedException();
        }
        public override void OnWorldChanging(World Previous, World New)
        {
            throw new System.NotImplementedException();
        }
        public override void OnUnregister(object Author)
        {
            throw new System.NotImplementedException();
        }
    }




    public class AnalyticsService : BackgroundService
    {
        public void Track(string eventName, params object[] arguments) { }
        public void LevelStart(int levelIndex) { }
        public void LevelComplete(int levelIndex) { }
        public void Error(string message, string stacktrace = "") { }
    }
    public class CloudService : BackgroundService
    {
        public void Save(object data) { }
        public void Load(System.Action<object> onLoaded) { }
        public void Sync() { }
    }
    public class AuthService : BackgroundService
    {
        public void LoginGuest() { }
        public void LoginGoogle() { }
        public void LoginApple() { }
        public void Logout() { }
        public string GetUserId() { return ""; }
    }
    public class RemoteConfigService : BackgroundService
    {
        public void Refresh(System.Action onComplete = null) { }
        public T Get<T>(string key, T defaultValue = default) { return defaultValue; }
    }
    // public class LogService : BackgroundService
    // {
    //     public void Info(string message) { }
    //     public void Warning(string message) { }
    //     public void Error(string message) { }
    // }
    public class StoreService : RemoteConfigService
    {
        public void Purchase(string productId) { }
        public void RestorePurchases() { }
    }
    public class SettingsService : StoreService
    {
        public float Volume { get; set; }
        public string Language { get; set; }
        public int GraphicsQuality { get; set; }

        public void Load() { }
        public void Save() { }
    }
    public class DeviceService : StoreService
    {
        public string UniqueID => "";
        public string Platform => "";
        public int Memory => 0;
    }
    // public class DebugService : DeviceService
    // {
    //     public void ShowFPS(bool active) { }
    //     public void ShowToast(string message) { }
    //     public void SetGodMode(bool active) { }
    // }
}