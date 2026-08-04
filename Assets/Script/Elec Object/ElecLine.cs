using UnityEngine;
using UnityEngine.InputSystem;

public class ElecLine : MonoBehaviour
{
    [SerializeField] private GameObject startTarget;
    [SerializeField] private GameObject endTarget;

    private LineRenderer line;
    private float width = 0.3f;

    private Vector2 startPos;
    private Vector2 endPos;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.startColor = Color.red;
        line.startWidth = width;
        line.endWidth = width;
        line.positionCount = 2;

        if (startTarget == null || endTarget == null)
        {
            Debug.Log("input target object");
            return;
        }
        startPos = startTarget.transform.position;
        endPos = endTarget.transform.position;
        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);
    }

    public void UpdateTarget(GameObject _endTarget)
    {
        if (_endTarget == startTarget || _endTarget == startTarget)
        {
            Cancel();
            return;
        }
        endTarget = _endTarget;

        updatePos(endTarget.transform.position);
    }

    public void updatePos(Vector2 _endPos)
    {
        endPos = _endPos;
        line.SetPosition(1, endPos);
    }
    public void Cancel()
    {
        startPos = startTarget.transform.position;
        endPos = endTarget.transform.position;

        line.SetPosition(0, startPos);
        line.SetPosition(1, endPos);

    }

    public float GetDistance()
    {
        Vector2 lineVec = endPos - startPos;
        Vector2 mousePos = GetMousePos();

        if (lineVec.sqrMagnitude == 0) return 0;
        float t = Vector2.Dot(mousePos - startPos, lineVec) / lineVec.sqrMagnitude;
        t = Mathf.Clamp01(t);

        Vector2 projectionPoint = startPos + t * lineVec;

        return (mousePos - projectionPoint).sqrMagnitude;
    }
    public Vector2[] GetNearPoint()
    {
        float dist_from_start = (GetMousePos() - startPos).sqrMagnitude;
        float dist_from_end = (GetMousePos() - endPos).sqrMagnitude;

        Vector2[] point = new Vector2[2];

        if (dist_from_start > dist_from_end)
        {
            point[0] = startPos;
            point[1] = endPos;
        }
        else
        {
            GameObject obj = endTarget;
            endTarget = startTarget;
            startTarget = obj;

            Vector2 tempPos = startPos;
            startPos = endPos;
            endPos = tempPos;

            line.SetPosition(0, startPos);
            line.SetPosition(1, endPos);

            point[0] = startPos;
            point[1] = endPos;
        }

        return point;
    }

    private Vector2 GetMousePos()
    {
        if (Mouse.current == null) return Vector2.zero;
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
