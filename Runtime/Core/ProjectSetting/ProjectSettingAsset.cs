using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    // Real Method Setting Storage
    public class ProjectSettingAsset : ScriptableObject
    {
        private string ProjectName;
        [Serializable]
        public struct FolderAddress
        {
            public AssetFormat AssetType;
            public string AssetPath;
            public string FolderName => System.IO.Path.GetFileName(AssetPath);
            public string GetFolderPath(ProjectSettingAsset settingAsset)
            {
                string RootPath = settingAsset.GetStructureType() == 0 ? "Assets" : "Assets/"+Application.productName;
                return $"{RootPath}/{AssetPath}";
            }

        }
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
        public enum FolderStructureType
        {
            Assets = 0,
            ProjectName = 1,
        }


        [Header("Initializer")]
        [SerializeField, ReadOnly]
        private string GameClass = "RealMethod.DefultGame";
        [SerializeField, ReadOnly]
        private string GameService = "RealMethod.DefaultGameService";
        [SerializeField]
        private GameConfig GameConfig;
        [SerializeField]
        private GameObject GamePrefab_1;
        [SerializeField]
        private GameObject GamePrefab_2;
        [SerializeField]
        private GameObject GamePrefab_3;
        [Header("FolderStructure")]
        [SerializeField, ReadOnly]
        private FolderStructureType structureType;
        [SerializeField, ReadOnly]
        private FolderAddress[] projectStructure = new FolderAddress[14]
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
        public IReadOnlyList<FolderAddress> ProjectStructure => projectStructure;


        // Access values
        public string this[AssetFormat type]
        {
            get => GetFolderAddressByType(type).GetFolderPath(this);
        }

        // Public Functions
        public Type GetGameInstanceType()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(GameClass);
                if (type != null)
                    return type;
            }
            return null;
        }
        public void SetGameInstanceType(Type type)
        {
            // Store fully qualified name of the type
            if (type != null)
            {
                GameClass = type.AssemblyQualifiedName;
            }
            else
            {
                Debug.LogError("Type is null. Cannot set GameInstanceClass.");
            }
        }
        public Type GetGameServiceType()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(GameService);
                if (type != null)
                    return type;
            }
            return null;
        }
        public void SetGameServiceType(Type type)
        {
            // Store fully qualified name of the type
            if (type != null)
            {
                GameService = type.AssemblyQualifiedName;
            }
            else
            {
                Debug.LogError("Type is null. Cannot set GameInstanceClass.");
            }

        }
        public GameConfig GetGameConfig()
        {
            return GameConfig;
        }
        public GameObject[] GetGamePrefabs()
        {
            return new GameObject[3] {
                                GamePrefab_1,
                                GamePrefab_2,
                                GamePrefab_3,
                            };
        }
        public int GetStructureType()
        {
            return (int)structureType;
        }
        public void SetStructureType(int type)
        {
            structureType = (FolderStructureType)type;
        }
        public FolderAddress GetFolderAddressByIndex(int index) => projectStructure[index];
        public void SetFolderAddressPath(int index, string value) => projectStructure[index].AssetPath = value;
        public FolderAddress GetFolderAddressByType(AssetFormat identity)
        {
            foreach (var PS in projectStructure)
            {
                if (PS.AssetType == identity)
                {
                    return PS;
                }
            }
            return default(FolderAddress);
        }
    }


}


