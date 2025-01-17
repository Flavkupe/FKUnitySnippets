

using UnityEngine;

public class DemoObject : MonoBehaviour
{
    [SerializeField]
    private string _componentName;

    public string ComponentName => _componentName;
}