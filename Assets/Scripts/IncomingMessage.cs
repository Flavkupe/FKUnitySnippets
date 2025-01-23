using System;
using UnityEngine;

public abstract class SetValueMessage
{
    public string FieldName;

    public abstract object FieldValue { get; }
}

[Serializable]
public class SetFloatValueMessage : SetValueMessage
{
    public float Value;

    public override object FieldValue => Value;
    
}

[Serializable]
public class SetBoolValueMessage : SetValueMessage
{
    public bool Value;

    public override object FieldValue => Value;
}

[Serializable]
public class SetVector3ValueMessage : SetValueMessage
{
    public Vector3 Value;

    public override object FieldValue => Value;
}

[Serializable]
public class InvokeMethodMessage
{
    public string MethodName;
}
