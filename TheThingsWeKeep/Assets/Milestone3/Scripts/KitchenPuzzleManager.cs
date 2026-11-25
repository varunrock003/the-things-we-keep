
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the kitchen's puzzle sequence, window clue,
/// blue tin, and recipe discovery.
/// </summary>
public class KitchenPuzzleManager : MonoBehaviour
{
    [Header("Original Numbered Puzzle")]
    public GameObject[] puzzleObjects;

    private int nextObject = 0;
    private Renderer[] objectRenderers;
    private Color[] originalColors;

    [Header("Window Clue")]
    public GameObject windowClue;

    [Header("Blue Tin Puzzle")]
    public GameObject tinLid;
    public GameObject recipeCard;

    [Header("Events")]
    public UnityEvent onRecipeFound;

    private bool clueRevealed = false;
    private bool recipeFound = false;

    private void Start()
    {
        objectRenderers = new Renderer[puzzleObjects.Length];
        originalColors = new Color[puzzleObjects.Length];

        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            if (puzzleObjects[i] == null)
                continue;

            objectRenderers[i] =
                puzzleObjects[i].GetComponent<Renderer>();

            if (objectRenderers[i] != null)
                originalColors[i] =
                    objectRenderers[i].material.color;
        }

        ResetPuzzle();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ActivatePuzzleObject(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ActivatePuzzleObject(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ActivatePuzzleObject(2);
    }

    private void ActivatePuzzleObject(int index)
    {
        if (index < 0 || index >= puzzleObjects.Length)
            return;

        if (index == nextObject)
        {
            Debug.Log("Correct puzzle object: " + (index + 1));

            if (objectRenderers[index] != null)
                objectRenderers[index].material.color =
                    Color.green;

            nextObject++;

            if (nextObject == puzzleObjects.Length)
            {
                Debug.Log("Puzzle completed successfully!");
                nextObject = 0;
            }
        }
        else
        {
            Debug.Log("Wrong order! Puzzle reset.");
            ResetNumberedPuzzle();
        }
    }

    /// <summary>
    /// Reveals the window clue when Steam is activated.
    /// </summary>
    public void RevealWindowClue()
    {
        if (clueRevealed)
            return;

        clueRevealed = true;

        if (windowClue != null)
            windowClue.SetActive(true);

        Debug.Log("Window clue revealed.");
    }

    /// <summary>
    /// Opens the blue tin after the window clue
    /// has been revealed.
    /// </summary>
    public void OpenBlueTin()
    {
        if (!clueRevealed)
        {
            Debug.Log("Reveal the window clue first.");
            return;
        }

        if (recipeFound)
            return;

        recipeFound = true;

        if (tinLid != null)
            tinLid.SetActive(false);

        if (recipeCard != null)
            recipeCard.SetActive(true);

        Debug.Log("Blue tin opened. Recipe card discovered.");
        onRecipeFound?.Invoke();
    }

    private void ResetNumberedPuzzle()
    {
        nextObject = 0;

        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            if (objectRenderers[i] != null)
                objectRenderers[i].material.color =
                    originalColors[i];
        }
    }

    /// <summary>
    /// Resets the kitchen puzzle to its starting state.
    /// </summary>
    public void ResetPuzzle()
    {
        ResetNumberedPuzzle();

        clueRevealed = false;
        recipeFound = false;

        if (windowClue != null)
            windowClue.SetActive(false);

        if (tinLid != null)
            tinLid.SetActive(true);

        if (recipeCard != null)
            recipeCard.SetActive(false);
    }
}
