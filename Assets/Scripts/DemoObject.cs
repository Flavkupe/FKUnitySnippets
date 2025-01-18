using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;

public class DemoObject : MonoBehaviour
{
    public enum DemoType
    {
        GameObject,
        UI,
    }

    [DllImport("__Internal")]
    private static extern void SendMessageToWeb(string message);

    [SerializeField]
    private DemoType _demoType;
    public DemoType Type => _demoType;

    [SerializeField]
    private string _componentName;
    public string ComponentName => _componentName;

    private void Start()
    {
        GenerateDescriptionMessage();
    }

    private void GenerateDescriptionMessage()
    {
        var component = this.GetComponent(_componentName);
        if (component == null)
        {
            Debug.LogError($"Component {_componentName} not attached to {this.gameObject.name}.");
            return;
        }

        var unityMessage = new UnityMessage();

        
        var controls = new List<string>();
        if (component is ICanReset)
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
