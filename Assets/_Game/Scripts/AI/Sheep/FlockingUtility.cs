using System.Collections.Generic;

using UnityEngine;

namespace Core.AI.Sheep
{
    /// <summary>
    /// Sheep separation and alignment for herds
    /// </summary>
    public static class FlockingUtility
    {
        /// <summary>
        /// Make a herding steer vector
        /// </summary>
        public static Vector3 Steering(
            Transform self,
            IReadOnlyList<Transform> neighbours,
            float separationDistance,
            float separationWeight,
            float alignmentWeight,
            float steerClamp)
        {
            if (self == null || neighbours == null || neighbours.Count == 0)
            {
                return Vector3.zero;
            }

            Vector3 separation = Vector3.zero;
            Vector3 alignment = Vector3.zero;
            int alignCount = 0;

            for (int i = 0; i < neighbours.Count; i++)
            {
                Transform n = neighbours[i];
                if (n == null || n == self) { continue; }

                Vector3 toMe = self.position - n.position;
                float d = toMe.magnitude;
                if (d <= Mathf.Epsilon) { continue; }

                if (d < separationDistance)
                {
                    separation += toMe.normalized * (separationDistance - d);
                }

                alignment += n.forward;
                alignCount++;
            }

            if(alignCount > 0)
            {
                alignment /= alignCount;
            }

            Vector3 steer = separation * separationWeight + alignment * alignmentWeight;
            if(steer.sqrMagnitude > steerClamp * steerClamp)
            {
                steer = steer.normalized * steerClamp;
            }

            return steer;
        }

        /// <summary>
        /// Check on XZ plane.
        /// </summary>
        public static bool IsOutSquare(Vector3 position, Vector3 center, Vector2 halfExtents)
        {
            Vector3 local = position - center;
            return Mathf.Abs(local.x) > halfExtents.x || Mathf.Abs(local.z) > halfExtents.y;
        }
    }
}
