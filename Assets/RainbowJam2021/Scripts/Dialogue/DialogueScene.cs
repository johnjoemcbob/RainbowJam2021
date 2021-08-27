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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            triggered = true;
            lineNumber = 0;
            timer = 2f;

            this.gameObject.GetComponent<MeshRenderer>().enabled = false;
            this.gameObject.GetComponent<SphereCollider>().enabled = false;
        }
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
                this.gameObject.SetActive(false);
            }
            else
            {
                DisplayDialogueLine();

                timer = 5f;
                lineNumber++;
            }
        }
    }

    private void DisplayDialogueLine()
    {
        DialogueLine line = DialogueLines[lineNumber];
        UIManager.SetDialogueText(line.Subtitle);
        UIManager.SetDialogueNametag(line.Actor);
        UIManager.SetDialogueBoxActive(true);
    }
}
