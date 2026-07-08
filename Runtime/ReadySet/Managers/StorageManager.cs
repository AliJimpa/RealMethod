using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    [AddComponentMenu("RealMethod/Manager/SaveManager")]
    public sealed class StorageManager : SaveManager_Storage
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        [SerializeField]
        private bool Log = true;
        protected override void WriteLog(string message)
        {
            if (Log)
            {
                base.WriteLog(message);
            }
        }
#endif

        // SaveManager Method
        protected override bool CustomSavefile(IFile file, ISaveMethod Method, SaveState state)
        {
            Debug.LogWarning("Didn't Implement any CustomSaving");
            return false;
        }
    }
}