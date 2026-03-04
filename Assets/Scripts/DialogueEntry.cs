[System.Serializable]
public class DialogueEntry
{
    public string text;                 // Customer's dialogue
    public string[] options;            // Buttons the player can pick
    public string[] responses;          // Replies for each choice
}

public class DialogueChoice
{
    public string buttonText;     // Text shown on button
    public int nextDialogueIndex; // Which dialogue to go to
}