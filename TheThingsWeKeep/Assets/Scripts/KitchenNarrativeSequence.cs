
using UnityEngine;

/// <summary>
/// Controls the kitchen narrative ending and restart.
/// </summary>
public class KitchenNarrativeSequence : MonoBehaviour
{
    [Header("References")]
    public LeenaReactionController leena;
    public GameObject endingCanvas;

    private void Start()
    {
        if (endingCanvas != null)
            endingCanvas.SetActive(false);
    }

    /// <summary>
    /// Displays the ending after Leena completes her reaction.
    /// </summary>
    public void ShowEnding()
    {
        if (endingCanvas != null)
            endingCanvas.SetActive(true);

        Debug.Log("Kitchen narrative completed.");
    }

    /// <summary>
    /// Hides the ending and resets Leena.
    /// </summary>
    public void RestartNarrative()
    {
        if (endingCanvas != null)
            endingCanvas.SetActive(false);

        if (leena != null)
            leena.ResetReaction();

        Debug.Log("Kitchen narrative restarted.");
    }
}
