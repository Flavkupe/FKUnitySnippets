using System.Linq;
using UnityEngine;

public class Demo : MonoBehaviour
{
    public enum DemoType
    {
        GameObject,
        UI,
    }

    [SerializeField]
    private string _demoName;

    public string DemoName => _demoName;

    [SerializeField]
    private DemoObject[] _demos;

    [SerializeField]
    private DemoType _demoType;
    public DemoType Type => _demoType;

    private int _currentDemoIndex;

    public bool HasNextDemo => _demos != null && _currentDemoIndex < _demos.Length - 1;
    public bool HasPreviousDemo => _demos != null && _currentDemoIndex > 0;

    private void Start()
    {
        _currentDemoIndex = 0;
        if (_demos.Length == 0)
        {
            return;
        }

        foreach (var demo in _demos)
        {
            demo.gameObject.SetActive(false);
        }

        _demos[_currentDemoIndex].gameObject.SetActive(true);
    }

    public void NextDemo()
    {
        if (!HasNextDemo)
        {
            return;
        }

        _demos[_currentDemoIndex].gameObject.SetActive(false);
        _currentDemoIndex++;
        _demos[_currentDemoIndex].gameObject.SetActive(true);
    }

    public void PreviousDemo()
    {
        if (!HasPreviousDemo)
        {
            return;
        }

        _demos[_currentDemoIndex].gameObject.SetActive(false);
        _currentDemoIndex--;
        _demos[_currentDemoIndex].gameObject.SetActive(true);
    }
}
