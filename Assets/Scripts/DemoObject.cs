using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class DemoObject : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SendMessageToWeb(string message);

    [Tooltip("The name of the specific component that will be actioned on for this demo.")]
    [SerializeField]
    private string _componentName;
    public string ComponentName => _componentName;

    [SerializeField]
    private Component _targetComponent;

    private void Start()
    {
        GenerateDescriptionMessage();
    }

    /// <summary>
    /// Gets the specific component that is targeted for this demo.
    /// </summary>
    /// <returns></returns>
    public Component GetDemoComponent()
    {
        if (_targetComponent != null)
        {
            return _targetComponent;
        }

        var component = this.GetComponent(_componentName);
        if (component == null)
        {
            Debug.LogError($"Component {_componentName} not attached to {this.gameObject.name}.");
            return null;
        }

        return component;
    }

    private void GenerateDescriptionMessage()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        return;
#endif

        var component = GetDemoComponent();
        if (component == null)
        {
            return;
        }

        var unityMessage = new UnityMessage();

        
        var controls = new List<string>();
        if (component is MonoBehaviour && ReflectionHelper.HasResetStateMethod(component))
        {
            var resetMessage = "Space: Reset";
            controls.Add(resetMessage);
        }

        var inputs = GetComponent<DemoInputs>();
        if (inputs != null)
        {
            controls.AddRange(inputs.GetDescriptions());
        }

        unityMessage.controls = controls.ToArray();
        var jsonMessage = JsonUtility.ToJson(unityMessage);
        SendMessageToWeb(jsonMessage);
    }
}
