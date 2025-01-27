using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Newtonsoft.Json;

public class DemoObject : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void SendMessageToWeb(string message);

    [Tooltip("The name of the specific component that will be actioned on for this demo.")]
    [SerializeField]
    private string _componentName;
    public string ComponentName => _componentName;

    [SerializeField]
    private GameObject _targetComponent;

    private void Start()
    {
        GenerateDemoInfoMessage();
    }

    /// <summary>
    /// Gets the specific component that is targeted for this demo.
    /// </summary>
    /// <returns></returns>
    public Component GetDemoComponent()
    {
        if (_targetComponent != null)
        {
            return _targetComponent.gameObject.GetComponent(_componentName);
        }

        var component = this.GetComponent(_componentName);
        if (component == null)
        {
            Debug.LogError($"Component {_componentName} not attached to {this.gameObject.name}.");
            return null;
        }

        return component;
    }

    private void GenerateDemoInfoMessage()
    {
        var component = GetDemoComponent();
        if (component == null)
        {
            return;
        }

        var unityMessage = new UnityMessage();

        var controls = new List<AbstractInputControl>();
        var inputs = GetComponent<DemoInputs>();
        if (inputs != null)
        {
            controls.AddRange(inputs.GetControls());
        }

        if (component is MonoBehaviour && ReflectionHelper.HasResetStateMethod(component))
        {
            controls.Add(new ButtonInputControl()
            {
                description = "Space: Reset",
                method = "ResetState",
                buttonName = "Reset",
            });
        }

        unityMessage.inputControls = controls.ToArray();
        var jsonMessage = JsonConvert.SerializeObject(unityMessage);

#if UNITY_WEBGL && !UNITY_EDITOR
        SendMessageToWeb(jsonMessage);
# elif UNITY_EDITOR
        Debug.Log(jsonMessage);
#endif
    }
}
