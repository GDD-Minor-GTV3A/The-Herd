using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectSheep : MonoBehaviour
{
    public float radius = 15f;
    [Range(0, 360)]
    public float angle = 360f;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [HideInInspector] public List<Transform> visibleTargets = new();

    private void Start() => StartCoroutine(FOVRoutine());

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        visibleTargets.Clear();

        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        foreach (Collider targetCollider in rangeChecks)
        {
            if (!targetCollider.CompareTag("Sheep"))
                continue;

            Transform target = targetCollider.transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
}
