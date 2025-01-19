using System;
using UnityEngine;

[Serializable]
public class InputOption
{
    public enum Trigger
    {
        UpDown,
        LeftRight,
        QW,
        AS,
        ZX,
        SpecificKey,
        MouseClick,
        MouseRightClick,
    }

    public enum EffectType
    {
        AddValueToField,
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

    public KeyCode specificKey;

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
            return GetKeypressDescription();
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

    private string GetKeypressDescription()
    {
        string keyDescription = trigger switch
        {
            Trigger.UpDown => "Up/Down",
            Trigger.LeftRight => "Left/Right",
            Trigger.QW => "Q/W",
            Trigger.AS => "A/S",
            Trigger.ZX => "Z/X",
            Trigger.SpecificKey => specificKey.ToString(),
            _ => null
        };

        string effectDescription = effectType switch
        {
            EffectType.AddValueToField => "Increase/Decrease value",
            EffectType.ToggleBetweenValues => "Switch between values",
            _ => null
        };

        if (keyDescription == null || effectDescription == null)
        {
            return null;
        }

        return $"Press {keyDescription}: {effectDescription} of {selectedField}";
    }
}
