using UnityEngine;

namespace Gameplay.Scarecrow
{
    public class LookAtPlayer : MonoBehaviour
    {
        [SerializeField] private Transform _player;

        void Update()
        {
            if (_player != null)
            {
                transform.LookAt(new Vector3(_player.position.x, transform.position.y, _player.position.z));
            }
        }
    }
}
