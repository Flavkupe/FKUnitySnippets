using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InputOption))]
public class InputOptionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var obj = property.serializedObject.targetObject as MonoBehaviour;

        var reflectionHelper = obj.GetComponent<ReflectionHelper>();

        SerializedProperty keyProp = property.FindPropertyRelative("keyPair");
        SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");
        SerializedProperty fieldTypeProp = property.FindPropertyRelative("fieldType");
        SerializedProperty selectedFieldProp = property.FindPropertyRelative("selectedField");
        SerializedProperty valueProp = property.FindPropertyRelative("value");
        SerializedProperty vectorValueProp = property.FindPropertyRelative("vectorValue");
        SerializedProperty valuesProp = property.FindPropertyRelative("values");
        SerializedProperty vectorValuesProp = property.FindPropertyRelative("vectorValues");

        var rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);

        // Draw the key field
        EditorGUI.PropertyField(rect, keyProp);
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        // Draw the effect type field
        EditorGUI.PropertyField(rect, effectTypeProp);
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        // Draw the field type field
        EditorGUI.PropertyField(rect, fieldTypeProp);
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        // Draw the dropdown for serialized fields
        if (reflectionHelper != null)
        {
            List<string> fieldNames = reflectionHelper.GetSerializedFieldNames();

            if (fieldNames.Count > 0)
            {
                var selectedIndex = Mathf.Max(0, fieldNames.IndexOf(selectedFieldProp.stringValue));
                
                selectedIndex = EditorGUI.Popup(rect, "Select Field", selectedIndex, fieldNames.ToArray());
                selectedFieldProp.stringValue = fieldNames[selectedIndex];
            }
            else
            {
                EditorGUI.LabelField(rect, "No serialized fields found.");
            }
        }
        else
        {
            EditorGUI.LabelField(rect, "No ReflectionHelper is available on this object; cannot determine fields.");
        }
        rect.y += EditorGUIUtility.singleLineHeight + 2;

        if ((InputOption.FieldType)fieldTypeProp.enumValueIndex == InputOption.FieldType.Float)
        {
            if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.AddValueToField)
            {
                EditorGUI.PropertyField(rect, valueProp);
            }
            else if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.ToggleBetweenValues)
            {
                EditorGUI.PropertyField(rect, valuesProp, true);
            }
        }
        else if ((InputOption.FieldType)fieldTypeProp.enumValueIndex == InputOption.FieldType.Vector3)
        {
            if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.AddValueToField)
            {
                EditorGUI.PropertyField(rect, vectorValueProp);
            }
            else if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.ToggleBetweenValues)
            {
                EditorGUI.PropertyField(rect, vectorValuesProp, true);
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");
        SerializedProperty fieldTypeProp = property.FindPropertyRelative("fieldType");

        float height = EditorGUIUtility.singleLineHeight * 6 + 10;

        if ((InputOption.FieldType)fieldTypeProp.enumValueIndex == InputOption.FieldType.Float &&
            (InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.ToggleBetweenValues)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("values"), true);
        }
        else if ((InputOption.FieldType)fieldTypeProp.enumValueIndex == InputOption.FieldType.Vector3 &&
                 (InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.ToggleBetweenValues)
        {
            height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("vectorValues"), true);
        }

        return height;
    }
}