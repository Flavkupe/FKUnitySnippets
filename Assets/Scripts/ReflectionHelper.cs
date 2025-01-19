using UnityEngine;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public static class ReflectionHelper
{
    public static bool HasResetStateMethod(Component component)
    {
        return HasMethodByName(component, "ResetState");
    }

    private static bool HasMethodByName(Component component, string methodName)
    {
        Type type = component.GetType();
        MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);

        if (methodInfo != null && methodInfo.ReturnType == typeof(void))
        {
            return true;
        }

        return false;
    }



    public static List<string> GetSerializedFieldNames(DemoObject demoObject, string fieldType)
    {
        var component = demoObject.GetDemoComponent();
        if (component == null)
        {
            return new List<string>();
        }

        FieldInfo[] fields = component.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        var fieldInfoList = fields.Where(field => 
            field.FieldType.Name == fieldType &&
            Attribute.IsDefined(field, typeof(SerializeField))).ToList();

        var fieldNames = new List<string>();
        foreach (var item in fieldInfoList)
        {
            fieldNames.Add(item.Name);
        }

        return fieldNames;
    }

    /// <summary>
    /// Gets public methods with no parameters.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetPublicMethodNames(DemoObject demoObject)
    {
        var component = demoObject.GetDemoComponent();
        if (component == null)
        {
            return new List<string>();
        }

        MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);

        var methodNames = new List<string>();
        foreach (var method in methods)
        {
            // only return void methods with no parameters
            if (method.GetParameters().Length == 0 && method.ReturnType.Name == "Void")
            {
                methodNames.Add(method.Name);
            }
        }

        return methodNames;
    }

    public static void SetFieldValue(DemoObject demoObject, string fieldName, object value)
    {
        var component = demoObject.GetDemoComponent();
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
