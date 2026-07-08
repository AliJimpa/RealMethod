using UnityEngine;

public class SlugAttribute : PropertyAttribute
{
    // Scale the width (0.0 = tiny, 1.0 = normal)
    public float WidthScale = 0.5f;
    public SlugAttribute(float _widthScale = 0.5f)
    {
        WidthScale = _widthScale;
    }
}
