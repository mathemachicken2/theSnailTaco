using UnityEngine;

public class Customer : MonoBehaviour
{
    [Header("Movement")]
    public Transform stopPoint;       // Where customer stops at counter
    private bool hasArrived = false;
    //public Transform zoomPoint;

    [Header("Dialogue")]
    public DialogueEntry[] dialogues; // Assign unique dialogues per customer
    private int currentDialogue = 0;

    // ------------------------
    // Movement / Counter Logic
    // ------------------------
    public void MoveToCounter()
    {
        transform.position = stopPoint.position;
        transform.rotation = stopPoint.rotation;
        hasArrived = true;
        Debug.Log("Customer arrived at counter");
    }

    public bool IsAtCounter()
    {
        return hasArrived;
    }

    public void LeaveCounter()
    {
        hasArrived = false;
        currentDialogue = 0; // reset dialogue when leaving
        Debug.Log("Customer left counter");
    }

    // ------------------------
    // Dialogue Logic
    // ------------------------
    // Returns the text of the current dialogue
    public string GetCurrentDialogueText()
    {
        if (dialogues == null || dialogues.Length == 0)
            return "";

        if (currentDialogue >= dialogues.Length)
            currentDialogue = dialogues.Length - 1;

        return dialogues[currentDialogue].text;
    }

    // Returns the options for the current dialogue
    public string[] GetCurrentDialogueOptions()
    {
        if (dialogues == null || dialogues.Length == 0)
            return new string[0];

        if (currentDialogue >= dialogues.Length)
            currentDialogue = dialogues.Length - 1;

        return dialogues[currentDialogue].options;
    }

    // Advance to next dialogue
    public void AdvanceDialogue()
    {
        currentDialogue++;
        if (currentDialogue >= dialogues.Length)
            currentDialogue = dialogues.Length - 1; // Clamp at last
    }
    public void SetDialogueIndex(int index)
    {
        if (index >= 0 && index < dialogues.Length)
            currentDialogue = index;
    }
    public DialogueEntry GetCurrentDialogue()
    {
        if (dialogues == null || dialogues.Length == 0)
            return null;

        return dialogues[currentDialogue];
    }
}