
using UnityEditor;

namespace RealMethod.Editor
{
    class PatternMenu
    {
        private const RealMethodLayer MenuLayer = RealMethodLayer.Pattern;


        ////////////// Scripts \\\\\\\\\\\\
        // Managers
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/AudioManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateAudioManager()
        {
            RM_Editor.CreateScriptTemplate("AudioManager", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/CompositManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateCompositManager()
        {
            RM_Editor.CreateScriptTemplate("CompositManager", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/DataManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateDataManager()
        {
            RM_Editor.CreateScriptTemplate("DataManager", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/GizmoManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateGizmoManager()
        {
            RM_Editor.CreateScriptTemplate("GizmoManager", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/UIManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateUIManager()
        {
            RM_Editor.CreateScriptTemplate("UIManager", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/MixerManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateMixerManager()
        {
            RM_Editor.CreateScriptTemplate("MixerManager", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/HapticManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateHapticManager()
        {
            RM_Editor.CreateScriptTemplate("HapticManager", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Manager/TickManager", false, RM_Editor.MenuOrder.Manager_Body)]
        public static void CreateTickManager()
        {
            RM_Editor.CreateScriptTemplate("TickManager", MenuLayer);
        }

        // Services
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Service/StateService", false, RM_Editor.MenuOrder.Service_Body)]
        public static void CreateStateService()
        {
            RM_Editor.CreateScriptTemplate("StateService", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Service/RuleService", false, RM_Editor.MenuOrder.Service_Body)]
        public static void CreateRuleService()
        {
            RM_Editor.CreateScriptTemplate("RuleService", MenuLayer);
        }

        // Assets
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Data/Table", false, RM_Editor.MenuOrder.DataAsset_Body)]
        public static void CreateTableAsset()
        {
            RM_Editor.CreateScriptTemplate("TableAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Data/Task", false, RM_Editor.MenuOrder.DataAsset_Body)]
        public static void CreateTaskAsset()
        {
            RM_Editor.CreateScriptTemplate("TaskAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Config/Item", false, RM_Editor.MenuOrder.ConfigAsset_Body)]
        public static void CreateItemConfig()
        {
            RM_Editor.CreateScriptTemplate("ItemConfig", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Config/Haptic", false, RM_Editor.MenuOrder.ConfigAsset_Body)]
        public static void CreateHapticConfig()
        {
            RM_Editor.CreateScriptTemplate("HapticConfig", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/File/EmptyFileAsset", false, RM_Editor.MenuOrder.UniqueAsset_Body)]
        public static void CreateFileAsset()
        {
            RM_Editor.CreateScriptTemplate("FileAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/File/Save", false, RM_Editor.MenuOrder.UniqueAsset_Body)]
        public static void CreateSaveFile()
        {
            RM_Editor.CreateScriptTemplate("SaveFile", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/File/GameSetting", false, RM_Editor.MenuOrder.UniqueAsset_Body)]
        public static void CreateGameSettingFile()
        {
            RM_Editor.CreateScriptTemplate("GameSettingFile", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Unique/SharedRootAsset", false, RM_Editor.MenuOrder.UniqueAsset_Body)]
        public static void CreateSharedRootAsset()
        {
            RM_Editor.CreateScriptTemplate("SharedRootAsset", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Asset/Unique/PoolAsset", false, RM_Editor.MenuOrder.UniqueAsset_Body)]
        public static void CreatePoolAsset()
        {
            RM_Editor.CreateScriptTemplate("PoolAsset", MenuLayer);
        }


        // UI
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Method/UI/Widget", false, RM_Editor.MenuOrder.Method)]
        public static void CreateWidget()
        {
            RM_Editor.CreateScriptTemplate("Widget", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Method/UI/WidgetToolkit", false, RM_Editor.MenuOrder.Method)]
        public static void CreateWidgetToolkit()
        {
            RM_Editor.CreateScriptTemplate("WidgetToolkit", MenuLayer);
        }

        // Command
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Method/Command", false, RM_Editor.MenuOrder.Method)]
        public static void CreateCommand()
        {
            RM_Editor.CreateScriptTemplate("Command", MenuLayer);
        }

        // Trigger
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Method/Trigger/Trigger3D", false, RM_Editor.MenuOrder.Method)]
        public static void CreateTrigger3D()
        {
            RM_Editor.CreateScriptTemplate("Trigger3D", MenuLayer);
        }
        [MenuItem(RM_Editor.ScriptMenuItemPath + "Method/Trigger/Trigger2D", false, RM_Editor.MenuOrder.Method)]
        public static void CreateTrigger2D()
        {
            RM_Editor.CreateScriptTemplate("Trigger2D", MenuLayer);
        }




    }
}