using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShamanDialogueManager : MonoBehaviour
{
    public ShamanDialogueTrigger shamanDialogueTrigger;
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText; 
    public GameObject choiceButtons;
    public Image portraitFrame;

    [Header("Portraits")]
    public Sprite shamanSprite;
    public Sprite playerSprite;

    private int index = 0;
    private bool inDialogue = false;

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;  
        public string text;
        public bool hasChoices;
    }

    [Header("Dialogue Data")]
    public DialogueLine[] lines;

    void Start()
    {
        dialoguePanel.SetActive(false);
        choiceButtons.SetActive(false);
    }

    void Update()
    {
        if (!inDialogue) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            NextLine();
        }
    }

    public void StartDialogue()
    {
        index = 0;
        inDialogue = true;
        dialoguePanel.SetActive(true);
        ShowLine();
    }

    void ShowLine()
    {
        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = lines[index];
        dialogueText.text = line.text;

        if (line.speaker == "Player")
        {
            speakerNameText.text = "Player";
            portraitFrame.sprite = playerSprite;
        }
        else
        {
            speakerNameText.text = "Shaman";
            portraitFrame.sprite = shamanSprite;  
        }


        choiceButtons.SetActive(line.hasChoices);
    }

    public void NextLine()
    {
        if (lines[index].hasChoices)
            return;

        index++;
        ShowLine();
    }

    public void PressYes()
    {
        EndDialogue();
    }

    public void PressNo()
    {
        EndDialogue();
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choiceButtons.SetActive(false);
        inDialogue = false;
        shamanDialogueTrigger.ShowInterText();
    }
}
