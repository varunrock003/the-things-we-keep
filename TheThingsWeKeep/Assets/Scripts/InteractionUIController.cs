
using UnityEngine;
using TMPro;

/// <summary>
/// Displays the player's interaction instructions,
/// story clues and completion messages.
/// </summary>
public class InteractionUIController : MonoBehaviour
{
    [Header("Interaction")]
    public GameObject interactionPrompt;
    public TMP_Text interactionText;
    public GameObject possessedPrompt;
    public TMP_Text possessedText;

    [Header("Clue")]
    public GameObject cluePanel;
    public TMP_Text clueText;

    [Header("Completion")]
    public GameObject completionPanel;
    public TMP_Text completionText;

    private void Start()
    {
        ResetUI();
    }

    public void ShowNearbyPrompt(string objectName)
    {
        HidePrompt();

        if (interactionText != null)
            interactionText.text =
                "Press E to possess " + objectName;

        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
    }

    public void ShowPossessedPrompt(
        string objectName, string verbName)
    {
        HidePrompt();

        if (possessedText != null)
            possessedText.text =
                "Space: " + verbName + " | E: Release";

        if (possessedPrompt != null)
            possessedPrompt.SetActive(true);
    }

    public void HidePrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (possessedPrompt != null)
            possessedPrompt.SetActive(false);
    }

    public void ShowClue(string message)
    {
        if (clueText != null)
            clueText.text = message;

        if (cluePanel != null)
            cluePanel.SetActive(true);
    }

    public void HideClue()
    {
        if (cluePanel != null)
            cluePanel.SetActive(false);
    }

    public void ShowCompletion(string message)
    {
        HidePrompt();

        if (completionText != null)
            completionText.text = message;

        if (completionPanel != null)
            completionPanel.SetActive(true);
    }

    public void ResetUI()
    {
        HidePrompt();
        HideClue();

        if (completionPanel != null)
            completionPanel.SetActive(false);
    }
}
