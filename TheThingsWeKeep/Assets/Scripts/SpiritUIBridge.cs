
using UnityEngine;

/// <summary>
/// Connects Spirit possession state to the UI.
/// </summary>
public class SpiritUIBridge : MonoBehaviour
{
    public PossessionController possession;
    public InteractionUIController ui;

    private PossessableObject lastObject;
    private PossessableObject lastNearby;

    private void Update()
    {
        if (possession == null || ui == null)
            return;

        PossessableObject current = possession.CurrentObject;

        if (current != null)
        {
            if (lastObject != current)
            {
                ui.ShowPossessedPrompt(
                    current.objectName,
                    current.verbName
                );
            }

            lastObject = current;
            lastNearby = null;
            return;
        }

        PossessableObject nearby = FindNearbyObject();

        if (lastObject != null || lastNearby != nearby)
        {
            if (nearby != null)
                ui.ShowNearbyPrompt(nearby.objectName);
            else
                ui.HidePrompt();
        }

        lastObject = null;
        lastNearby = nearby;
    }

    private PossessableObject FindNearbyObject()
    {
        if (possession.objectsInRoom == null)
            return null;

        PossessableObject nearest = null;
        float nearestDistance = possession.possessionRange;

        foreach (PossessableObject obj in possession.objectsInRoom)
        {
            if (obj == null || !obj.isActiveAndEnabled)
                continue;

            float distance = Vector3.Distance(
                possession.transform.position,
                obj.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearest = obj;
                nearestDistance = distance;
            }
        }

        return nearest;
    }
}
