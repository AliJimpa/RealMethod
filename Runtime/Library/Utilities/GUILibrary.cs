#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;

namespace RealMethod
{
    public static class RM_GUI
    {
        public static Rect GetButtonRect(Dir9 corner, float width, float height, float margin = 10f)
        {
            float x = 0;
            float y = 0;

            switch (corner)
            {
                case Dir9.UpLeft:
                    x = margin;
                    y = margin;
                    break;

                case Dir9.Up:
                    x = (Screen.width - width) * 0.5f;
                    y = margin;
                    break;

                case Dir9.UpRight:
                    x = Screen.width - width - margin;
                    y = margin;
                    break;

                case Dir9.Left:
                    x = margin;
                    y = (Screen.height - height) * 0.5f;
                    break;

                case Dir9.Right:
                    x = Screen.width - width - margin;
                    y = (Screen.height - height) * 0.5f;
                    break;

                case Dir9.DownLeft:
                    x = margin;
                    y = Screen.height - height - margin;
                    break;

                case Dir9.Down:
                    x = (Screen.width - width) * 0.5f;
                    y = Screen.height - height - margin;
                    break;

                case Dir9.DownRight:
                    x = Screen.width - width - margin;
                    y = Screen.height - height - margin;
                    break;

                case Dir9.Center:
                    x = (Screen.width - width) * 0.5f;
                    y = (Screen.height - height) * 0.5f;
                    break;
            }

            return new Rect(x, y, width, height);
        }

    }

}
#endif