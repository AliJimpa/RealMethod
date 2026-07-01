using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    // Real Method Setting Storage
    public class ProjectSettingAsset : PrimitiveAsset
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
            public string GetFolderPath()
            {
                string RootPath = "Assets/" + Application.productName;
                return $"{RootPath}/{AssetPath}";
            }

        }


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
        [Header("GameStatus")]
        [ReadOnly]
        public List<string> Status = new List<string>(5) { "Default", "Menu", "Playing", "Pause", "GameOver" };
#endif
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [Header("Spectator")]
        [SerializeField]
        private Prefab SpectatorPrefab; // <-- this name must match
        [Header("Debug")]
        public float Log_Duration = 5;
        public Color Log_Color = Color.cyan;
        public float Warning_Duration = 10;
        public Color Warning_Color = Color.yellow;
        public float Error_Duration = 15;
        public Color Error_Color = Color.red;
        public float Assert_Duration = 15;
        public Color Assert_Color = new Color(1f, 0.4941176f, 0.4941176f);
        public float Exception_Duration = 15;
        public Color Exception_Color = Color.blue;
#endif
#if UNITY_EDITOR
        [Header("CompileGuard")]
        [SerializeField, ReadOnly]
        private bool Compiling = true; // <-- this name must match
        [SerializeField, ReadOnly]
        private string[] Rules = null; // <-- this name must match
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
        public GameObject GetSpectator()
        {
            return SpectatorPrefab;
        }

#if UNITY_EDITOR
        protected sealed override void EnsureAssetPermission()
        {
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
            return GetFolderAddressByType(identity).GetFolderPath();
        }
        public FolderAddress GetFolderAddressByIndex(int index) => folderStructure[index];
        public void SetFolderAddressPath(int index, string value) => folderStructure[index].AssetPath = value;
        public bool CanCompile()
        {
            return Compiling;
        }
        public Type[] GetCompileRules()
        {
            List<Type> result = new List<Type>();
            foreach (var item in Rules)
            {
                result.Add(Type.GetType(item));
            }
            return result.ToArray();
        }
#endif

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        public void ResetLogColor()
        {
            Log_Duration = 5;
            Log_Color = Color.cyan;
            Warning_Duration = 10;
            Warning_Color = Color.yellow;
            Error_Duration = 15;
            Error_Color = Color.red;
            Assert_Duration = 15;
            Assert_Color = new Color(1f, 0.4941176f, 0.4941176f);
            Exception_Duration = 15;
            Exception_Color = Color.blue;
        }
#endif

#if UNITY_SERVER || UNITY_EDITOR
        public GameObject GetPrefab_3()
        {
            return GamePrefab_3;
        }
#endif
    }


}


