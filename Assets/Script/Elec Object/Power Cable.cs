using UnityEngine;

public class PowerCable : MonoBehaviour
{
    [SerializeField] private GameObject start_target_obj;
    [SerializeField] private GameObject end_target_obj;

    private IPower start_target;
    private IPower end_target;

    private LineRenderer line;
    private float width = 0.3f;

    public GameObject start_obj => start_target_obj;
    public GameObject end_obj => end_target_obj;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();

        line.positionCount = 2;
        line.startWidth = width;
        line.endWidth = width;
    }

    private void Start()
    {
        if (start_target_obj == null || end_target_obj == null) return;

        line.SetPosition(0, start_target_obj.transform.position);
        line.SetPosition(1, end_target_obj.transform.position);

        start_target = start_target_obj.GetComponent<IPower>();
        end_target = end_target_obj.GetComponent<IPower>();

        start_target.Connected(end_target);
        end_target.Connected(start_target);
    }

    public void UpdateCablePosition(Vector2 mousePos)
    {
        line.SetPosition(0, start_target_obj.transform.position);
        line.SetPosition(1, mousePos);
    }
    public void UpdateCablePosition()
    {
        line.SetPosition(0, start_target_obj.transform.position);
        line.SetPosition(1, end_target_obj.transform.position);
    }
    public void UpdateTargetObject(GameObject target_obj)
    {
        if (target_obj != null && target_obj.CompareTag("Power") && start_target_obj != target_obj && end_target_obj != target_obj)
        {
            start_target.DisConnected(end_target);
            end_target.DisConnected(start_target);
            end_target_obj = target_obj;
            end_target = end_target_obj.GetComponent<IPower>();

            start_target.Connected(end_target);
            end_target.Connected(start_target);
        }
        line.SetPosition(1, end_target_obj.transform.position);

    }
    public void CheckNearPoint(Vector2 mousePos)
    {
        float dist_from_start = (mousePos - (Vector2)start_target_obj.transform.position).sqrMagnitude;
        float dist_from_end = (mousePos - (Vector2)end_target_obj.transform.position).sqrMagnitude;

        if (dist_from_start < dist_from_end)
        {
            GameObject obj = start_target_obj;
            start_target_obj = end_target_obj;
            end_target_obj = obj;

            IPower temp = start_target;
            start_target = end_target;
            end_target = temp;
        }

        line.SetPosition(0, start_target_obj.transform.position);
        line.SetPosition(1, end_target_obj.transform.position);
    }
    public float GetDistance(Vector2 mousePos)
    {
        Vector2 lineVec = end_target_obj.transform.position - start_target_obj.transform.position;
        if (lineVec.sqrMagnitude == 0) return 0;

        float t = Vector2.Dot(mousePos - (Vector2)start_target_obj.transform.position, lineVec) / lineVec.sqrMagnitude;
        t = Mathf.Clamp01(t);

        Vector2 projectionPoint = (Vector2)start_target_obj.transform.position + t * lineVec;

        return (mousePos - projectionPoint).sqrMagnitude;
    }

    public bool IsDuplicated(GameObject start, GameObject end)
    {
        if (start == start_target_obj && end == end_target_obj) return true;
        else if (start == end_target_obj && end == start_target_obj) return true;
        return false;
    }
}
