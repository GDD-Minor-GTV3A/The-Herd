using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Shaman Dialogue")]
public class DialogueData : ScriptableObject
{
    public ShamanDialogueManager.DialogueLine[] lines;
}