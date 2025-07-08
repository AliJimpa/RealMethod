using System;
using Unity.VisualScripting;
using UnityEngine;

namespace RealMethod
{
    // Real Method Setting Storage
    public class ProjectSettingAsset : ScriptableObject
    {
        [Serializable]
        public struct FolderAddress
        {
            public IdentityAsset Identity;
            public string Path;
            public string FolderName => System.IO.Path.GetFileName(Path);
        }

        [Serializable]
        public enum IdentityAsset
        {
            Scene = 0,
            Script = 1,
            Prefab = 2,
            ScriptableObject = 3,
            Geometry = 4,
            Sprite = 5,
            Texture = 6,
            Video = 7,
            Material = 8,
            Shader = 9,
            Audio = 10,
            Particle = 11,
            Animationclip = 12,
            Miscellaneous = 13,
            User = 14,
            Resources = 15,
            ThirdpartyPack = 16,
            PCG = 17,
            ScriptTemplate,
        }


        [Header("Initializer")]
        [SerializeField]
        private string GameClass = "RealMethod.DefultGame";
        [SerializeField]
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
        public FolderAddress[] ProjectStructure = new FolderAddress[19]
        {
        new FolderAddress { Identity = 0, Path = "Assets/1_Scenes"},
        new FolderAddress { Identity = (IdentityAsset)1, Path = "Assets/2_Scripts" },
        new FolderAddress { Identity = (IdentityAsset)2, Path = "Assets/3_Prefabs"},
        new FolderAddress { Identity = (IdentityAsset)3, Path = "Assets/4_Data"  },
        new FolderAddress { Identity = (IdentityAsset)4, Path = "Assets/5_Meshes"},
        new FolderAddress { Identity = (IdentityAsset)5, Path = "Assets/Sprites"},
        new FolderAddress { Identity = (IdentityAsset)6, Path = "Assets/7_Misc/Textures"},
        new FolderAddress { Identity = (IdentityAsset)7, Path = "Assets/7_Misc/Videos"},
        new FolderAddress { Identity = (IdentityAsset)8, Path = "Assets/7_Misc/Materials"},
        new FolderAddress { Identity = (IdentityAsset)9, Path = "Assets/6_Shader"},
        new FolderAddress { Identity = (IdentityAsset)10, Path = "Assets/8_Sound&Music"},
        new FolderAddress { Identity = (IdentityAsset)11, Path = "Assets/9_VFX"},
        new FolderAddress { Identity = (IdentityAsset)12, Path = "Assets/10_Animation"},
        new FolderAddress { Identity = (IdentityAsset)13, Path = "Assets/7_Misc"},
        new FolderAddress { Identity = (IdentityAsset)14, Path = "Assets/Developer"},
        new FolderAddress { Identity = (IdentityAsset)15, Path = "Assets/Resources"},
        new FolderAddress { Identity = (IdentityAsset)16, Path = "Assets/~Thirdparty"},
        new FolderAddress { Identity = (IdentityAsset)17, Path = "Assets/4_Data/PCG"},
        new FolderAddress { Identity = (IdentityAsset)18, Path = "Assets/7_Misc/Templates/Scripts"}
        };


        public Type GetGameInstanceClass()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(GameClass);
                if (type != null)
                    return type;
            }
            return null;
        }
        public void SetGameInstanceClass(Type type)
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
        public Type GetGameServiceClass()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(GameService);
                if (type != null)
                    return type;
            }
            return null;
        }
        public void SetGameServiceClass(Type type)
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
        public FolderAddress FindAddres(IdentityAsset identity)
        {
            foreach (var PS in ProjectStructure)
            {
                if (PS.Identity == identity)
                {
                    return PS;
                }
            }
            return default(FolderAddress);
        }

    }


}


