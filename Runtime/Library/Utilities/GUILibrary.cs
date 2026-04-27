#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace RealMethod
{
    public static class RM_GUI
    {
        public static Rect GetButtonRect(Corner corner, float width, float height, float margin = 10f)
        {
            float x = 0;
            float y = 0;

            switch (corner)
            {
                case Corner.UpLeft:
                    x = margin;
                    y = margin;
                    break;

                case Corner.UpRight:
                    x = Screen.width - width - margin;
                    y = margin;
                    break;

                case Corner.DownLeft:
                    x = margin;
                    y = Screen.height - height - margin;
                    break;

                case Corner.DownRight:
                    x = Screen.width - width - margin;
                    y = Screen.height - height - margin;
                    break;
            }

            return new Rect(x, y, width, height);
        }
    }

}
#endif