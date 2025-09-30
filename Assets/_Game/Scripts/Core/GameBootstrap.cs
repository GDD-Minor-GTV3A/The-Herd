using UnityEngine;
using Core.Shared.Utilities;
using Core.Dialogue;

namespace Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [SerializeField][Required] private DialogueManager _dialogueManager;

        void Start()
        {
            _dialogueManager.Initialize();
        }
    }
}
