using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    // Real Method Setting Storage
    public class ProjectSettingAsset : ScriptableObject
    {
        [Serializable]
        public enum AssetFormat
        {
            Scene = 0,
            Script = 1,
            Prefab = 2,
            ScriptableObject = 3,
            Mesh = 4,
            Sprite = 5,
            Texture = 6,
            Video = 7,
            Material = 8,
            Shader = 9,
            Audio = 10,
            Particle = 11,
            Animationclip = 12,
            Other = 13
        }
        [Serializable]
        public struct FolderAddress
        {
            public AssetFormat AssetType;
            public string AssetPath;
            public string FolderName => System.IO.Path.GetFileName(AssetPath);
            public string GetFolderPath(ProjectSettingAsset settingAsset)
            {
                string RootPath = "Assets/" + Application.productName;
                return $"{RootPath}/{AssetPath}";
            }

        }

        public static string Path = "Assets/Resources/RealMethod/RealMethodSetting.asset";

        [Header("Initializer")]
        [SerializeField, ReadOnly]
        private string GameClass = string.Empty; // <-- this name must match
        [SerializeField, ReadOnly]
        private string GameBridge = string.Empty; // <-- this name must match
        [SerializeField]
        private GameConfig GameConfig; // <-- this name must match
        [SerializeField]
        private GameObject GamePrefab_1; // <-- this name must match

#if UNITY_EDITOR
        [SerializeField]
        private GameObject GamePrefab_2; // <-- this name must match
#endif

#if UNITY_SERVER || UNITY_EDITOR
        [SerializeField]
        private GameObject GamePrefab_3; // <-- this name must match
#endif

#if UNITY_EDITOR
        [Header("FolderStructure")]
        [SerializeField, ReadOnly]
        private FolderAddress[] folderStructure = new FolderAddress[14]
        {
        new FolderAddress { AssetType = 0, AssetPath = "1_Scenes"},
        new FolderAddress { AssetType = (AssetFormat)1, AssetPath = "2_Scripts" },
        new FolderAddress { AssetType = (AssetFormat)2, AssetPath = "3_Prefabs"},
        new FolderAddress { AssetType = (AssetFormat)3, AssetPath = "4_Data"  },
        new FolderAddress { AssetType = (AssetFormat)4, AssetPath = "5_Mesh"},
        new FolderAddress { AssetType = (AssetFormat)5, AssetPath = "5_Sprite"},
        new FolderAddress { AssetType = (AssetFormat)6, AssetPath = "7_Misc/Textures"},
        new FolderAddress { AssetType = (AssetFormat)7, AssetPath = "7_Misc/Videos"},
        new FolderAddress { AssetType = (AssetFormat)8, AssetPath = "7_Misc/Materials"},
        new FolderAddress { AssetType = (AssetFormat)9, AssetPath = "6_Shader"},
        new FolderAddress { AssetType = (AssetFormat)10, AssetPath = "8_Sound&Music"},
        new FolderAddress { AssetType = (AssetFormat)11, AssetPath = "9_VFX"},
        new FolderAddress { AssetType = (AssetFormat)12, AssetPath = "10_Animation"},
        new FolderAddress { AssetType = (AssetFormat)13, AssetPath = "7_Misc"}
        };
        public IReadOnlyList<FolderAddress> FolderStructure => folderStructure;
#endif

#if UNITY_EDITOR
        [Header("GameStatus")]
        [ReadOnly]
        public List<string> Status = new List<string>(4) { "Menu", "Playing", "Pause", "GameOver" };
#endif


        // Unity Methods
        protected virtual void OnEnable()
        {
            if (string.IsNullOrEmpty(GameClass))
                GameClass = typeof(DefultGame).AssemblyQualifiedName;
            if (string.IsNullOrEmpty(GameBridge))
                GameBridge = typeof(DefaultGameBridge).AssemblyQualifiedName;
        }
        protected virtual void Reset()
        {
            if (string.IsNullOrEmpty(GameClass))
                GameClass = typeof(DefultGame).AssemblyQualifiedName;
            if (string.IsNullOrEmpty(GameBridge))
                GameBridge = typeof(DefaultGameBridge).AssemblyQualifiedName;
        }



        // Public Functions
        public Type GetGameType()
        {
            return Type.GetType(GameClass);
        }
        public Type GetBridgeType()
        {
            return Type.GetType(GameBridge);
        }
        public GameConfig GetGameConfigAsset()
        {
            return GameConfig;
        }
        public GameObject GetPrefab_1()
        {
            return GamePrefab_1;
        }

#if UNITY_EDITOR
        public static ProjectSettingAsset Loaded()
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>(Path);
        }
        public GameObject GetPrefab_2()
        {
            return GamePrefab_2;
        }
        public FolderAddress GetFolderAddressByType(AssetFormat identity)
        {
            foreach (var PS in folderStructure)
            {
                if (PS.AssetType == identity)
                {
                    return PS;
                }
            }
            return default;
        }
        public string GetFolderPathByType(AssetFormat identity)
        {
            return GetFolderAddressByType(identity).GetFolderPath(this);
        }
        public FolderAddress GetFolderAddressByIndex(int index) => folderStructure[index];
        public void SetFolderAddressPath(int index, string value) => folderStructure[index].AssetPath = value;
#endif

#if UNITY_SERVER || UNITY_EDITOR
        public GameObject GetPrefab_3()
        {
            return GamePrefab_3;
        }
#endif


    }


}


