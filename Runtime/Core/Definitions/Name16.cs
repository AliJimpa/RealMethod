using System;
using System.Text;
using UnityEngine;



namespace RealMethod
{
    /// <summary>
    /// String identifier.
    /// Store 16 character with very fast compairing
    /// </summary>
    [Serializable]
    public struct Name16
    {
        [SerializeField]
        public ulong Name_A;
        [SerializeField]
        public ulong Name_B;

        /// <summary>
        /// Creates a Name16 from a string.
        /// </summary>
        public Name16(string value)
        {
            if (value.Length > 16)
                throw new ArgumentException($"Name cannot exceed {16} characters.");

            Name_A = 0;
            Name_B = 0;

            if (string.IsNullOrEmpty(value))
                return;

            int len = Math.Min(16, value.Length);

            for (int i = 0; i < len; i++)
            {
                byte b = (byte)value[i];

                if (i < 8)
                    Name_A |= ((ulong)b) << (i * 8);
                else
                    Name_B |= ((ulong)b) << ((i - 8) * 8);
            }
        }


        /// <summary>
        /// Checks equality with another Name.
        /// </summary>
        public bool Equals(Name16 other)
        {
            return this == other;
        }


        /// <summary>
        /// Converts Name to string.
        /// </summary>
        public override string ToString()
        {
            Span<byte> bytes = stackalloc byte[16];

            for (int i = 0; i < 8; i++)
                bytes[i] = (byte)((Name_A >> (i * 8)) & 0xFF);

            for (int i = 0; i < 8; i++)
                bytes[i + 8] = (byte)((Name_B >> (i * 8)) & 0xFF);

            int len = 16;
            while (len > 0 && bytes[len - 1] == 0)
                len--;

            return Encoding.ASCII.GetString(bytes.Slice(0, len));
        }
        /// <summary>
        /// Checks equality with an object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is Name16 other && Equals(other);
        }
        /// <summary>
        /// Hash code of the Name.
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name_A, Name_B);
        }



        /// <summary>
        /// Implicit conversion from string.
        /// </summary>
        public static implicit operator Name16(string value)
        {
            return new Name16(value);
        }
        /// <summary>
        /// Implicit conversion to string.
        /// </summary>
        public static implicit operator string(Name16 value)
        {
            return value.ToString();
        }
        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(Name16 a, Name16 b)
        {
            return a.Name_A == b.Name_A && a.Name_B == b.Name_B;
        }
        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(Name16 a, Name16 b)
        {
            return !(a == b);
        }

    }




}




