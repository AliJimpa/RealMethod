using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif


namespace RealMethod
{

    /// <summary>
    /// Lightweight engine-style string identifier.
    /// Stores only an integer ID while the actual strings live in a global table.
    /// </summary>
    [Serializable]
    public struct TName : IEquatable<TName>
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
#if UNITY_EDITOR
        private static bool editorInitialized = false;
        private static ProjectSettingAsset _settings;
        private static ProjectSettingAsset ProjectSettings
        {
            get
            {
                if (editorInitialized && _settings == null && !Application.isPlaying)
                {
                    _settings = AssetDatabase.LoadAssetAtPath<ProjectSettingAsset>("Assets/RealMethod/RealMethodSetting.asset");
                }
                return _settings;
            }
        }
#endif


        /// <summary>
        /// Creates a Name from a string.
        /// </summary>
        public TName(string value)
        {
            id = GetOrCreateId(value);
        }


        ////////// Static Functions
        /// <summary>
        /// Called at runtime during game load projectsetting
        /// The projectsetting after load call these function with self refrence 
        /// </summary>
        /// <param name="setting">ProjectSetting instance loaded at runtime</param>
        public static void OnProjectSettingLoaded(ProjectSettingAsset setting)
        {
            if (setting == null)
            {
                Debug.LogError("FName Initialize failed: ProjectSettingAsset is null.");
                return;
            }

            lock (tableLock)
            {
                idToName.Clear();
                hashToIds.Clear();

                var names = setting.Names; // assume List<string> Names in the asset

                if (names == null)
                    return;

                for (int i = 0; i < names.Count; i++)
                {
                    string value = names[i];

                    if (string.IsNullOrEmpty(value))
                        continue;

                    int id = idToName.Count;
                    idToName.Add(value);

                    int hash = value.GetHashCode();

                    if (!hashToIds.TryGetValue(hash, out var bucket))
                    {
                        bucket = new List<int>(2);
                        hashToIds.Add(hash, bucket);
                    }

                    bucket.Add(id);
                }
            }
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

#if UNITY_EDITOR
                if (ProjectSettings != null)
                {
                    ProjectSettings.Names = idToName;
                }
                else
                {
                    Debug.LogWarning("ProjectSettingAsset'NamesTable can't update, ProjectSettingAsset is not valid!");
                }
#endif
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



        ////////// Functions 
        /// <summary>
        /// Checks equality with another Name.
        /// </summary>
        public bool Equals(TName other)
        {
            return id == other.id;
        }




        ////////// Overrides 
        /// <summary>
        /// Converts Name to string.
        /// </summary>
        public override string ToString()
        {
            return GetString(id);
        }
        /// <summary>
        /// Checks equality with an object.
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is TName other && Equals(other);
        }
        /// <summary>
        /// Hash code of the Name.
        /// </summary>
        public override int GetHashCode()
        {
            return id;
        }



        ////////// Operators 
        /// <summary>
        /// Implicit conversion from string.
        /// </summary>
        public static implicit operator TName(string value)
        {
            return new TName(value);
        }

        /// <summary>
        /// Implicit conversion to string.
        /// </summary>
        public static implicit operator string(TName name)
        {
            return GetString(name.id);
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        public static bool operator ==(TName a, TName b)
        {
            return a.id == b.id;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        public static bool operator !=(TName a, TName b)
        {
            return a.id != b.id;
        }




#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void MarkEditorReady()
        {
            editorInitialized = true;
            _settings = Resources.Load<ProjectSettingAsset>("RealMethod/RealMethodSetting");
        }
        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            editorInitialized = true;

            if (ProjectSettings == null)
            {
                Debug.LogWarning("Static NamesTable can't update, ProjectSettingAsset is not valid!");
                return;
            }

            lock (tableLock)
            {
                idToName.Clear();
                hashToIds.Clear();

                var names = ProjectSettings.Names; // assume List<string> Names in the asset

                if (names == null)
                    return;

                for (int i = 0; i < names.Count; i++)
                {
                    string value = names[i];

                    if (string.IsNullOrEmpty(value))
                        continue;

                    int id = idToName.Count;
                    idToName.Add(value);

                    int hash = value.GetHashCode();

                    if (!hashToIds.TryGetValue(hash, out var bucket))
                    {
                        bucket = new List<int>(2);
                        hashToIds.Add(hash, bucket);
                    }

                    bucket.Add(id);
                }
            }
        }
#endif
    }

}