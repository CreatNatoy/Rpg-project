using UnityEngine;
using UnityEditor;

namespace Haven.Demo
{
    /// <summary>
    /// Property drawer for PaintPreset
    /// </summary>
    [CustomPropertyDrawer(typeof(PaintPreset))]
    public class PaintPresetDrawer : PropertyDrawer
    {
        /// <summary>
        /// OnGUI override
        /// </summary>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            float extend = 60f;
            position.x -= extend;
            position.width += extend;

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float floatsWidth = position.width * 0.3f;
            float layersWidth = position.width - floatsWidth;
            float spacing = 1f;
            float digitWidth = 10f;
            Rect pSizeRect = position;
            pSizeRect.width = (floatsWidth - 3.2f * (digitWidth + spacing)) * 0.5f - spacing;
            Rect eSizeRect = pSizeRect;
            eSizeRect.x += pSizeRect.width + spacing;
            Rect slRect = eSizeRect;
            slRect.x += eSizeRect.width + spacing;
            slRect.width = 2f * digitWidth;
            Rect rampRect = slRect;
            rampRect.x += slRect.width + spacing;
            rampRect.width = 1.2f * digitWidth;
            Rect pLayerRect = position;
            pLayerRect.x += floatsWidth;
            pLayerRect.width = layersWidth * 0.5f - spacing;
            Rect eLayerRect = pLayerRect;
            eLayerRect.x += pLayerRect.width + 2 * spacing;

            SerializedProperty pSize = property.FindPropertyRelative("PathSize");
            SerializedProperty eSize = property.FindPropertyRelative("EmbankmentSize");
            SerializedProperty sl = property.FindPropertyRelative("SlopeLimit");
            SerializedProperty ramp = property.FindPropertyRelative("Ramp");
            SerializedProperty pLayer = property.FindPropertyRelative("PathLayer");
            SerializedProperty eLayer = property.FindPropertyRelative("EmbankmentLayer");

            EditorGUI.BeginChangeCheck();
            {
                EditorGUI.PropertyField(pSizeRect, pSize, GUIContent.none);
                EditorGUI.PropertyField(eSizeRect, eSize, GUIContent.none);
                EditorGUI.PropertyField(slRect, sl, GUIContent.none);
                EditorGUI.PropertyField(rampRect, ramp, GUIContent.none);
                EditorGUI.PropertyField(pLayerRect, pLayer, GUIContent.none);
                EditorGUI.PropertyField(eLayerRect, eLayer, GUIContent.none);
            }
            if (EditorGUI.EndChangeCheck())
            {
                // Make sure the values make sense
                pSize.floatValue = Mathf.Clamp(pSize.floatValue, 0.1f, 200f);
                eSize.floatValue = Mathf.Clamp(eSize.floatValue, pSize.floatValue, 200f);
                sl.floatValue = Mathf.Clamp(sl.floatValue, 0f, 90f);
                ramp.floatValue = Mathf.Clamp01(ramp.floatValue);
            }

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
