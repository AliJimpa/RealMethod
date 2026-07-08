using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    /// [HelpBox("This is the first line of the help box.\nThis is the second line of the help box.", {0,1,2,3} )]
    /// [TextArea(3, 10)]
    /// public string Variable;
    /// </code>
    /// {0:None,1:Info,2:Warning,3:Error}
    /// </summary>
    public sealed class HelpBoxAttribute : PropertyAttribute
    {
        public string text;
        public int messageType;
        public int height;

        public HelpBoxAttribute(string text, int messageType = 0, int Boxheight = 2)
        {
            this.text = text;
            this.messageType = messageType;
            height = Boxheight;
        }
    }
}
