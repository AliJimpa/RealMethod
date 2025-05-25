using UnityEditor;

namespace RealMethod
{
    class RealMethodScriptTemplate
    {
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/DataAsset", false, 80)]
        public static void CreateDataAsset()
        {
            string Path = ScriptCreator.Create("DataAssetTemplate.txt", "MyDataAsset.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/DataManager", false, 80)]
        public static void CreateDataManager()
        {
            string Path = ScriptCreator.Create("DataManagerTemplate.txt", "MyDataManager.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/GameService", false, 80)]
        public static void CreateGameServiceClass()
        {
            string Path = ScriptCreator.Create("GameServiceTemplate.txt", "MyGameService.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Game", false, 80)]
        public static void CreateGameClass()
        {
            string Path = ScriptCreator.Create("GameTemplate.txt", "MyGame.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/GizmoManager", false, 80)]
        public static void CreateGizmoManager()
        {
            string Path = ScriptCreator.Create("GizmoManagerTemplate.txt", "MyGizmoManager.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Manager", false, 80)]
        public static void CreateManager()
        {
            string Path = ScriptCreator.Create("ManagerTemplate.txt", "MyManager.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/MixerManager", false, 80)]
        public static void CreateMixer()
        {
            string Path = ScriptCreator.Create("MixerManagerTemplate.txt", "MyMixerManager.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/PoolAsset", false, 80)]
        public static void CreatePoolAsset()
        {
            string Path = ScriptCreator.Create("PoolAssetTemplate.txt", "MyPoolAsset.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/RuleService", false, 80)]
        public static void CreateRuleService()
        {
            string Path = ScriptCreator.Create("RuleServiceTemplate.txt", "MyRuleService.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/Service", false, 80)]
        public static void CreateService()
        {
            string Path = ScriptCreator.Create("ServiceTemplate.txt", "MyServicec.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Setting/SettingSection", false, 80)]
        public static void CreateSettingSection()
        {
            string Path = ScriptCreator.Create("SettingSectionTemplate.txt", "MySection.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/SharedRootAsset", false, 80)]
        public static void CreateSharedRootAsset()
        {
            string Path = ScriptCreator.Create("SharedRootAssetTemplate.txt", "MySharedRootAsset.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Service/StateService", false, 80)]
        public static void CreateStateService()
        {
            string Path = ScriptCreator.Create("StateServiceTemplate.txt", "MyStateService.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Asset/TableAsset", false, 80)]
        public static void CreateTableAsset()
        {
            string Path = ScriptCreator.Create("TableTemplate.txt", "MyTableAsset.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Manager/UIManager", false, 80)]
        public static void CreateUIManager()
        {
            string Path = ScriptCreator.Create("UIManagerTemplate.txt", "MyUIManager.cs", true);
        }
        [MenuItem("Assets/Create/Scripting/RealMethod/Essential/World", false, 80)]
        public static void CreateWorld()
        {
            string Path = ScriptCreator.Create("WorldTemplate.txt", "MyWorld.cs", true);
        }


    }
}