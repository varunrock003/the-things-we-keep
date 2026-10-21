
using UnityEngine;

[RequireComponent(typeof(SpiritMovement))]
public class PossessionController : MonoBehaviour
{
    public float possessionRange = 2f;
    public PossessableObject[] objectsInRoom;

    private PossessableObject currentObject;
    private SpiritMovement movement;

    void Awake()
    {
        movement = GetComponent<SpiritMovement>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentObject != null)
                ReleaseObject();
            else
                TryPossess();
        }

        if (Input.GetKeyDown(KeyCode.Space)
            && currentObject != null)
        {
            currentObject.UseVerb();
        }
    }

    void TryPossess()
    {
        PossessableObject nearest = null;
        float nearestDistance = possessionRange;

        foreach (PossessableObject obj in objectsInRoom)
        {
            if (obj == null || !obj.isActiveAndEnabled)
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
    }

    void ReleaseObject()
    {
        currentObject.Release();
        currentObject = null;

        movement.SetMovementEnabled(true);
    }

    void OnGUI()
    {
        string message = currentObject == null
            ? "WASD: Move | E: Possess nearby object"
            : currentObject.objectName + " | Space: "
              + currentObject.verbName + " | E: Release";

        GUI.Label(new Rect(20, 20, 750, 35), message);
    }
}
