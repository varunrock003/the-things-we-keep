
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Represents an object that the Spirit can possess.
/// Each object has a name and a special action (verb).
/// </summary>
public class PossessableObject : MonoBehaviour
{
    [Header("Object Details")]
    public string objectName = "Household Object";
    public string verbName = "Interact";

    [Header("Action")]
    public UnityEvent onVerbUsed;

    private bool isPossessed;

    /// <summary>
    /// Called when the Spirit possesses this object.
    /// </summary>
    public void Possess()
    {
        isPossessed = true;
        Debug.Log(objectName + " possessed.");
    }

    /// <summary>
    /// Called when the Spirit releases this object.
    /// </summary>
    public void Release()
    {
        isPossessed = false;
        Debug.Log(objectName + " released.");
    }

    /// <summary>
    /// Activates the special action of the object.
    /// Only works while this object is possessed.
    /// </summary>
    public void UseVerb()
    {
        if (!isPossessed)
            return;

        Debug.Log(objectName + " used: " + verbName);
        onVerbUsed?.Invoke();
    }
}
