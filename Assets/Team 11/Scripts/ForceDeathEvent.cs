using UnityEngine;
using Gameplay.Player; // Player component with Die()

[RequireComponent(typeof(Player))]
public class ForceDeathEvent : MonoBehaviour
{
    [Header("Death Trigger")]
    [SerializeField] private KeyCode key = KeyCode.M;

    [Header("Debug Overlay")]
    [SerializeField] private bool showOnScreen = true;
    [SerializeField] private float onScreenSeconds = 2f;

    private Player player;
    private float overlayTimer;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(key)) return;

        Debug.Log("☠️ [ForceDeathEvent] Force-death key pressed → calling Player.Die()");

        // IMPORTANT: Do NOT also call DeathScreenUI.Show() here,
        // because Player.Die() should trigger your normal death pipeline (listener/UI).
        player.Die();

        if (showOnScreen)
            overlayTimer = onScreenSeconds;
    }

    private void OnGUI()
    {
        if (!showOnScreen) return;
        if (overlayTimer <= 0f) return;

        overlayTimer -= Time.unscaledDeltaTime;

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 30,
            alignment = TextAnchor.MiddleCenter
        };
        style.normal.textColor = Color.red;

        GUI.Label(new Rect(0, 40, Screen.width, 60), "PLAYER DIES", style);
    }
}
