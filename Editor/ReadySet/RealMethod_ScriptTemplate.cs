using UnityEditor;

namespace RealMethod.Editor
{
    class RealMethodScriptTemplate
    {

        // Essential
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Game", false, 80)]
        public static void CreateGameClass()
        {
            string Path = ScriptCreator.Create("GameTemplate.txt", "MyGame.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/World", false, 80)]
        public static void CreateWorld()
        {
            string Path = ScriptCreator.Create("WorldTemplate.txt", "MyWorld.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Service", false, 80)]
        public static void CreateService()
        {
            string Path = ScriptCreator.Create("ServiceTemplate.txt", "MyServicec.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/GameSetting", false, 80)]
        public static void CreateGameSettingAsset()
        {
            string Path = ScriptCreator.Create("GameSettingAssetTemplate.txt", "MyGameSetting.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Manager", false, 80)]
        public static void CreateManager()
        {
            string Path = ScriptCreator.Create("ManagerTemplate.txt", "MyManager.cs");
        }


        // Managers
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/AudioManager", false, 80)]
        public static void CreateAudioManager()
        {
            string Path = ScriptCreator.Create("AudioManagerTemplater.txt", "MyAudioManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/CompositManager", false, 80)]
        public static void CreateCompositManager()
        {
            string Path = ScriptCreator.Create("CompositManagerTemplate.txt", "MyCompositManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/DataManager", false, 80)]
        public static void CreateDataManager()
        {
            string Path = ScriptCreator.Create("DataManagerTemplate.txt", "MyDataManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/GizmoManager", false, 80)]
        public static void CreateGizmoManager()
        {
            string Path = ScriptCreator.Create("GizmoManagerTemplate.txt", "MyGizmoManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/UIManager", false, 80)]
        public static void CreateUIManager()
        {
            string Path = ScriptCreator.Create("UIManagerTemplate.txt", "MyUIManager.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/MixerManager", false, 80)]
        public static void CreateMixer()
        {
            string Path = ScriptCreator.Create("MixerManagerTemplate.txt", "MyMixerManager.cs");
        }


        // Service
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/GameService", false, 80)]
        public static void CreateGameServiceClass()
        {
            string Path = ScriptCreator.Create("GameServiceTemplate.txt", "MyGameService.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/StateService", false, 80)]
        public static void CreateStateService()
        {
            string Path = ScriptCreator.Create("StateServiceTemplate.txt", "MyStateService.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/RuleService", false, 80)]
        public static void CreateRuleService()
        {
            string Path = ScriptCreator.Create("RuleServiceTemplate.txt", "MyRuleService.cs");
        }


        // Assets
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/DataAsset", false, 80)]
        public static void CreateDataAsset()
        {
            string Path = ScriptCreator.Create("DataAssetTemplate.txt", "MyDataAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/ItemAsset", false, 80)]
        public static void CreateItemAsset()
        {
            string Path = ScriptCreator.Create("ItemAssetTemplate.txt", "MyItemAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/ConfigAsset", false, 80)]
        public static void CreateConfigAsset()
        {
            string Path = ScriptCreator.Create("ConfigAssetTemplate.txt", "MyConfigAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/SharedRootAsset", false, 80)]
        public static void CreateSharedRootAsset()
        {
            string Path = ScriptCreator.Create("SharedRootAssetTemplate.txt", "MySharedRootAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/PoolAsset", false, 80)]
        public static void CreatePoolAsset()
        {
            string Path = ScriptCreator.Create("PoolAssetTemplate.txt", "MyPoolAsset.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/SaveAsset", false, 80)]
        public static void CreateSaveFile()
        {
            string Path = ScriptCreator.Create("SaveFileTemplate.txt", "MySaveFile.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/TableAsset", false, 80)]
        public static void CreateTableAsset()
        {
            string Path = ScriptCreator.Create("TableTemplate.txt", "MyTableAsset.cs");
        }


        // UI
        [MenuItem("Assets/Create/Scripting/RealMethod/General/UI/Widget", false, 80)]
        public static void CreateWidget()
        {
            string Path = ScriptCreator.Create("WidgetTemplate.txt", "MyWidget.cs");
        }
        // Pattern
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/BaseCommand", false, 80)]
        public static void CreateCommand()
        {
            string Path = ScriptCreator.Create("CommandTemplate.txt", "MyCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/ExecutCommand", false, 80)]
        public static void CreateExecutCommand()
        {
            string Path = ScriptCreator.Create("ExecutCommandTemplate.txt", "MyExecutCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/TargetedCommand", false, 80)]
        public static void CreateTargetedCommand()
        {
            string Path = ScriptCreator.Create("TargetedCommandTemplate.txt", "MyTargetedCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/LifecycleCommand", false, 80)]
        public static void CreateLifecycleCommand()
        {
            string Path = ScriptCreator.Create("LifecycleCommandTemplate.txt", "MyLifecycleCommand.cs");
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/General/Command/ActionCommand", false, 80)]
        public static void CreateActionCommand()
        {
            string Path = ScriptCreator.Create("ActionCommandTemplate.txt", "MyActionCommand.cs");
        }



        // Editor
        [MenuItem("Assets/Create/Scripting/RealMethod/Editor/SettingSection", false, 80)]
        public static void CreateSettingSection()
        {
            string Path = ScriptCreator.Create("SettingSectionTemplate.txt", "MySection.cs");
        }

















    }
}