#if UNITY_EDITOR
using UnityEngine;

namespace Core.AI.Sheep
{
    public partial class SheepHerdController
    {
        private void OnDrawGizmosSelected()
        {
            if (_player == null) return;
            Gizmos.color = Color.yellow;
            Vector3 c = _player.position;
            Vector3 size = new Vector3(_halfExtents.x * 2f, 0.01f, _halfExtents.y * 2f);
            Gizmos.DrawWireCube(new Vector3(c.x, c.y + 0.05f, c.z), size);
        }
    }
}
#endif
