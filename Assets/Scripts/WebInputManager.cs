using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class WebInputManager : MonoBehaviour
{
    private Demo _currentDemo;
    public Demo CurrentDemo => _currentDemo;

    [DllImport("__Internal")]
    private static extern void SendMessageToWeb(string message);

    private Demo[] _demos;

    [SerializeField]
    private Canvas _canvas;

    [SerializeField]
    private GameObject _notAvailableText;

    [SerializeField]
    private string _testComponent;

    [SerializeField]
    private Button _nextDemoButton;

    [SerializeField]
    private Button _previousDemoButton;

    public void Start()
    {
        _demos = Resources.LoadAll<Demo>("Prefabs");

        if (!string.IsNullOrEmpty(_testComponent))
        {
            SwitchDemo(_testComponent);
        }

        _nextDemoButton.onClick.AddListener(NextPressed);
        _previousDemoButton.onClick.AddListener(PreviousPressed);

        SendIsReadyMessage();
    }

    public void SendIsReadyMessage()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        var message = new WebGLReadyMessage();
        SendMessageToWeb(JsonUtility.ToJson(message));
#endif
    }

    public void SwitchDemo(string demoName)
    {
        _notAvailableText.SetActive(false);
        if (_currentDemo != null)
        {
            if (_currentDemo.name == demoName)
            {
                return;
            }

            Destroy(_currentDemo.gameObject);
        }

        _currentDemo = null;

        var currentDemo = _demos.FirstOrDefault(demo => string.Equals(demo.DemoName, demoName, System.StringComparison.OrdinalIgnoreCase));

        if (currentDemo == null)
        {
            _notAvailableText.SetActive(true);
            Debug.LogWarning($"No object found for name {demoName}");
            return;
        }

        _currentDemo = Instantiate(currentDemo);

        if (_currentDemo.Type == Demo.DemoType.UI)
        {
            _currentDemo.transform.SetParent(_canvas.transform, false);
        }

        UpdateButtonState();
    }

    public void NextPressed()
    {
        _currentDemo.NextDemo();
        UpdateButtonState();
    }

    public void PreviousPressed()
    {
        _currentDemo.PreviousDemo();
        UpdateButtonState();
    }

    private void UpdateButtonState()
    {
        _previousDemoButton.gameObject.SetActive(_currentDemo.HasPreviousDemo);
        _nextDemoButton.gameObject.SetActive(_currentDemo.HasNextDemo);
    }
}
