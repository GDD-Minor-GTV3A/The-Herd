using UnityEngine;

public class AmalgamationLineTelegraph : MonoBehaviour
{
    [SerializeField] private LineRenderer line;

    private void Awake()
    {
        if (line == null)
            line = GetComponent<LineRenderer>();

        if (line != null)
            line.enabled = false;
    }

    public void Show(Vector3 start, Vector3 end)
    {
        if (line == null) return;

        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }

    public void Hide()
    {
        if (line == null) return;
        line.enabled = false;
    }
}
