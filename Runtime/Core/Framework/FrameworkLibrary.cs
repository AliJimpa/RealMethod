using System;

namespace RealMethod
{
    public static class RM_Framework
    {
        public static bool IsCinemachineAvailable => Type.GetType("Cinemachine.CinemachineBrain, Cinemachine") != null;
    }
}