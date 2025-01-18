using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;

[RequireComponent(typeof(DemoObject))]
public class ReflectionHelper : MonoBehaviour
{
    private List<FieldInfo> _fieldInfoList = new();

    public List<string> GetSerializedFieldNames()
    {
        var component = GetDemoComponent();
        if (component == null)
        {
            return new List<string>();
        }

        ListSerializedFields(component);

        var fieldNames = new List<string>();
        foreach (var item in _fieldInfoList)
        {
            fieldNames.Add(item.Name);
        }

        return fieldNames;
    }

    private Component GetDemoComponent()
    {
        var demoObject = GetComponent<DemoObject>();
        var componentName = demoObject.ComponentName;
        var component = demoObject.gameObject.GetComponent(componentName);
        if (component == null)
        {
            Debug.LogError($"Component {componentName} not attached to {demoObject.gameObject.name}.");
            return null;
        }

        return component;
    }

    private void ListSerializedFields(Component component)
    {
        _fieldInfoList.Clear();
        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            if (Attribute.IsDefined(field, typeof(SerializeField)))
            {
                _fieldInfoList.Add(field);
            }
        }
    }

    public void SetFieldValue(string fieldName, object value)
    {
        var component = GetDemoComponent();
        if (component == null)
        {
            return;
        }

        FieldInfo field = component.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (field == null || !Attribute.IsDefined(field, typeof(SerializeField)))
        {
            Debug.LogError($"Field {fieldName} not found or not serialized.");
            return;
        }

        field.SetValue(component, Convert.ChangeType(value, field.FieldType));
        Debug.Log($"Field {fieldName} set to {value}.");
    }
}