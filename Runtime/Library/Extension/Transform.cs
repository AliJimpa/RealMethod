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
        public static Transform GetSocket(this Transform trans, Name16 socketname)
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
        /// <summary>
        /// Reset transform quickly
        /// </summary>
        /// <param name="t">Target terasform</param>
        public static void Reset(this Transform t)
        {
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
        }
        public static TransformData ConvertToData(this Transform trans)
        {
            return new TransformData(trans);
        }
    }
}