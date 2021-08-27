using UnityEngine;

[System.Serializable]
public struct DialogueLine
{
    public string Actor;
    [FMODUnity.EventRef]
    public string VoiceLine;
    public string Subtitle;
}

public class DialogueScene : MonoBehaviour 
{
    public DialogueLine[] DialogueLines;

    public UIManager UIManager;

    private int lineNumber;
    private float timer;
    private bool dialogueActive;

    public void Start()
    {
        UIManager = GameObject.FindObjectOfType<UIManager>();
    }

    public void OnTriggerEnter(Collider other)
    {
        // TODO: test for player collider
        // TODO: name tag for each line
        dialogueActive = true;
        lineNumber = 0;
        DisplayDialogueLine();
        UIManager.SetDialogueBoxActive(true);
    }

    public void Update()
    {
        if (!dialogueActive)
        { 
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            lineNumber++;
            
            if (lineNumber >= DialogueLines.Length)
            {
                dialogueActive = false;
                UIManager.SetDialogueBoxActive(false);
            }
            else
            {
                DisplayDialogueLine();
            }
        }
    }

    private void DisplayDialogueLine()
    {
        timer = 5f;
        DialogueLine line = DialogueLines[lineNumber];
        UIManager.SetDialogueText(line.Subtitle);
        UIManager.SetDialogueNametag(line.Actor);
    }
}
