using System.Reflection;
using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DemoObject))]
public class DemoInputs : MonoBehaviour
{
    [SerializeField]
    private InputOption[] _inputOptions;

    public List<string> GetDescriptions()
    {
        var descriptions = new List<string>();
        foreach (var option in _inputOptions)
        {
            var description = option.GetDescription();
            if (!string.IsNullOrWhiteSpace(description))
            {
                descriptions.Add(description);
            }
        }

        return descriptions;
    }

    private void Update()
    {
        foreach (var inputOption in _inputOptions)
        {
            if (IsTriggerActivated(inputOption))
            {
                PerformAction(inputOption);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var demoObject = GetComponent<DemoObject>();
            var demoComponent = demoObject.GetDemoComponent() as MonoBehaviour;
            if (demoComponent != null && ReflectionHelper.HasResetStateMethod(demoComponent))
            {
                demoComponent.Invoke("ResetState", 0f);
            }
        }
    }

    private bool IsInverseKey(InputOption.Trigger keyPair)
    {
        switch (keyPair)
        {
            case InputOption.Trigger.UpDown:
                return Input.GetKeyDown(KeyCode.DownArrow);
            case InputOption.Trigger.LeftRight:
                return Input.GetKeyDown(KeyCode.LeftArrow);
            case InputOption.Trigger.QW:
                return Input.GetKeyDown(KeyCode.Q);
            case InputOption.Trigger.AS:
                return Input.GetKeyDown(KeyCode.A);
            case InputOption.Trigger.ZX:
                return Input.GetKeyDown(KeyCode.Z);
            default:
                return false;
        }
    }

    private bool IsTriggerActivated(InputOption inputOption)
    {
        var trigger = inputOption.trigger;
        switch (trigger)
        {
            case InputOption.Trigger.UpDown:
                return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow);
            case InputOption.Trigger.LeftRight:
                return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
            case InputOption.Trigger.QW:
                return Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W);
            case InputOption.Trigger.AS:
                return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S);
            case InputOption.Trigger.ZX:
                return Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X);
            case InputOption.Trigger.SpecificKey:
                return Input.GetKeyDown(inputOption.specificKey);
            case InputOption.Trigger.MouseClick:
                return Input.GetMouseButtonDown(0);
            case InputOption.Trigger.MouseRightClick:
                return Input.GetMouseButtonDown(1);
            default:
                return false;
        }
    }

    private void PerformAction(InputOption inputOption)
    {
        var demoObject = GetComponent<DemoObject>();
        var target = GetComponent(demoObject.ComponentName) as MonoBehaviour;
        if (target == null)
        {
            return;
        }

        var field = target.GetType().GetField(inputOption.selectedField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field == null)
        {
            return;
        }

        if (inputOption.IsMouseTrigger)
        {
            PerformMouseActions(inputOption, target, field);
        }
        else
        {
            PerformKeyboardActions(inputOption, target, field);
        }
    }

    private void PerformMouseActions(InputOption inputOption, MonoBehaviour target, FieldInfo field)
    {
        var mousePosition = Input.mousePosition;
        var worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0;
        switch (inputOption.clickEffectType)
        {
            case InputOption.ClickEffectType.SetVector3ValueToPointer:
                field.SetValue(target, worldPosition);
                break;
            case InputOption.ClickEffectType.MoveObjectToPointer:
                inputOption.targetObject.transform.position = worldPosition;
                break;
            default:
                return;
        }
    }

    private void PerformKeyboardActions(InputOption inputOption, MonoBehaviour target, FieldInfo field)
    {
        var inverse = IsInverseKey(inputOption.trigger);
        switch (inputOption.effectType)
        {
            case InputOption.EffectType.AddValueToField:
                if (inputOption.fieldType == InputOption.FieldType.Float)
                {
                    float currentValue = (float)field.GetValue(target);
                    var value = inverse ? -inputOption.value : inputOption.value;
                    field.SetValue(target, currentValue + value);
                }
                else if (inputOption.fieldType == InputOption.FieldType.Vector3)
                {
                    Vector3 currentValue = (Vector3)field.GetValue(target);
                    var value = inverse ? -inputOption.vectorValue : inputOption.vectorValue;
                    field.SetValue(target, currentValue + value);
                }
                break;

            case InputOption.EffectType.ToggleBetweenValues:
                if (inputOption.fieldType == InputOption.FieldType.Float)
                {
                    float currentValue = (float)field.GetValue(target);
                    int index = Array.IndexOf(inputOption.values, currentValue);
                    index = (index + 1) % inputOption.values.Length;
                    field.SetValue(target, inputOption.values[index]);
                }
                else if (inputOption.fieldType == InputOption.FieldType.Vector3)
                {
                    Vector3 currentValue = (Vector3)field.GetValue(target);
                    int index = Array.IndexOf(inputOption.vectorValues, currentValue);
                    index = (index + 1) % inputOption.vectorValues.Length;
                    field.SetValue(target, inputOption.vectorValues[index]);
                }
                else if (inputOption.fieldType == InputOption.FieldType.Bool)
                {
                    bool currentValue = (bool)field.GetValue(target);
                    field.SetValue(target, !currentValue);
                }

                break;

            case InputOption.EffectType.CallsMethod:
                if (inputOption.methodToInvoke != null)
                {
                    target.Invoke(inputOption.methodToInvoke, 0.0f);
                }
                break;
        }
    }
}