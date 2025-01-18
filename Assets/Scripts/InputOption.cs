using System;
using UnityEngine;

[Serializable]
public class InputOption
{
    public enum KeyPair
    {
        UpDown,
        LeftRight,
        QW,
        AS,
        ZX,
    }

    public enum EffectType
    {
        AddValueToField,
        ToggleBetweenValues,
    }

    public enum FieldType
    {
        Float,
        Vector3,
    }

    public EffectType effectType;

    public FieldType fieldType;

    public KeyPair keyPair;

    public string selectedField;

    public float value;

    public Vector3 vectorValue;

    public float[] values;

    public Vector3[] vectorValues;

    public string GetDescription()
    {
        string keyDescription = keyPair switch
        {
            KeyPair.UpDown => "Up/Down",
            KeyPair.LeftRight => "Left/Right",
            KeyPair.QW => "Q/W",
            KeyPair.AS => "A/S",
            KeyPair.ZX => "Z/X",
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
