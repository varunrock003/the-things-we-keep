
using UnityEngine;

/// <summary>
/// Controls Spirit movement inside the current room.
/// Supports WASD and arrow-key movement.
/// </summary>
public class SpiritMovement : MonoBehaviour
{
    [Header("Movement")]
    [Min(0f)]
    public float moveSpeed = 3f;

    [Header("Room Boundaries")]
    public float minX = -4f;
    public float maxX = 4f;
    public float minZ = -4f;
    public float maxZ = 4f;

    private bool canMove = true;
    private Vector3 startingPosition;

    private void Awake()
    {
        startingPosition = transform.position;
    }

    private void Update()
    {
        if (!canMove)
            return;

        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.LeftArrow))
            horizontal -= 1f;

        if (Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.RightArrow))
            horizontal += 1f;

        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.UpArrow))
            vertical += 1f;

        if (Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.DownArrow))
            vertical -= 1f;

        Vector3 direction =
            new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 nextPosition = transform.position +
            direction * moveSpeed * Time.deltaTime;

        nextPosition.x =
            Mathf.Clamp(nextPosition.x, minX, maxX);

        nextPosition.z =
            Mathf.Clamp(nextPosition.z, minZ, maxZ);

        transform.position = nextPosition;
    }

    /// <summary>
    /// Enables or disables movement, for example
    /// when an object is possessed.
    /// </summary>
    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
    }

    /// <summary>
    /// Returns the Spirit to its initial position
    /// and enables movement.
    /// </summary>
    public void ResetMovement()
    {
        transform.position = startingPosition;
        canMove = true;
    }
}
