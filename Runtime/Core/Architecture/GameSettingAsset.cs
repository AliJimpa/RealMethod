using System;
using UnityEngine;

namespace RealMethod
{
    public abstract class GameSettingAsset : DataAsset
    {
        [Header("Srvice")]
        public Action<Service> OnNewService;
        [Header("FadeScreen")]
        public Action<bool> OnScreenFade;
        public float FadingTime = 1;
        [Header("LoadScreen")]
        public Action<bool> OnScreenLoading;
        public Action<float> OnLoadingProcess;
        [Header("Status")]
        public Action<bool> OnSceneLoading;

    }

}