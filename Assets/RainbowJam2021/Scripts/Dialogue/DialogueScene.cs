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

    private UIManager UIManager;
    private int lineNumber;
    private float timer;
    private bool triggered;

    public void Start()
    {
        UIManager = GameObject.FindObjectOfType<UIManager>();
    }

    public void Update()
    {
        if (!triggered)
        { 
            return;
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            if (lineNumber >= DialogueLines.Length)
            {
                UIManager.SetDialogueBoxActive(false);
            }
            else
            {
                DisplayDialogueLine();
                lineNumber++;
            }
        }
    }

    public void Activate()
    {
        triggered = true;
        lineNumber = 0;
        timer = 2f;
    }

    public void Stop()
	{
        triggered = false;
        UIManager.SetDialogueBoxActive( false );
    }

    private void DisplayDialogueLine()
    {
        DialogueLine line = DialogueLines[lineNumber];
        UIManager.SetDialogueText(line.Subtitle);
        UIManager.SetDialogueNametag(line.Actor);
        UIManager.SetDialogueBoxActive(true);

        // Play the audio!
        if ( line.VoiceLine != "" )
        {
            var voiceLineEvent = FMODUnity.RuntimeManager.CreateInstance(line.VoiceLine);
            voiceLineEvent.getDescription(out var voiceLineDescription);
            voiceLineDescription.getLength(out var voiceLineLengthMs);

            timer = (voiceLineLengthMs / 1000f) + 1f;

            voiceLineEvent.start();
            voiceLineEvent.release();
        }

        if (timer <= 1.0f)
        {
            timer = 5.0f;
        }
    }
}
