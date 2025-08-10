using UnityEngine;

namespace RealMethod
{
    public static class Transform_Extension
    {
        public static void SetTransform(this Transform trans, Transform newtransform)
        {
            trans.position = newtransform.position;
            trans.rotation = newtransform.rotation;
            trans.localScale = newtransform.localScale;
        }
        public static Transform FindSocket(this Transform trans, string socketname)
        {
            foreach (var item in trans.GetComponentsInChildren<Transform>())
            {
                if (item.gameObject.name == socketname)
                {
                    return item;
                }
            }
            Debug.LogError($"Not find any socket with {socketname} name");
            return trans;
        }
    }
}