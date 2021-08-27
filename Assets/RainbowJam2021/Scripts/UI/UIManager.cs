using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject DialogueBox;
    public TMP_Text DialogueText;
    public TMP_Text DialogueNametag;

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
}
