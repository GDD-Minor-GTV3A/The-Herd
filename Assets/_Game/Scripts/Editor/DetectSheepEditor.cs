using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DetectSheep))]
public class DetectSheepEditor : Editor
{
    private void OnSceneGUI()
    {
        DetectSheep detector = (DetectSheep)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(detector.transform.position, Vector3.up, Vector3.forward, 360, detector.radius);

        Vector3 viewAngle01 = DirectionFromAngle(detector.transform.eulerAngles.y, -detector.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(detector.transform.eulerAngles.y, detector.angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(detector.transform.position, detector.transform.position + viewAngle01 * detector.radius);
        Handles.DrawLine(detector.transform.position, detector.transform.position + viewAngle02 * detector.radius);

        Handles.color = Color.green;
        foreach (Transform target in detector.visibleTargets)
        {
            Handles.DrawLine(detector.transform.position, target.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
