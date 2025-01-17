using System.Linq;
using UnityEngine;

public class WebInputManager : MonoBehaviour
{
    private DemoObject _currentDemoObject;

    private DemoObject[] _demoObjects;

    [SerializeField]
    private GameObject _notAvailableText;

    public void Start()
    {
        _demoObjects = Resources.LoadAll<DemoObject>("Prefabs");
    }

    public void SwitchComponent(string componentName)
    {
        _notAvailableText.SetActive(false);
        if (_currentDemoObject != null)
        {
            if (_currentDemoObject.name == componentName)
            {
                return;
            }

            Destroy(_currentDemoObject.gameObject);
        }

        _currentDemoObject = null;

        var demoObject = _demoObjects.FirstOrDefault(_demoObjects => string.Equals(_demoObjects.name, componentName, System.StringComparison.OrdinalIgnoreCase));

        if (demoObject == null)
        {
            _notAvailableText.SetActive(true);
            Debug.LogWarning($"No object found for name {componentName}");
            return;
        }

        _currentDemoObject = Instantiate(demoObject);
    }
}
