using System;
using UnityEngine;

// Real Method Setting Storage
public class RMSS : ScriptableObject
{
    [Serializable]
    public struct FolderAddress
    {
        public IdentityCategory Identity;
        public string Path;
        public string FolderName => System.IO.Path.GetFileName(Path);
    }

    [Serializable]
    public enum IdentityCategory
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
    }


    public GameData GeneralGameData;
    [SerializeField]
    private string GameClass;
    public FolderAddress[] ProjectStructure = new FolderAddress[17]
    {
        new FolderAddress { Identity = 0, Path = "Assets/1_Scenes"},
        new FolderAddress { Identity = (IdentityCategory)1, Path = "Assets/2_Scripts" },
        new FolderAddress { Identity = (IdentityCategory)2, Path = "Assets/3_Prefabs"},
        new FolderAddress { Identity = (IdentityCategory)3, Path = "Assets/4_Data"  },
        new FolderAddress { Identity = (IdentityCategory)4, Path = "Assets/5_Meshes"},
        new FolderAddress { Identity = (IdentityCategory)5, Path = "Assets/Sprites"},
        new FolderAddress { Identity = (IdentityCategory)6, Path = "Assets/7_Misc/Textures"},
        new FolderAddress { Identity = (IdentityCategory)7, Path = "Assets/7_Misc/Videos"},
        new FolderAddress { Identity = (IdentityCategory)8, Path = "Assets/7_Misc/Materials"},
        new FolderAddress { Identity = (IdentityCategory)9, Path = "Assets/6_Shader"},
       new FolderAddress { Identity = (IdentityCategory)10, Path = "Assets/8_Sound&Music"},
        new FolderAddress { Identity = (IdentityCategory)11, Path = "Assets/9_VFX"},
        new FolderAddress { Identity = (IdentityCategory)12, Path = "Assets/10_Animation"},
        new FolderAddress { Identity = (IdentityCategory)13, Path = "Assets/7_Misc"},
        new FolderAddress { Identity = (IdentityCategory)14, Path = "Assets/Developer"},
        new FolderAddress { Identity = (IdentityCategory)15, Path = "Assets/Resources"},
        new FolderAddress { Identity = (IdentityCategory)16, Path = "Assets/~Thirdparty"}
    };



    public Type GetGameClass()
    {
        return System.Type.GetType(GameClass);
    }
    public void SetGameClass(Type type)
    {
        GameClass = type.AssemblyQualifiedName; // Store fully qualified name
    }

}
