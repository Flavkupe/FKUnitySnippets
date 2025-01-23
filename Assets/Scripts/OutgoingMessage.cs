using JetBrains.Annotations;
using System;
using UnityEngine;

[Serializable]
public class SerializableVector3
{
    [SerializeField]
    public float x;

    [SerializeField]
    public float y;

    [SerializeField]
    public float z;

    public SerializableVector3()
    {
    }

    public SerializableVector3(Vector3 vector3)
    {
        x = vector3.x;
        y = vector3.y;
        z = vector3.z;
    }

    public static implicit operator SerializableVector3(Vector3 vector3)
    {
        return new SerializableVector3(vector3);
    }
}

[Serializable]
public class OutgoingMessage
{
    [SerializeField]
    public AbstractInputControl[] InputControls;
}

[Serializable]
public abstract class AbstractInputControl
{
    [SerializeField]
    public string inputType;

    [SerializeField]
    public string description;
}

[Serializable]
public abstract class AbstractFieldInputControl : AbstractInputControl
{
    public AbstractFieldInputControl(string fieldType)
    {
        this.fieldType = fieldType;
        this.inputType = "field";
    }

    [SerializeField]
    public string fieldType;

    [SerializeField]
    public string fieldName;
}

/// <summary>
/// This one only has a description
/// </summary>
[Serializable]
public class InfoInputControl : AbstractInputControl
{
    public InfoInputControl()
    {
        this.inputType = "info";
    }
}

[Serializable]
public class ButtonInputControl : AbstractInputControl
{
    public ButtonInputControl()
    {
        this.inputType = "button";
    }

    [SerializeField]
    public string method;

    [SerializeField]
    public string buttonName;
}

[Serializable]
public class FloatInputControl : AbstractFieldInputControl
{
    public FloatInputControl() : base("float") {}

    /// <summary>
    /// Initial value
    /// </summary>
    [SerializeField]
    public float value;

    /// <summary>
    /// How much to add/remove per tick
    /// </summary>
    [SerializeField]
    public float incrementValue;
}

[Serializable]
public class Vector3InputControl : AbstractFieldInputControl
{
    public Vector3InputControl() : base("vector3") { }

    /// <summary>
    /// Initial value
    /// </summary>
    [SerializeField]
    public SerializableVector3 value;

    /// <summary>
    /// How much to add/remove per tick
    /// </summary>
    [SerializeField]
    public float incrementValue;
}

[Serializable]
public class BoolInputControl : AbstractFieldInputControl
{
    public BoolInputControl() : base("bool") { }

    /// <summary>
    /// Initial value
    /// </summary>
    [SerializeField]
    public bool value;
}
