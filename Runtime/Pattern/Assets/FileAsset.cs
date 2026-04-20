using System;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    ///  A specialized UniqueAsset used for File system.
    /// The asset is uniqueAsset can instance at runtime.
    /// and developer can use fileasset for saving data in asset,
    /// or in runtime asset creation.
    /// The asset itself cannot be used cloned.
    /// Instead, the system creates new independent instances based on this asset's data.
    /// </summary>
    public abstract class FileAsset : UniqueAsset, IFile
    {
        [Header("File")]
        [SerializeField, TextArea(5, 20)]
        private string fileContents;
        private DateTime createTime;
        private DateTime modifiedTime;


        // Implement IFile Interface
        string IFile.Name => name;
        DateTime IFile.CreateTime => createTime;
        DateTime IFile.ModifiedTime => modifiedTime;
        public object GetObject() => this;


        // Unity Events
        protected virtual void OnEnable()
        {
            createTime = DateTime.Now;
        }

        // Virtual Methods
        protected sealed override void EnsureAssetPermission()
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (HasCloneName())
            {
                Debug.LogError($"[{name}] FileAsset cannot clone at runtime. NewAsset has been removed!");
                DestroyImmediate(this);
                return;
            }
            if (IsSpawned)
            {
                if (IsProjectAsset())
                {
                    Debug.LogError($"[{name}] This asset is direct asset but Spawn Event called. Asset has been removed!");
                    DestroyImmediate(this);
                    return;
                }
            }
#endif
        }

        // Protected Methods
        protected void Modifiy()
        {
            modifiedTime = DateTime.Now;
            EnsureAssetPermission();
        }

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            Modifiy();
        }


#endif
    }
}