using System;
using UnityEngine;

[Serializable]
public class InputOption
{
    public enum Trigger
    {
        /// <summary>
        /// Console-like controls, appearing on the DOM.
        /// </summary>
        Default,

        /// <summary>
        /// Allows control of the demo by clicking on the WebGL embed.
        /// </summary>
        MouseClick,

        /// <summary>
        /// Allows control of the demo by right-clicking on the WebGL embed.
        /// </summary>
        MouseRightClick,

        /// <summary>
        /// Used for just showing the description.
        /// </summary>
        None,
    }

    public enum EffectType
    {
        ChangeFieldValue,
        ToggleBetweenValues,
        CallsMethod,
    }

    public enum ClickEffectType
    {
        SetVector3ValueToPointer,
        MoveObjectToPointer,
    }

    public enum FieldType
    {
        Float,
        Vector3,
        Bool,
    }

    public enum DescriptionType
    {
        Default,
        None,
        Custom,
    }

    public EffectType effectType;

    public ClickEffectType clickEffectType;

    public FieldType fieldType;

    public Trigger trigger;

    public DescriptionType descriptionType = DescriptionType.Default;

    public string selectedField;

    public string methodToInvoke;

    public float value;

    public Vector3 vectorValue;

    public float[] values;

    public Vector3[] vectorValues;

    public string customDescription;

    public GameObject targetObject;

    public bool IsMouseTrigger => trigger == Trigger.MouseClick || trigger == Trigger.MouseRightClick;

    public AbstractInputControl ToWebMessageInputControl(MonoBehaviour target)
    {
        var description = GetDescription();

        if (trigger != Trigger.Default)
        {
            if (string.IsNullOrEmpty(description))
            {
                // don't even need to return this one, as it serves no purpose
                // on the web controls
                return null;
            }

            return new InfoInputControl
            {
                description = description,
            };
        }

        switch (effectType)
        {
            case EffectType.ChangeFieldValue:
                return CreateAddValueInputControl(target, description);
            case EffectType.ToggleBetweenValues:
                // TODO: add support for this!
                return null;
            case EffectType.CallsMethod:
                return new ButtonInputControl
                {
                    description = description,
                    method = methodToInvoke,
                    buttonName = methodToInvoke,
                };
            default:
                return null;
        }
    }

    private AbstractFieldInputControl CreateAddValueInputControl(MonoBehaviour target, string description)
    {
        if (fieldType == FieldType.Float)
        {
            var fieldValue = ReflectionHelper.GetFloatFieldValue(target, selectedField);
            return new FloatInputControl
            {
                description = description,
                value = fieldValue,
                incrementValue = value,
                fieldName = selectedField,
            };
        }
        else if (fieldType == FieldType.Vector3)
        {
            var fieldValue = ReflectionHelper.GetVector3FieldValue(target, selectedField);
            return new Vector3InputControl
            {
                description = description,
                value = fieldValue,
                incrementValue = value,
                fieldName = selectedField,
            };
        }
        else if (fieldType == FieldType.Bool)
        {
            var fieldValue = ReflectionHelper.GetBoolFieldValue(target, selectedField);
            return new BoolInputControl
            {
                description = description,
                value = fieldValue,
                fieldName = selectedField,
            };
        }

        return null;
    }

    public string GetDescription()
    {
        if (descriptionType == DescriptionType.Custom)
        {
            return customDescription;
        }

        if (descriptionType == DescriptionType.None)
        {
            return null;
        }

        if (this.IsMouseTrigger)
        {
            return GetMouseClickDescription();
            
        }
        else
        {
            return "Use the controls to change the values";
        }
    }

    private string GetMouseClickDescription()
    {
        var command = trigger == Trigger.MouseClick ? "Left" : "Right";

        if (clickEffectType == ClickEffectType.SetVector3ValueToPointer)
        {
            return $"{command} click to set {selectedField}";
        } else
        {
            if (targetObject == null)
            {
                return null;
            }

            return $"{command} click to move {targetObject.name}";
        }
    }
}
