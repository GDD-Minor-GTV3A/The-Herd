using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using Gameplay.Player;

public class ShamanDialogueManager : MonoBehaviour
{
    public ShamanSpawner shamanSpawner;

    [Header("UI References")]
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public TMP_Text speakerNameText;
    public GameObject choiceButtons;
    public Image portraitFrame;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Portraits")]
    public Sprite shamanSprite;
    public Sprite playerSprite;

    [Header("Events")]
    public UnityEvent onDialogueEnded;

    [Header("Typewriter")]
    public float typingSpeed = 0.04f;

    private int index = 0;
    private bool inDialogue = false;
    private bool choicesActive = false;
    private bool isEnding = false;

    // Typewriter state
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string currentFullLine = "";

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
        
        if (choicesActive)
        {
            if (isTyping) return;

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

        // Advance dialogue or skip typing
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                FinishTypingImmediately();
                return;
            }

            NextLine();
        }
    }

    public void StartDialogue()
    {
        index = 0;
        inDialogue = true;
        dialoguePanel.SetActive(true);

        if (shamanSpawner != null)
            shamanSpawner.InterText.enabled = false;

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

        // Speaker + portrait
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

        // Stop any previous typing
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Hide choices while typing
        choiceButtons.SetActive(false);
        choicesActive = false;

        // Start typewriter
        typingCoroutine = StartCoroutine(TypeLine(line.text));

        // Show choices only after typing finishes (if this line has any)
        StartCoroutine(ShowChoicesAfterTyping(line));
    }

    private IEnumerator TypeLine(string text)
    {
        isTyping = true;
        currentFullLine = text;

        dialogueText.text = text;
        dialogueText.ForceMeshUpdate();

        int totalChars = dialogueText.textInfo.characterCount;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i <= totalChars; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        typingCoroutine = null;
    }

    private void FinishTypingImmediately()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        dialogueText.text = currentFullLine;
        dialogueText.ForceMeshUpdate();
        dialogueText.maxVisibleCharacters = int.MaxValue;

        isTyping = false;
    }

    private IEnumerator ShowChoicesAfterTyping(DialogueLine line)
    {
        while (isTyping) yield return null;

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
        if (index >= lines.Length)
        {
            EndDialogue();
            return;
        }

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
        // Stop typing if dialogue ends mid-line
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;
        currentFullLine = "";

        dialoguePanel.SetActive(false);
        choiceButtons.SetActive(false);
        inDialogue = false;
        choicesActive = false;

        onDialogueEnded?.Invoke();

        if (isEnding && gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            return;
        }

        PlayerInputHandler.EnableAllPlayerActions();

        if (shamanSpawner != null && shamanSpawner.Triggered)
        {
            shamanSpawner.InterText.enabled = true;
        }
    }

    public void SetIsEnding(bool value)
    {
        isEnding = value;
    }
}
