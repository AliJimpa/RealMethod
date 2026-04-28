#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// <code>
    ///  [ShowOnly] 
    ///  public float aaa = 123.45678f;
    ///  [ShowOnly]
    ///  public int bbb = 234;
    ///  [ShowOnly] 
    ///  public bool ccc = false;
    ///  [ShowOnly]
    ///  bool ddd = true;
    /// </code>
    /// </summary>
    public sealed class ShowOnlyAttribute : PropertyAttribute
    {
    }
}