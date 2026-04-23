using System;
using UnityEngine;

namespace RealMethod
{
    public enum StorageMode
    {
        [DescriptionEnum("Instantiated uniquely for the owner(self).")]
        Exclusive = 0,

        [DescriptionEnum("Shared instance obtained from an FileAsset, SaveSystem, or any other global provider.")]
        Shared = 1
    }
    [Serializable]
    public struct Storage<T> where T : IFile
    {
        [SerializeField]
        private StorageMode Mode;
        [Space]
        [SerializeField, ConditionalShowByEnum("Mode", StorageMode.Shared)]
        private bool UseAsset;
        [SerializeField, ConditionalShowByEnum("Mode", StorageMode.Shared), ConditionalHide("UseAsset", true, false)]
        private SaveAsset FileAsset;
        [SerializeField, ConditionalShowByEnum("Mode", StorageMode.Shared), ConditionalHide("UseAsset", true, true), ShowOnly]
        private string SelectedMergeFile;
        [SerializeField, ConditionalShowByEnum("Mode", StorageMode.Exclusive)]
        private bool UseSelf;
        [SerializeField, ConditionalShowByEnum("Mode", StorageMode.Exclusive), ConditionalHide("UseSelf", true, true)]
        private string FileName;
        [SerializeField, ConditionalShowByEnum("Mode", StorageMode.Exclusive), ConditionalHide("UseSelf", true, true)]
        private SoftType<ISaveFile> FileClass;


        // Private Fields
        private UnityEngine.Object _owner;
        private T _file;
        private ISaveSystem _saveSystem;
        private IFile _mergeFile;

        // Properties
        private ISaveSystem saveSystem
        {
            get
            {
                if (_saveSystem == null)
                {
                    _saveSystem = Game.Instance.SaveSystem;
                }
                return _saveSystem;
            }
        }
        public T File
        {
            get
            {
                if (_file == null)
                {
                    Refresh();
                }

                return _file;
            }
        }
        public bool IsExist
        {
            get
            {
                return saveSystem.IsExist(_file);
            }
        }

        // Functions
        public void Refresh()
        {
            if (Mode == StorageMode.Shared)
            {
                if (UseAsset)
                {
                    if (FileAsset == null)
                    {
                        Debug.LogError($"Your FileAsset is not valid");
                        _file = default;
                    }

                    if (FileAsset is T provider)
                    {
                        _file = provider;
                    }
                    else
                    {
                        Debug.LogError($"Your FileAsset({FileAsset.name}) Should implement {typeof(T)} interface");
                        return;
                    }
                }
                else
                {
                    _mergeFile = saveSystem.MainSaveFile;
                    if (_mergeFile != null)
                    {
                        SelectedMergeFile = _mergeFile.GetObject().GetType().ToString();
                        if (_mergeFile is T provider)
                        {
                            _file = provider;
                        }
                        else
                        {
                            Debug.LogError($"Your MergeFile({_mergeFile.GetObject().GetType()}) Should implement {typeof(T)} interface");
                            return;
                        }
                    }
                    else
                    {
                        Debug.LogError($"MergeFile in savesystem is not valid");
                        return;
                    }
                }
            }
            else
            {
                if (UseSelf)
                {
                    if (_owner == null)
                    {
                        Debug.LogError($"Your SelfRefrence is not valid [use SetSelf(this) function befor get File]");
                        return;
                    }

                    if (_owner is T provider)
                    {
                        saveSystem.AddFile(provider);
                        _file = provider;
                    }
                    else
                    {
                        Debug.LogError($"Your SelfRefrence({_owner.name}) Should implement {typeof(T)} interface");
                        return;
                    }
                }
                else
                {
                    object instance = FileClass.Type.CreateInstance();
                    if (instance is T provider)
                    {
                        if (instance is ISaveFile saveProvider)
                        {
                            saveProvider.SetName(FileName);
                        }
                        saveSystem.AddFile(provider);
                        _file = provider;
                    }
                    else
                    {
                        Debug.LogError($"Your class type({FileClass}) Should implement {typeof(T)} interface");
                        return;
                    }
                }
            }

        }
        public void SetSelf(UnityEngine.Object self)
        {
            _owner = self;
        }
        public void Save()
        {
            saveSystem.Save(_file);
        }
        public void SetFileName(string NewName)
        {
            FileName = NewName;
        }
        public void Clear()
        {
            Debug.LogWarning("Clear Dint implement! Sorry");
        }
    }

}