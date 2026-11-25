
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls Leena's reaction to discovering the recipe.
/// </summary>
public class LeenaReactionController : MonoBehaviour
{
    public enum LeenaState
    {
        Idle,
        NoticingClue,
        FoundRecipe,
        Completed
    }

    [Header("Movement")]
    public Transform recipeTarget;
    public float moveSpeed = 1.2f;
    public float turnSpeed = 4f;
    public float arrivalDistance = 0.15f;

    [Header("Reaction")]
    public float noticeDuration = 1.5f;

    [Header("Events")]
    public UnityEvent onReactionCompleted;

    public LeenaState CurrentState { get; private set; }

    private Vector3 startPosition;
    private Quaternion startRotation;
    private float noticeTimer;

    private void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        ResetReaction();
    }

    private void Update()
    {
        if (CurrentState == LeenaState.NoticingClue)
        {
            noticeTimer -= Time.deltaTime;

            if (noticeTimer <= 0f)
                CurrentState = LeenaState.FoundRecipe;
        }

        if (CurrentState == LeenaState.FoundRecipe)
        {
            MoveTowardsRecipe();
        }
    }

    /// <summary>
    /// Starts Leena's reaction when the recipe is found.
    /// </summary>
    public void OnRecipeFound()
    {
        if (CurrentState != LeenaState.Idle)
            return;

        CurrentState = LeenaState.NoticingClue;
        noticeTimer = noticeDuration;

        Debug.Log("Leena noticed the recipe.");
    }

    private void MoveTowardsRecipe()
    {
        if (recipeTarget == null)
            return;

        Vector3 target = recipeTarget.position;
        target.y = transform.position.y;

        Vector3 direction = target - transform.position;

        if (direction.magnitude <= arrivalDistance)
        {
            CurrentState = LeenaState.Completed;
            Debug.Log("Leena reaction completed.");
            onReactionCompleted?.Invoke();
            return;
        }

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );
    }

    /// <summary>
    /// Restores Leena to her initial position and state.
    /// </summary>
    public void ResetReaction()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        CurrentState = LeenaState.Idle;
        noticeTimer = 0f;
    }
}
