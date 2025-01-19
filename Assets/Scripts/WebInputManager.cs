using System.Linq;
using UnityEngine;

public class WebInputManager : MonoBehaviour
{
    private DemoObject _currentDemoObject;
    public DemoObject CurrentDemoObject => _currentDemoObject;

    private DemoObject[] _demoObjects;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private GameObject _notAvailableText;

    [SerializeField]
    private string _testComponent;

    public void Start()
    {
        _demoObjects = Resources.LoadAll<DemoObject>("Prefabs");

        if (!string.IsNullOrEmpty(_testComponent))
        {
            SwitchComponent(_testComponent);
        }
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

        if (_currentDemoObject.Type == DemoObject.DemoType.UI)
        {
            _currentDemoObject.transform.SetParent(_canvas.transform, false);
        }
    }
}
