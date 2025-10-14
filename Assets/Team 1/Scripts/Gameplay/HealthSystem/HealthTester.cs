using UnityEngine;
using Gameplay.Player; // adjust if needed

public class PlayerHealthTester : MonoBehaviour
{
    [SerializeField] private Player _player;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _player.TakeDamage(20f);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            _player.Heal(10f);
        }
    }
}
