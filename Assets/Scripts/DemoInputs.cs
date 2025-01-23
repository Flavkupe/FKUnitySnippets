using System.Reflection;
using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(DemoObject))]
public class DemoInputs : MonoBehaviour
{
    [SerializeField]
    private InputOption[] _inputOptions;

    public List<AbstractInputControl> GetControls()
    {
        var target = Target;
        var controls = new List<AbstractInputControl>();
        foreach (var option in _inputOptions)
        {
            var control = option.ToWebMessageInputControl(target);
            if (control != null)
            {
                controls.Add(control);
            }
        }

        return controls;
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
    }

    private bool IsTriggerActivated(InputOption inputOption)
    {
        var trigger = inputOption.trigger;
        switch (trigger)
        {
            case InputOption.Trigger.MouseClick:
                return Input.GetMouseButtonDown(0);
            case InputOption.Trigger.MouseRightClick:
                return Input.GetMouseButtonDown(1);
            default:
                return false;
        }
    }

    private MonoBehaviour Target => GetComponent<DemoObject>()?.GetDemoComponent() as MonoBehaviour;

    private FieldInfo GetField(string fieldName)
    {
        var target = Target;
        if (target == null)
        {
            return null;
        }

        return ReflectionHelper.GetField(target, fieldName);
    }

    private void PerformAction(InputOption inputOption)
    {
        var target = Target;
        if (target == null)
        {
            return;
        }

        var field = GetField(inputOption.selectedField);
        if (field == null)
        {
            return;
        }

        if (inputOption.IsMouseTrigger)
        {
            PerformMouseActions(inputOption, target, field);
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

    public void SetFieldValue(SetValueMessage message)
    {
        var field = GetField(message.FieldName);
        if (field == null)
        {
            return;
        }

        field.SetValue(Target, message.FieldValue);
    }

    public void InvokeMethodOnTarget(string methodName)
    {
        var target = Target;
        if (target == null)
        {
            return;
        }

        target.Invoke(methodName, 0);
    }

}
