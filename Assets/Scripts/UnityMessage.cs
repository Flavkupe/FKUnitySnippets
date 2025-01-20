using UnityEngine;

[System.Serializable]
public class UnityMessage
{
    [SerializeField]
    public string type = "UNITY_MESSAGE";

    [SerializeField]
    public string[] controls;
}

[System.Serializable]
public class WebGLReadyMessage
{
    [SerializeField]
    public string type = "WEB_GL_READY";

    [SerializeField]
    public bool isReady = true;
}
