#if UNITY_EDITOR

// public interface IMotion
// {
//     void StartMotion();
//     void StopMotion();
// }



// [CustomPropertyDrawer(typeof(ScriptableObject), true)]
// public class InterfaceValidationDrawer : PropertyDrawer
// {
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {
//         EditorGUI.BeginProperty(position, label, property);

//         var obj = property.objectReferenceValue;

//         // Ensure the assigned object implements the desired interface
//         if (obj != null && !(obj is IMotion))
//         {
//             property.objectReferenceValue = null;
//             Debug.LogError("Assigned object must implement the IMotion interface.");
//         }

//         EditorGUI.PropertyField(position, property, label);

//         EditorGUI.EndProperty();
//     }
// }

#endif
