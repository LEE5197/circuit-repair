using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour, IPower
{
    private float current_power;
    public void Connected(IPower target)
    {
    }

    public void DisConnected(IPower target)
    {
    }

    public void SetPower(float power)
    {
        current_power += power;
        if (current_power < 0) current_power = 0;
    }

    private Rigidbody2D rigid;
    private LineRenderer line;
    private float time = 0f;
    private int target_point_index = 0;
    private bool IsActive = false;

    private Vector2 targetPos = Vector2.zero;
    private Vector3 position_offset = Vector3.zero;

    private Controller player;

    [Header("Moveing platform default position and target point")]
    [SerializeField] private List<Vector2> target_points;

    [Space]
    [Header("Speed lerp")]
    [SerializeField] private AnimationCurve movement_curve;

    [Space]
    [Header("Movement")]
    [SerializeField] private float maxSpeed;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        line = GetComponent<LineRenderer>();
        player = null;

        InitTargetPoints();
        InitDrawLineBetweenTarget();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Move();
        if (((Vector2)rigid.position - targetPos).sqrMagnitude < 0.01f)
        {
            UpdatePosition();
            UpdateTargetPos();
        }
    }
    private void UpdatePosition()
    {
        Vector2 curPos = rigid.position;
        position_offset += ((Vector3)targetPos - (Vector3)rigid.position);
        rigid.position = targetPos;
    }
    private void UpdateTargetPos()
    {
        if (IsActive)
        {
            if (target_point_index < target_points.Count)
            {
                targetPos = target_points[target_point_index++];
            }
            else
            {
                IsActive = false;
                target_point_index = target_points.Count - 1;
            }
        }
        else
        {
            if (target_point_index >= 0)
            {
                targetPos = target_points[target_point_index--];
            }
            else
            {
                IsActive = true;
                target_point_index = 0;
            }
        }
    }
    private void Move()
    {
        float rate;
        float speed;
        if (IsActive)
        {
            rate = movement_curve.Evaluate(time);
            speed = rate * maxSpeed;
            time += Time.fixedDeltaTime;
        }
        else
        {
            rate = 0.3f;
            speed = rate * maxSpeed;
            time = 0;
        }

        if (current_power <= 0f) speed = 0;

        Vector2 curPos = rigid.position;
        Vector2 nextPos = Vector2.MoveTowards(rigid.position, targetPos, speed * Time.fixedDeltaTime);

        position_offset = nextPos - curPos;

        rigid.MovePosition(nextPos);
    }

    private void InitTargetPoints()
    {

        for(int i = 0; i < target_points.Count; i++)
        {
            target_points[i] = new Vector2((int)target_points[i].x, (int)target_points[i].y);
        }

        transform.position = target_points[0];
    }
    private void InitDrawLineBetweenTarget()
    {
        line.positionCount = target_points.Count;
        line.startWidth = 0.3f;
        line.endWidth = 0.3f;

        for(int i = 0; i < target_points.Count; i++)
        {
            line.SetPosition(i, target_points[i]);
        }

        targetPos = target_points[target_point_index++];
        IsActive = true;
    }

    // 셀레스트와 유사한 동작을 위해 플레이어 점프 스크립트 변경해야 할 필요 있음, 밑의 스크립트 일단 동작은 되긴 하는데 프레임 버벅이는 문제 발생함
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && player == null)
        {
            player = collision.gameObject.GetComponent<Controller>();
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        collision.transform.position += position_offset;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player.bufferTimer > 0f)
            {
                Vector2 dir = position_offset.normalized;
                player.rigid.linearVelocity+=dir * maxSpeed * 10;
            }
        }
    }
}
