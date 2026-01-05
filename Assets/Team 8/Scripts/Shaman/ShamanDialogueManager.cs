using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShamanDialogueManager : MonoBehaviour
{
    public ShamanSpawner shamanSpawner;

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
    private bool choicesActive = false;

    [System.Serializable]
    public class DialogueChoice
    {
        public string text;
        public int nextLineIndex;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        public string text;
        public DialogueChoice[] choices;
        public int nextAfterThis = -1;
    }

    [Header("Dialogue Data")]
    public DialogueLine[] lines;

    void Start()
    {
        dialoguePanel.SetActive(false);
        choiceButtons.SetActive(false);
        choicesActive = false;
    }

    void Update()
    {
        if (!inDialogue) return;

        // Handle choice selection with number keys
        if (choicesActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ChooseChoiceByIndex(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ChooseChoiceByIndex(1);
            }
            return;
        }

        // Advance dialogue normally
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

        if (line.choices != null && line.choices.Length > 0)
        {
            choiceButtons.SetActive(true);
            choicesActive = true;

            for (int i = 0; i < choiceButtons.transform.childCount; i++)
            {
                Button button = choiceButtons.transform.GetChild(i).GetComponent<Button>();
                TMP_Text buttonText = button.GetComponentInChildren<TMP_Text>();

                if (i < line.choices.Length)
                {
                    button.gameObject.SetActive(true);
                    buttonText.text = line.choices[i].text;

                    int choiceIndex = i;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() =>
                        Choose(line.choices[choiceIndex].nextLineIndex)
                    );
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            choiceButtons.SetActive(false);
            choicesActive = false;
        }
    }

    void ChooseChoiceByIndex(int choiceIndex)
    {
        DialogueLine line = lines[index];

        if (line.choices == null) return;
        if (choiceIndex >= line.choices.Length) return;

        Choose(line.choices[choiceIndex].nextLineIndex);
    }

    public void NextLine()
    {
        DialogueLine line = lines[index];

        if (line.nextAfterThis >= 0)
        {
            index = line.nextAfterThis;
        }
        else
        {
            index++;
        }

        ShowLine();
    }

    public void Choose(int nextLineIndex)
    {
        index = nextLineIndex;
        ShowLine();
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choiceButtons.SetActive(false);
        inDialogue = false;
        choicesActive = false;

        if (shamanSpawner != null && shamanSpawner.Triggered)
        {
            shamanSpawner.InterText.enabled = true;
        }
    }
}
