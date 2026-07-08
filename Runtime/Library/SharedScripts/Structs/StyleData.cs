#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
namespace RealMethod
{
    [System.Serializable]
    public struct GUIStyleStateData
    {
        public Texture2D background;
        public Color textColor;


        public void Apply(GUIStyleState state)
        {
            state.background = background;
            state.textColor = textColor;
        }
    }

    [System.Serializable]
    public struct GUIStyleData
    {
        public Font font;
        public int fontSize;
        public FontStyle fontStyle;

        public TextAnchor alignment;
        public bool wordWrap;
        public bool richText;
        public bool clipping;

        public Vector2 contentOffset;

        public RectOffset padding;
        public RectOffset margin;
        public RectOffset border;
        public RectOffset overflow;

        public float fixedWidth;
        public float fixedHeight;

        public bool stretchWidth;
        public bool stretchHeight;

        public GUIStyleStateData normal;
        public GUIStyleStateData hover;
        public GUIStyleStateData active;
        public GUIStyleStateData focused;

        public GUIStyleStateData onNormal;
        public GUIStyleStateData onHover;
        public GUIStyleStateData onActive;
        public GUIStyleStateData onFocused;

        public GUIStyle Build()
        {
            GUIStyle style = new GUIStyle();

            style.font = font;
            style.fontSize = fontSize;
            style.fontStyle = fontStyle;

            style.alignment = alignment;
            style.wordWrap = wordWrap;
            style.richText = richText;
            style.clipping = clipping ? TextClipping.Clip : TextClipping.Overflow;

            style.contentOffset = contentOffset;

            style.padding = padding;
            style.margin = margin;
            style.border = border;
            style.overflow = overflow;

            style.fixedWidth = fixedWidth;
            style.fixedHeight = fixedHeight;

            style.stretchWidth = stretchWidth;
            style.stretchHeight = stretchHeight;

            normal.Apply(style.normal);
            hover.Apply(style.hover);
            active.Apply(style.active);
            focused.Apply(style.focused);

            onNormal.Apply(style.onNormal);
            onHover.Apply(style.onHover);
            onActive.Apply(style.onActive);
            onFocused.Apply(style.onFocused);

            return style;
        }
    }
}
#endif