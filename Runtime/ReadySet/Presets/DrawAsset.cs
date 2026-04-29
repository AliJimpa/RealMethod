using System;
using UnityEngine;

namespace RealMethod
{
    [CreateAssetMenu(fileName = "Draw", menuName = "RealMethod/Develop/DrawAsset", order = 1)]
    public class DrawAsset : GUIAsset
    {
        public event Action<Name16> OnButtonClicked;
        public event Action<Name16, bool> OnToggleChanged;


        // GUIAsset Methods
        public override void OnButtonClick(string ButtonName)
        {
            OnButtonClicked?.Invoke(ButtonName);
        }
        public override void OnToggleChange(string ToggleName, bool Val)
        {
            OnToggleChanged?.Invoke(ToggleName, Val);
        }


    }
}