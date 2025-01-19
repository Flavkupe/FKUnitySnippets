using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InputOption))]
public class InputOptionDrawer : PropertyDrawer
{
    private float LineHeight => EditorGUIUtility.singleLineHeight + 2;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var component = property.serializedObject.targetObject as MonoBehaviour;
        if (component == null)
        {
            EditorGUI.LabelField(position, "InputOption should only be used with MonoBehaviour components.");
            return;
        }

        var demoObject = component.GetComponent<DemoObject>();
        if (demoObject == null)
        {
            EditorGUI.LabelField(position, "InputOption should only be used with DemoObject components.");
            return;
        }

        SerializedProperty triggerProp = property.FindPropertyRelative("trigger");
        SerializedProperty specificKeyProp = property.FindPropertyRelative("specificKey");
        SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");
        SerializedProperty clickEffectTypeProp = property.FindPropertyRelative("clickEffectType");
        SerializedProperty descriptionTypeProp = property.FindPropertyRelative("descriptionType");

        var rect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);

        EditorGUI.PropertyField(rect, triggerProp);
        rect.y += LineHeight;

        if ((InputOption.Trigger)triggerProp.enumValueIndex == InputOption.Trigger.SpecificKey)
        {
            EditorGUI.PropertyField(rect, specificKeyProp);
            rect.y += LineHeight;
        }

        EditorGUI.PropertyField(rect, descriptionTypeProp);
        rect.y += LineHeight;

        if ((InputOption.DescriptionType)descriptionTypeProp.enumValueIndex == InputOption.DescriptionType.Custom)
        {
            SerializedProperty customDescriptionProp = property.FindPropertyRelative("customDescription");
            EditorGUI.PropertyField(rect, customDescriptionProp);
            rect.y += LineHeight;
        }

        if ((InputOption.Trigger)triggerProp.enumValueIndex == InputOption.Trigger.MouseClick ||
            (InputOption.Trigger)triggerProp.enumValueIndex == InputOption.Trigger.MouseRightClick)
        {
            EditorGUI.PropertyField(rect, clickEffectTypeProp);
            rect.y += LineHeight;
            DrawMouseEffectFields(rect, property, demoObject, clickEffectTypeProp);
        }
        else
        {
            EditorGUI.PropertyField(rect, effectTypeProp);
            rect.y += LineHeight;
            DrawKeyboardEffectFields(rect, property, demoObject, effectTypeProp);
        }

        rect.y += LineHeight;

        

        EditorGUI.EndProperty();
    }

    private void DrawMouseEffectFields(Rect rect, SerializedProperty property, DemoObject demoObject, SerializedProperty mouseEffectTypeProp)
    {
        
        if ((InputOption.ClickEffectType)mouseEffectTypeProp.enumValueIndex == InputOption.ClickEffectType.SetVector3ValueToPointer)
        {
            SerializedProperty selectedFieldProp = property.FindPropertyRelative("selectedField");
            DrawSelectedField(rect, demoObject, selectedFieldProp, "Vector3");
        }
        else
        {
            SerializedProperty targetObjectProp = property.FindPropertyRelative("targetObject");
            EditorGUI.ObjectField(rect, targetObjectProp);
        }
    }

    private void DrawKeyboardEffectFields(Rect rect, SerializedProperty property, DemoObject demoObject, SerializedProperty effectTypeProp)
    {
        if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.CallsMethod)
        {
            DrawMethodToInvokeField(rect, property, demoObject);
        }
        else
        {
            SerializedProperty fieldTypeProp = property.FindPropertyRelative("fieldType");
            SerializedProperty selectedFieldProp = property.FindPropertyRelative("selectedField");

            DrawFieldTypePopup(rect, fieldTypeProp, effectTypeProp);
            rect.y += LineHeight;

            var fieldTypeName = GetFieldTypeName(fieldTypeProp);
            DrawSelectedField(rect, demoObject, selectedFieldProp, fieldTypeName);
            rect.y += LineHeight;

            DrawValueFields(rect, property, effectTypeProp, fieldTypeProp);
        }
    }

    private void DrawMethodToInvokeField(Rect rect, SerializedProperty property, DemoObject demoObject)
    {
        var methodNames = ReflectionHelper.GetPublicMethodNames(demoObject);
        if (methodNames.Count > 0)
        {
            SerializedProperty methodToInvokeProp = property.FindPropertyRelative("methodToInvoke");
            var selectedIndex = Mathf.Max(0, methodNames.IndexOf(methodToInvokeProp.stringValue));
            selectedIndex = EditorGUI.Popup(rect, "Select Method", selectedIndex, methodNames.ToArray());
            methodToInvokeProp.stringValue = methodNames[selectedIndex];
        }
        else
        {
            EditorGUI.HelpBox(rect, "No public methods found.", MessageType.Error);
        }
    }

    private void DrawSelectedField(Rect rect, DemoObject demoObject, SerializedProperty selectedFieldProp, string fieldTypeName)
    {
        var fieldNames = ReflectionHelper.GetSerializedFieldNames(demoObject, fieldTypeName);

        if (fieldNames.Count > 0)
        {
            var selectedIndex = Mathf.Max(0, fieldNames.IndexOf(selectedFieldProp.stringValue));
            selectedIndex = EditorGUI.Popup(rect, "Select Field", selectedIndex, fieldNames.ToArray());
            selectedFieldProp.stringValue = fieldNames[selectedIndex];
        }
        else
        {
            EditorGUI.HelpBox(rect, "No serialized fields found.", MessageType.Error);
        }
    }

    private string GetFieldTypeName(SerializedProperty fieldTypeProp)
    {
        var valueType = (InputOption.FieldType)fieldTypeProp.enumValueIndex;
        switch (valueType) {
            case InputOption.FieldType.Bool:
                return "Boolean";
            case InputOption.FieldType.Float:
                return "Single";
            case InputOption.FieldType.Vector3:
                return "Vector3";
            default:
                return "";
        }
    }

    private void DrawValueFields(Rect rect, SerializedProperty property, SerializedProperty effectTypeProp, SerializedProperty fieldTypeProp)
    {
        if ((InputOption.FieldType)fieldTypeProp.enumValueIndex == InputOption.FieldType.Float)
        {
            DrawFloatValueFields(rect, property, effectTypeProp);
        }
        else if ((InputOption.FieldType)fieldTypeProp.enumValueIndex == InputOption.FieldType.Vector3)
        {
            DrawVector3ValueFields(rect, property, effectTypeProp);
        }
    }

    private void DrawFloatValueFields(Rect rect, SerializedProperty property, SerializedProperty effectTypeProp)
    {
        if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.AddValueToField)
        {
            SerializedProperty valueProp = property.FindPropertyRelative("value");
            EditorGUI.PropertyField(rect, valueProp);
        }
        else if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.ToggleBetweenValues)
        {
            SerializedProperty valuesProp = property.FindPropertyRelative("values");
            EditorGUI.PropertyField(rect, valuesProp, true);
        }
    }

    private void DrawVector3ValueFields(Rect rect, SerializedProperty property, SerializedProperty effectTypeProp)
    {

        if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.AddValueToField)
        {
            SerializedProperty vectorValueProp = property.FindPropertyRelative("vectorValue");
            EditorGUI.PropertyField(rect, vectorValueProp);
        }
        else if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.ToggleBetweenValues)
        {
            SerializedProperty vectorValuesProp = property.FindPropertyRelative("vectorValues");
            EditorGUI.PropertyField(rect, vectorValuesProp, true);
        }
    }

    private void DrawFieldTypePopup(Rect rect, SerializedProperty fieldTypeProp, SerializedProperty effectTypeProp)
    {
        InputOption.FieldType[] displayedOptions;

        if ((InputOption.EffectType)effectTypeProp.enumValueIndex == InputOption.EffectType.ToggleBetweenValues)
        {
            displayedOptions = new InputOption.FieldType[] { InputOption.FieldType.Float, InputOption.FieldType.Vector3, InputOption.FieldType.Bool };
        }
        else
        {
            displayedOptions = new InputOption.FieldType[] { InputOption.FieldType.Float, InputOption.FieldType.Vector3 };
        }

        int selectedIndex = Mathf.Max(0, System.Array.IndexOf(displayedOptions, (InputOption.FieldType)fieldTypeProp.enumValueIndex));
        selectedIndex = EditorGUI.Popup(rect, "Field Type", selectedIndex, System.Array.ConvertAll(displayedOptions, item => item.ToString()));
        fieldTypeProp.enumValueIndex = (int)displayedOptions[selectedIndex];
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty effectTypeProp = property.FindPropertyRelative("effectType");
        SerializedProperty fieldTypeProp = property.FindPropertyRelative("fieldType");
        SerializedProperty descriptionTypeProp = property.FindPropertyRelative("descriptionType");

        float height = EditorGUIUtility.singleLineHeight * 8 + 14;

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

        if ((InputOption.DescriptionType)descriptionTypeProp.enumValueIndex == InputOption.DescriptionType.Custom)
        {
            height += EditorGUIUtility.singleLineHeight + 2;
        }

        return height;
    }
}