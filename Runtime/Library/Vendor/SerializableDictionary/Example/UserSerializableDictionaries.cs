using System;
using UnityEngine;

namespace RealMethod
{
    [Serializable]
    public class StringBoolDictionary : SerializableDictionary<string, bool> { }
    [Serializable]
    public class StringByteDictionary : SerializableDictionary<string, byte> { }
    [Serializable]
    public class StringIntDictionary : SerializableDictionary<string, int> { }
    [Serializable]
    public class StringFloatDictionary : SerializableDictionary<string, float> { }
    [Serializable]
    public class StringAssetObjectDictionary : SerializableDictionary<string, PrimitiveAsset> { }
    [Serializable]
    public class StringGObjectDictionary : SerializableDictionary<string, GameObject> { }
    [Serializable]
    public class ObjectColorDictionary : SerializableDictionary<UnityEngine.Object, Color> { }

    // ColorArray
    [Serializable]
    public class ColorArrayStorage : SerializableDictionary.Storage<Color[]> { }
    [Serializable]
    public class StringColorArrayDictionary : SerializableDictionary<string, Color[], ColorArrayStorage> { }
}
