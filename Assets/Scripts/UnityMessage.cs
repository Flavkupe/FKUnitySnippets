using UnityEngine;

[System.Serializable]
public class UnityMessage
{
    public string type = "UNITY_MESSAGE";

    [SerializeField]
    public string[] controls;
}
