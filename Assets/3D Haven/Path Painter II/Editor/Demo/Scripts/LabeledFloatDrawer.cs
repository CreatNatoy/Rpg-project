using UnityEngine;
using UnityEditor;

namespace Haven.Demo
{
    /// <summary>
    /// Property drawer for PaintPreset
    /// </summary>
    [CustomPropertyDrawer(typeof(LabeledFloat))]
    public class LabeledFloatDrawer : PropertyDrawer
    {
        /// <summary>
        /// OnGUI override
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float floatsWidth = position.width * 0.3f;
            float labelWidth = position.width - floatsWidth;
            float spacing = 2f;
            Rect labRect = position;
            labRect.width = labelWidth;
            Rect valRect = position;
            valRect.xMin = labRect.xMax + spacing;

            SerializedProperty lab = property.FindPropertyRelative("Label");
            SerializedProperty val = property.FindPropertyRelative("Value");
            EditorGUI.PropertyField(labRect, lab, GUIContent.none);
            EditorGUI.PropertyField(valRect, val, GUIContent.none);

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        /// <summary>
        /// GetPropertyHeight override
        /// </summary>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}
