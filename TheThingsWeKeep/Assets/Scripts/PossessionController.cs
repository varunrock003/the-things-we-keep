
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Allows the Spirit to possess nearby household objects
/// and invoke their unique interaction verbs.
/// </summary>
[RequireComponent(typeof(SpiritMovement))]
public class PossessionController : MonoBehaviour
{
    [Header("Possession")]
    [Min(0f)]
    public float possessionRange = 2f;

    public PossessableObject[] objectsInRoom;

    [Header("Events")]
    public UnityEvent onPossessed;
    public UnityEvent onReleased;

    private PossessableObject currentObject;
    private SpiritMovement movement;

    public PossessableObject CurrentObject => currentObject;

    private void Awake()
    {
        movement = GetComponent<SpiritMovement>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentObject != null)
                ReleaseObject();
            else
                TryPossess();
        }

        if (Input.GetKeyDown(KeyCode.Space) &&
            currentObject != null)
        {
            currentObject.UseVerb();
        }
    }

    /// <summary>
    /// Possesses the nearest eligible object within range.
    /// </summary>
    public void TryPossess()
    {
        if (currentObject != null ||
            objectsInRoom == null)
            return;

        PossessableObject nearest = null;
        float nearestDistance = possessionRange;

        foreach (PossessableObject obj in objectsInRoom)
        {
            if (obj == null ||
                !obj.isActiveAndEnabled)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                obj.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearest = obj;
                nearestDistance = distance;
            }
        }

        if (nearest == null)
        {
            Debug.Log("No object nearby to possess.");
            return;
        }

        currentObject = nearest;
        currentObject.Possess();

        movement.SetMovementEnabled(false);
        onPossessed?.Invoke();
    }

    /// <summary>
    /// Releases the currently possessed object.
    /// </summary>
    public void ReleaseObject()
    {
        if (currentObject == null)
            return;

        currentObject.Release();
        currentObject = null;

        movement.SetMovementEnabled(true);
        onReleased?.Invoke();
    }

    /// <summary>
    /// Releases the object and resets the Spirit position.
    /// Called when restarting the kitchen.
    /// </summary>
    public void ResetPossession()
    {
        ReleaseObject();
        movement.ResetMovement();
    }

    private void OnGUI()
    {
        string message = currentObject == null
            ? "WASD: Move | E: Possess nearby object"
            : currentObject.objectName + " | Space: " +
              currentObject.verbName + " | E: Release";

        GUI.Label(new Rect(20, 20, 750, 35), message);
    }
}
