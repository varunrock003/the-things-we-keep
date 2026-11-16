
using UnityEngine;

public class PreethamPuzzleManager : MonoBehaviour
{
    public GameObject[] puzzleObjects;

    private int nextObject = 0;
    private Renderer[] objectRenderers;
    private Color[] originalColors;

    void Start()
    {
        objectRenderers = new Renderer[puzzleObjects.Length];
        originalColors = new Color[puzzleObjects.Length];

        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            objectRenderers[i] = puzzleObjects[i].GetComponent<Renderer>();
            originalColors[i] = objectRenderers[i].material.color;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ActivatePuzzleObject(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            ActivatePuzzleObject(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            ActivatePuzzleObject(2);
    }

    void ActivatePuzzleObject(int index)
    {
        if (index == nextObject)
        {
            Debug.Log("Correct puzzle object: " + (index + 1));

            objectRenderers[index].material.color = Color.green;

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
            ResetPuzzle();
        }
    }

    void ResetPuzzle()
    {
        nextObject = 0;

        for (int i = 0; i < puzzleObjects.Length; i++)
        {
            objectRenderers[i].material.color = originalColors[i];
        }
    }
}
