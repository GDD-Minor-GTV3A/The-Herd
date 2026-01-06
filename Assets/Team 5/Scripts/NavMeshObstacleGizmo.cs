using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways]
public class NavMeshObstacleGizmo : MonoBehaviour
{
    /// <summary>
    /// Method for drawing Gizmos for the NavMesh Obstacle.
    /// </summary>
    private void OnDrawGizmos()
    {
        var obstacle = GetComponent<NavMeshObstacle>();
        if (!obstacle || !obstacle.enabled)
            return;

        Gizmos.color = new Color(1f, 0f, 0f, 0.6f);
        Gizmos.matrix = transform.localToWorldMatrix;

        if (obstacle.shape == NavMeshObstacleShape.Box)
        {
            Gizmos.DrawWireCube(obstacle.center, obstacle.size);
        }
        else if (obstacle.shape == NavMeshObstacleShape.Capsule)
        {
            Gizmos.DrawWireSphere(obstacle.center, obstacle.radius);
        }
    }
}