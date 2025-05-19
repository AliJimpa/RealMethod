using UnityEngine;

namespace RealMethod
{
    public struct TransformData
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public TransformData(Vector3 pos, Quaternion rot, Vector3 scl)
        {
            position = pos;
            rotation = rot;
            scale = scl;
        }

        public static TransformData FromTransform(Transform t)
        {
            return new TransformData(t.position, t.rotation, t.localScale);
        }

        public void ApplyTo(Transform t)
        {
            t.position = position;
            t.rotation = rotation;
            t.localScale = scale;
        }

        public static TransformData Lerp(TransformData a, TransformData b, float t)
        {
            return new TransformData(
                Vector3.Lerp(a.position, b.position, t),
                Quaternion.Slerp(a.rotation, b.rotation, t),
                Vector3.Lerp(a.scale, b.scale, t)
            );
        }

        public static TransformData operator +(TransformData a, TransformData b)
        {
            return new TransformData(
                a.position + b.position,
                a.rotation * b.rotation,
                Vector3.Scale(a.scale, b.scale)
            );
        }

        public static TransformData operator -(TransformData a, TransformData b)
        {
            return new TransformData(
                a.position - b.position,
                Quaternion.Inverse(b.rotation) * a.rotation,
                Vector3.Scale(a.scale, new Vector3(1 / b.scale.x, 1 / b.scale.y, 1 / b.scale.z))
            );
        }

        public static TransformData operator *(TransformData a, float b)
        {
            return new TransformData(
                a.position * b,
                Quaternion.Slerp(Quaternion.identity, a.rotation, b),
                a.scale * b
            );
        }

        public static TransformData operator /(TransformData a, float b)
        {
            return new TransformData(
                a.position / b,
                Quaternion.Slerp(Quaternion.identity, a.rotation, 1 / b),
                a.scale / b
            );
        }

        public static bool operator ==(TransformData a, TransformData b)
        {
            return a.position == b.position && a.rotation == b.rotation && a.scale == b.scale;
        }

        public static bool operator !=(TransformData a, TransformData b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj is TransformData other)
            {
                return this == other;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return position.GetHashCode() ^ rotation.GetHashCode() ^ scale.GetHashCode();
        }

        public override string ToString()
        {
            return $"Position: {position}, Rotation: {rotation}, Scale: {scale}";
        }

        public static TransformData Identity
        {
            get { return new TransformData(Vector3.zero, Quaternion.identity, Vector3.one); }
        }

        public static TransformData Zero
        {
            get { return new TransformData(Vector3.zero, Quaternion.identity, Vector3.zero); }
        }
        public static TransformData One
        {
            get { return new TransformData(Vector3.one, Quaternion.identity, Vector3.one); }
        }
        public static TransformData Up
        {
            get { return new TransformData(Vector3.up, Quaternion.identity, Vector3.one); }
        }
        public static TransformData Down
        {
            get { return new TransformData(Vector3.down, Quaternion.identity, Vector3.one); }
        }
        public static TransformData Left
        {
            get { return new TransformData(Vector3.left, Quaternion.identity, Vector3.one); }
        }
        public static TransformData Right
        {
            get { return new TransformData(Vector3.right, Quaternion.identity, Vector3.one); }
        }
        public static TransformData Forward
        {
            get { return new TransformData(Vector3.forward, Quaternion.identity, Vector3.one); }
        }
        public static TransformData Backward
        {
            get { return new TransformData(Vector3.back, Quaternion.identity, Vector3.one); }

        }
        public static TransformData Scale(Vector3 scale)
        {
            return new TransformData(Vector3.zero, Quaternion.identity, scale);
        }
        public static TransformData Translate(Vector3 translation)
        {
            return new TransformData(translation, Quaternion.identity, Vector3.one);
        }
        public static TransformData Rotate(Quaternion rotation)
        {
            return new TransformData(Vector3.zero, rotation, Vector3.one);
        }
        public static TransformData LookAt(Vector3 target, Vector3 up = default)
        {
            if (up == default) up = Vector3.up;
            Quaternion rotation = Quaternion.LookRotation(target, up);
            return new TransformData(Vector3.zero, rotation, Vector3.one);
        }
        public static TransformData Scale(float scale)
        {
            return new TransformData(Vector3.zero, Quaternion.identity, new Vector3(scale, scale, scale));
        }
        public static TransformData Scale(float x, float y, float z)
        {
            return new TransformData(Vector3.zero, Quaternion.identity, new Vector3(x, y, z));
        }
        public static TransformData Scale(float x, float y, float z, float w)
        {
            return new TransformData(Vector3.zero, Quaternion.identity, new Vector3(x, y, z) * w);
        }
        public static TransformData Scale(TransformData a, TransformData b)
        {
            return new TransformData(
                Vector3.Scale(a.position, b.position),
                Quaternion.Slerp(a.rotation, b.rotation, 0.5f),
                Vector3.Scale(a.scale, b.scale)
            );
        }
        public static TransformData Scale(TransformData a, float b)
        {
            return new TransformData(
                a.position * b,
                Quaternion.Slerp(a.rotation, Quaternion.identity, b),
                a.scale * b
            );
        }
        public static TransformData Scale(TransformData a, Vector3 b)
        {
            return new TransformData(
                Vector3.Scale(a.position, b),
                Quaternion.Slerp(a.rotation, Quaternion.identity, 0.5f),
                Vector3.Scale(a.scale, b)
            );
        }
        public static TransformData Scale(TransformData a, Quaternion b)
        {
            return new TransformData(
                Vector3.Scale(a.position, b.eulerAngles),
                Quaternion.Slerp(a.rotation, b, 0.5f),
                Vector3.Scale(a.scale, new Vector3(b.x, b.y, b.z))
            );
        }
        public static TransformData Scale(TransformData a, TransformData b, float t)
        {
            return new TransformData(
                Vector3.Lerp(a.position, b.position, t),
                Quaternion.Slerp(a.rotation, b.rotation, t),
                Vector3.Lerp(a.scale, b.scale, t)
            );
        }
        public static TransformData Scale(TransformData a, TransformData b, Vector3 t)
        {
            return new TransformData(
                Vector3.Scale(a.position, b.position),
                Quaternion.Slerp(a.rotation, b.rotation, 0.5f),
                Vector3.Scale(a.scale, t)
            );
        }
        public static TransformData Scale(TransformData a, TransformData b, Quaternion t)
        {
            return new TransformData(
                Vector3.Scale(a.position, b.position),
                Quaternion.Slerp(a.rotation, b.rotation, 0.5f),
                Vector3.Scale(a.scale, new Vector3(t.x, t.y, t.z))
            );
        }
        public static TransformData Scale(TransformData a, TransformData b, float t, Vector3 v)
        {
            return new TransformData(
                Vector3.Lerp(a.position, b.position, t),
                Quaternion.Slerp(a.rotation, b.rotation, t),
                Vector3.Scale(a.scale, v)
            );
        }
        public static TransformData Scale(TransformData a, TransformData b, float t, Quaternion q)
        {
            return new TransformData(
                Vector3.Lerp(a.position, b.position, t),
                Quaternion.Slerp(a.rotation, b.rotation, t),
                Vector3.Scale(a.scale, new Vector3(q.x, q.y, q.z))
            );
        }
        

    }
}


