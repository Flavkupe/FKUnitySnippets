

using UnityEngine;

public class DemoObject : MonoBehaviour
{
    public enum DemoType
    {
        GameObject,
        UI,
    }

    [SerializeField]
    private DemoType _demoType;
    public DemoType Type => _demoType;

    [SerializeField]
    private string _componentName;
    public string ComponentName => _componentName;
}