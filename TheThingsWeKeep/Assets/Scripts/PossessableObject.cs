
using UnityEngine;
using UnityEngine.Events;

public class PossessableObject : MonoBehaviour
{
    [Header("Object Details")]
    public string objectName = "Household Object";
    public string verbName = "Interact";

    [Header("Action")]
    public UnityEvent onVerbUsed;

    private bool isPossessed;

    public void Possess()
    {
        isPossessed = true;
        Debug.Log(objectName + " possessed.");
    }

    public void Release()
    {
        isPossessed = false;
        Debug.Log(objectName + " released.");
    }

    public void UseVerb()
    {
        if (!isPossessed)
            return;

        Debug.Log(objectName + " used: " + verbName);
        onVerbUsed?.Invoke();
    }
}
