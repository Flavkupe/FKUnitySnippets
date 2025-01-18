

using System.Reflection;
using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(ReflectionHelper))]
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
            if (description != null)
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
            var keyPair = inputOption.keyPair;
            if (IsKeyDown(keyPair))
            {
                var inverse = IsInverseKey(keyPair);
                PerformAction(inputOption, inverse);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            var resettable = GetComponent<ICanReset>();
            resettable?.Reset();
        }
    }

    private bool IsInverseKey(InputOption.KeyPair keyPair)
    {
        switch (keyPair)
        {
            case InputOption.KeyPair.UpDown:
                return Input.GetKeyDown(KeyCode.DownArrow);
            case InputOption.KeyPair.LeftRight:
                return Input.GetKeyDown(KeyCode.LeftArrow);
            case InputOption.KeyPair.QW:
                return Input.GetKeyDown(KeyCode.Q);
            case InputOption.KeyPair.AS:
                return Input.GetKeyDown(KeyCode.A);
            case InputOption.KeyPair.ZX:
                return Input.GetKeyDown(KeyCode.Z);
            default:
                return false;
        }
    }

    private bool IsKeyDown(InputOption.KeyPair keyPair)
    {
        switch (keyPair)
        {
            case InputOption.KeyPair.UpDown:
                return Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow);
            case InputOption.KeyPair.LeftRight:
                return Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
            case InputOption.KeyPair.QW:
                return Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W);
            case InputOption.KeyPair.AS:
                return Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S);
            case InputOption.KeyPair.ZX:
                return Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X);
            default:
                return false;
        }
    }

    private void PerformAction(InputOption inputOption, bool inverse)
    {
        var demoObject = GetComponent<DemoObject>();
        var target = GetComponent(demoObject.ComponentName);
        if (target == null)
        {
            return;
        }

        var field = target.GetType().GetField(inputOption.selectedField, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field == null)
        {
            return;
        }

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
                break;
        }
    }
}