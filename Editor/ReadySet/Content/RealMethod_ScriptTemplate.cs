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
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Service", false, 80)]
        public static void CreateService()
        {
            string Path = RM_Create.Script("ServiceTemplate.txt", "MyServicec.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/GameConfig", false, 80)]
        public static void CreateGameConfig()
        {
            string Path = RM_Create.Script("GameConfigTemplate.txt", "MyGameConfig.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Manager", false, 80)]
        public static void CreateManager()
        {
            string Path = RM_Create.Script("ManagerTemplate.txt", "MyManager.cs");
        }


        // Managers
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
        public static void CreateMixer()
        {
            string Path = RM_Create.Script("MixerManagerTemplate.txt", "MyMixerManager.cs");
        }


        // Service
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
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/HapticService", false, 80)]
        public static void CreateHapticService()
        {
            string Path = RM_Create.Script("HapticServiceTemplate.txt", "MyHapticService.cs");
        }


        // Assets
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/DataAsset", false, 80)]
        public static void CreateDataAsset()
        {
            string Path = RM_Create.Script("DataAssetTemplate.txt", "MyDataAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/ItemAsset", false, 80)]
        public static void CreateItemAsset()
        {
            string Path = RM_Create.Script("ItemAssetTemplate.txt", "MyItemAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/SharedRootAsset", false, 80)]
        public static void CreateSharedRootAsset()
        {
            string Path = RM_Create.Script("SharedRootAssetTemplate.txt", "MySharedRootAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/PoolAsset", false, 80)]
        public static void CreatePoolAsset()
        {
            string Path = RM_Create.Script("PoolAssetTemplate.txt", "MyPoolAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/SaveAsset", false, 80)]
        public static void CreateSaveFile()
        {
            string Path = RM_Create.Script("SaveFileTemplate.txt", "MySaveFile.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/GameSetting", false, 80)]
        public static void CreateGameSettingFile()
        {
            string Path = RM_Create.Script("GameSettingFileTemplate.txt", "GameSettingFile.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/TableAsset", false, 80)]
        public static void CreateTableAsset()
        {
            string Path = RM_Create.Script("TableTemplate.txt", "MyTableAsset.cs");
        }

        //Config
        [MenuItem("Assets/Create/Scripting/RealMethod/Config/ConfigAsset", false, 80)]
        public static void CreateConfigAsset()
        {
            string Path = RM_Create.Script("ConfigAssetTemplate.txt", "MyConfigAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Config/Haptic", false, 80)]
        public static void CreateHapticConfig()
        {
            string Path = RM_Create.Script("HapticConfigTemplate.txt", "MyHaptic.cs");
        }



        // UI
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
        
        // Pattern
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/BaseCommand", false, 80)]
        public static void CreateCommand()
        {
            string Path = RM_Create.Script("CommandTemplate.txt", "MyCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/ExecutCommand", false, 80)]
        public static void CreateExecutCommand()
        {
            string Path = RM_Create.Script("ExecutCommandTemplate.txt", "MyExecutCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/TargetedCommand", false, 80)]
        public static void CreateTargetedCommand()
        {
            string Path = RM_Create.Script("TargetedCommandTemplate.txt", "MyTargetedCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/LifecycleCommand", false, 80)]
        public static void CreateLifecycleCommand()
        {
            string Path = RM_Create.Script("LifecycleCommandTemplate.txt", "MyLifecycleCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/ActionCommand", false, 80)]
        public static void CreateActionCommand()
        {
            string Path = RM_Create.Script("ActionCommandTemplate.txt", "MyActionCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Component/Trigger3D", false, 80)]
        public static void CreateTrigger3D()
        {
            string Path = RM_Create.Script("Trigger3DTemplate.txt", "MyTrigger.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Component/Trigger2D", false, 80)]
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