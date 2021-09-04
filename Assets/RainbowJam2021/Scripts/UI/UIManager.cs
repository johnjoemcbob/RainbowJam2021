using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject DialogueBox;
    public TMP_Text DialogueText;
    public TMP_Text DialogueNametag;

    public GameObject FailureScreen;
    public TMP_Text FailureText;

    public void SetDialogueBoxActive(bool active)
    {
        DialogueBox.SetActive(active);
    }

    public void SetDialogueText(string text)
    {
        DialogueText.text = text;
    }

    public void SetDialogueNametag(string text)
    {
        DialogueNametag.text = text;
    }

    public void SetFailureScreenActive(bool active)
    {
        FailureScreen.SetActive(active);
    }

    public void SetFailureText(string text)
    {
        FailureText.text = text;
    }
}
