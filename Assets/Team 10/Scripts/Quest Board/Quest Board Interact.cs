using UnityEngine;

public class QuestBoardInteract : MonoBehaviour
{
    [Header("References (assign in Inspector)")]
    [Tooltip("The UI GameObject to enable when E is pressed (menu panel).")]
    public GameObject menuPanel;

    [Tooltip("The TMP prompt / gameobject that says 'Press E to interact'.")]
    public GameObject promptTextObject;

    [Tooltip("The collider that defines the interaction area. Can be any collider in the scene.")]
    public Collider detectionCollider;

    [Header("Options")]
    [Tooltip("If true, the menuPanel will be automatically closed when the player leaves the area.")]
    public bool closeOnExit = true;

    // internal
    private Transform _player;
    private bool _playerInArea;

    private void Start()
    {
        // fallback to using this object's collider if none assigned
        if (detectionCollider == null)
            detectionCollider = GetComponent<Collider>();

        // find player transform
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            _player = playerObj.transform;
        else
            Debug.LogWarning("[QuestBoardInteract] No GameObject with tag 'Player' found in scene.");

        // initial UI states
        if (promptTextObject != null) promptTextObject.SetActive(false);
        // Do NOT force menuPanel state — assume it starts disabled like you want
    }

    private void Update()
    {
        if (_player == null || detectionCollider == null) return;

        // check if player's position is inside the detection collider
        bool inside = detectionCollider.bounds.Contains(_player.position);

        if (inside && !_playerInArea)
        {
            _playerInArea = true;
            if (promptTextObject != null) promptTextObject.SetActive(true);
            Debug.Log("[QuestBoardInteract] Player entered area.");
        }
        else if (!inside && _playerInArea)
        {
            _playerInArea = false;
            if (promptTextObject != null) promptTextObject.SetActive(false);
            Debug.Log("[QuestBoardInteract] Player left area.");

            if (closeOnExit && menuPanel != null)
                menuPanel.SetActive(false);
        }

        // while inside, listen for E
        if (_playerInArea && Input.GetKeyDown(KeyCode.E))
        {
            if (menuPanel != null)
            {
                menuPanel.SetActive(true);
                Debug.Log("[QuestBoardInteract] Menu Panel opened via E.");
            }
            else
            {
                Debug.LogWarning("[QuestBoardInteract] menuPanel reference is not assigned.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionCollider != null)
        {
            Gizmos.color = new Color(0f, 0.7f, 1f, 0.15f);
            Gizmos.DrawCube(detectionCollider.bounds.center, detectionCollider.bounds.size);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(detectionCollider.bounds.center, detectionCollider.bounds.size);
        }
    }
}
