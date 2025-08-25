using UnityEditor;

namespace RealMethod.Editor
{
    class RealMethodScriptTemplate
    {
        // Essential
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Game", false, 80)]
        public static void CreateGameClass()
        {
            string Path = RM_Create.Script("GameTemplate.txt", "MyGame.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/World", false, 80)]
        public static void CreateWorld()
        {
            string Path = RM_Create.Script("WorldTemplate.txt", "MyWorld.cs");
        }

        // Managers
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/BaseManager", false, 80)]
        public static void CreateManager()
        {
            string Path = RM_Create.Script("ManagerTemplate.txt", "MyManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/AudioManager", false, 80)]
        public static void CreateAudioManager()
        {
            string Path = RM_Create.Script("AudioManagerTemplater.txt", "MyAudioManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/CompositManager", false, 80)]
        public static void CreateCompositManager()
        {
            string Path = RM_Create.Script("CompositManagerTemplate.txt", "MyCompositManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/DataManager", false, 80)]
        public static void CreateDataManager()
        {
            string Path = RM_Create.Script("DataManagerTemplate.txt", "MyDataManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/GizmoManager", false, 80)]
        public static void CreateGizmoManager()
        {
            string Path = RM_Create.Script("GizmoManagerTemplate.txt", "MyGizmoManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/UIManager", false, 80)]
        public static void CreateUIManager()
        {
            string Path = RM_Create.Script("UIManagerTemplate.txt", "MyUIManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/MixerManager", false, 80)]
        public static void CreateMixerManager()
        {
            string Path = RM_Create.Script("MixerManagerTemplate.txt", "MyMixerManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/HapticManager", false, 80)]
        public static void CreateHapticManager()
        {
            string Path = RM_Create.Script("HapticManagerTemplate.txt", "MyHapticManager.cs");
        }


        // Service
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/BaseService", false, 80)]
        public static void CreateService()
        {
            string Path = RM_Create.Script("ServiceTemplate.txt", "MyServicec.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/GameService", false, 80)]
        public static void CreateGameServiceClass()
        {
            string Path = RM_Create.Script("GameServiceTemplate.txt", "MyGameService.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/StateService", false, 80)]
        public static void CreateStateService()
        {
            string Path = RM_Create.Script("StateServiceTemplate.txt", "MyStateService.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/RuleService", false, 80)]
        public static void CreateRuleService()
        {
            string Path = RM_Create.Script("RuleServiceTemplate.txt", "MyRuleService.cs");
        }
        


        // Assets
        // // Data
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Data/BaseData", false, 80)]
        public static void CreateDataAsset()
        {
            string Path = RM_Create.Script("DataAssetTemplate.txt", "MyDataAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Data/ItemAsset", false, 80)]
        public static void CreateItemAsset()
        {
            string Path = RM_Create.Script("ItemAssetTemplate.txt", "MyItemAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Data/TableAsset", false, 80)]
        public static void CreateTableAsset()
        {
            string Path = RM_Create.Script("TableTemplate.txt", "MyTableAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Data/Task", false, 80)]
        public static void CreateTaskAsset()
        {
            string Path = RM_Create.Script("TaskAssetTemplate.txt", "MyTaskAsset.cs");
        }
        // // Config
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Config/BaseConfig", false, 80)]
        public static void CreateConfigAsset()
        {
            string Path = RM_Create.Script("ConfigAssetTemplate.txt", "MyConfigAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Config/ItemConfig", false, 80)]
        public static void CreateItemConfig()
        {
            string Path = RM_Create.Script("ItemConfigTemplate.txt", "MyItemConfig.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Config/GameConfig", false, 80)]
        public static void CreateGameConfig()
        {
            string Path = RM_Create.Script("GameConfigTemplate.txt", "MyGameConfig.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Config/Haptic", false, 80)]
        public static void CreateHapticConfig()
        {
            string Path = RM_Create.Script("HapticConfigTemplate.txt", "MyHaptic.cs");
        }
        // // Files
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/File/BaseFile", false, 80)]
        public static void CreateFileAsset()
        {
            string Path = RM_Create.Script("FileAssetTemplate.txt", "MyFileAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/File/SaveFile", false, 80)]
        public static void CreateSaveFile()
        {
            string Path = RM_Create.Script("SaveFileTemplate.txt", "MySaveFile.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/File/GameSetting", false, 80)]
        public static void CreateGameSettingFile()
        {
            string Path = RM_Create.Script("GameSettingFileTemplate.txt", "GameSettingFile.cs");
        }
        // // Unique
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Unique/BaseUnique", false, 80)]
        public static void CreateUniqueAsset()
        {
            string Path = RM_Create.Script("UniqueAssetTemplate.txt", "MyFileAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Unique/SharedRootAsset", false, 80)]
        public static void CreateSharedRootAsset()
        {
            string Path = RM_Create.Script("SharedRootAssetTemplate.txt", "MySharedRootAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/Unique/PoolAsset", false, 80)]
        public static void CreatePoolAsset()
        {
            string Path = RM_Create.Script("PoolAssetTemplate.txt", "MyPoolAsset.cs");
        }

        // General
        // // UI
        [MenuItem("Assets/Create/Scripting/RealMethod/General/UI/Widget", false, 80)]
        public static void CreateWidget()
        {
            string Path = RM_Create.Script("WidgetTemplate.txt", "MyWidget.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/UI/WidgetToolkit", false, 80)]
        public static void CreateWidgetToolkit()
        {
            string Path = RM_Create.Script("WidgetToolkitTemplate.txt", "MyWidgetToolkit.cs");
        }
        // // Command
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command", false, 80)]
        public static void CreateCommand()
        {
            string Path = RM_Create.Script("CommandTemplate.txt", "MyCommand.cs");
        }
        // // Trigger
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Trigger/Trigger3D", false, 80)]
        public static void CreateTrigger3D()
        {
            string Path = RM_Create.Script("Trigger3DTemplate.txt", "MyTrigger.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Trigger/Trigger2D", false, 80)]
        public static void CreateTrigger2D()
        {
            string Path = RM_Create.Script("Trigger2DTemplate.txt", "MyTrigger.cs");
        }


        // Editor
        [MenuItem("Assets/Create/Scripting/RealMethod/Editor/SettingSection", false, 80)]
        public static void CreateSettingSection()
        {
            string Path = RM_Create.Script("SettingSectionTemplate.txt", "MySection.cs");
        }

















    }
}