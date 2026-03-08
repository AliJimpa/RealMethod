using System;
using System.Collections.Generic;
using UnityEngine;

namespace RealMethod
{
    /// <summary>
    /// Lightweight engine-style string identifier.
    /// Stores only an integer ID while the actual strings live in a global table.
    /// </summary>
    [Serializable]
    public struct FName : IEquatable<FName>
    {
        /// <summary>
        /// Maximum allowed characters for a name.
        /// </summary>
        public const int MaxLength = 1024;

        /// <summary>
        /// Internal identifier referencing the global name table.
        /// </summary>
        [SerializeField]
        private int id;

        /// <summary>
        /// Stores strings by ID.
        /// </summary>
        private static readonly List<string> idToName = new List<string>(256);

        /// <summary>
        /// Hash → list of IDs (collision bucket).
        /// </summary>
        private static readonly Dictionary<int, List<int>> hashToIds = new Dictionary<int, List<int>>(256);

        /// <summary>
        /// Lock for thread-safe name registration.
        /// </summary>
        private static readonly object tableLock = new object();

        /// <summary>
        /// Creates a Name from a string.
        /// </summary>
        public FName(string value)
        {
            id = GetOrCreateId(value);
        }

        /// <summary>
        /// Returns the ID for a string or creates a new one.
        /// </summary>
        public static int GetOrCreateId(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Name cannot be null or empty.");

            if (value.Length > MaxLength)
                throw new ArgumentException($"Name cannot exceed {MaxLength} characters.");

            int hash = value.GetHashCode();

            lock (tableLock)
            {
                if (hashToIds.TryGetValue(hash, out var bucket))
                {
                    for (int i = 0; i < bucket.Count; i++)
                    {
                        int existingId = bucket[i];

                        if (idToName[existingId] == value)
                            return existingId;
                    }
                }
                else
                {
                    bucket = new List<int>(2);
                    hashToIds.Add(hash, bucket);
                }

                int newId = idToName.Count;

                idToName.Add(value);
                bucket.Add(newId);

                return newId;
            }
        }

        /// <summary>
        /// Returns the string for an ID.
        /// </summary>
        public static string GetString(int id)
        {
            if (id < 0 || id >= idToName.Count)
                return string.Empty;

            return idToName[id];
        }

        /// <summary>
        /// Converts Name to string.
        /// </summary>
        public override string ToString()
        {
            return GetString(id);
        }

        /// <summary>
        /// Implicit conversion from string.
        /// </summary>
        public static implicit operator FName(string value)
        {
            return new FName(value);
        }

        /// <summary>
        /// Implicit conversion to string.
        /// </summary>
        public static implicit operator string(FName name)
        {
            return GetString(name.id);
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(FName a, FName b)
        {
            return a.id == b.id;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(FName a, FName b)
        {
            return a.id != b.id;
        }

        /// <summary>
        /// Checks equality with another Name.
        /// </summary>
        public bool Equals(FName other)
        {
            return id == other.id;
        }

        /// <summary>
        /// Checks equality with an object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is FName other && Equals(other);
        }

        /// <summary>
        /// Hash code of the Name.
        /// </summary>
        public override int GetHashCode()
        {
            return id;
        }
    }
}




