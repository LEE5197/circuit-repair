using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Controller : MonoBehaviour
{
    public Rigidbody2D rigid { get; private set; }
    public Animator anim { get; private set; }
    public Vector2 inputVec { get; private set; }
    public SpriteRenderer render { get; private set; }
    private PlayerFSM fsm;

    public bool jumpPress { get; private set; }

    public Vector2 mousePos { get; private set; }

    private OutLineShader OutLineShader;

	#region parameter
	[Header("Movement")]
    public float maxSpeed;
    public float acceleration;
    public float decceleration;
    public float velPower;
    public float friction;

    [Space]
    [Header("Jump")]
    public float jumpForce;
    public float jumpCutMultiflier;
    public float coyoteTime;
    public float bufferTime;
    public float fallMultiflier;
    public float gravityScale;
    public float maxFallSpeed;

    public float coyoteTimer { get; private set; }
    public float bufferTimer { get; private set; }

    [Space]
    public float wallSlideSpeed;
    public float wallJumpForce;
    public Vector2 wallJumpRate;    // 벽점프 방향 비율

    [Space]
    [Header("Check")]
    public Vector2 groundSize;
    public Vector2 groundPos;
    public LayerMask groundLayer;
    public bool onGround { get; private set; }

    [Space]
    public Vector2 leftWallSize;
    public Vector2 leftWallPos;
    public bool onLeftWall { get; private set; }

    [Space]
    public Vector2 rightWallSize;
    public Vector2 rightWallPos;
    public bool onRightWall { get; private set; }

    public bool onLedge { get; private set; }
    public bool onWall { get; private set; }
    public float ledgeHeight;
    public float wallHeight;

    [Space]
    [Header("Squash and Stretch")]
    public float duration;
    public Vector2 squashRate;
    public Vector2 stretchRate;
    #endregion
    void Awake()
	{
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        render = GetComponent<SpriteRenderer>();
        fsm = new PlayerFSM(this);

        coyoteTimer = coyoteTime;
	}
	void Start()
    {
        OutLineShader = new OutLineShader(render, new MaterialPropertyBlock());
    }

    void Update()
    {
        collCheck();

        fsm.Update();
    }
    void FixedUpdate()
	{

        fsm.FixedUpdate();
	}

	private void LateUpdate()
	{
        OutLineShader.LateUpdate();
	}

	void OnMove(InputValue value)
	{
        inputVec = value.Get<Vector2>();
	}
    void OnJump(InputValue value)
	{
        jumpPress = value.isPressed;

        if (jumpPress) bufferTimer = bufferTime;
	}
    void OnLook(InputValue value)
	{
        mousePos = Camera.main.ScreenToWorldPoint(value.Get<Vector2>());
	}

    private bool check_right_click = false;
    [Space] public UnityEvent On_Right_Click;
    void OnRightClick(InputValue value)
    {
        if (!value.isPressed)
        {
            check_right_click = false;
            return;
        }
        if (value.isPressed&&check_right_click) return;
        check_right_click = true;
        On_Right_Click.Invoke();
    }

    private void collCheck()
    {
        onGround = Physics2D.OverlapBox((Vector2)transform.position + groundPos, groundSize, 0f, groundLayer);
        onLeftWall = Physics2D.OverlapBox((Vector2)transform.position + leftWallPos, leftWallSize, 0f, groundLayer);
        onRightWall = Physics2D.OverlapBox((Vector2)transform.position + rightWallPos, rightWallSize, 0f, groundLayer);

        onLedge = Physics2D.Raycast((Vector2)transform.position + new Vector2(1, ledgeHeight), Vector2.left, 2f, groundLayer);
        onWall = Physics2D.Raycast((Vector2)transform.position + new Vector2(1, wallHeight), Vector2.left, 2f, groundLayer);

        if (onGround) coyoteTimer = coyoteTime;
        else if (!onGround && coyoteTimer >= 0)
        {
            coyoteTimer-=Time.deltaTime;
        }

		if (bufferTimer >= 0)
		{
            bufferTimer -= Time.deltaTime;
		}
    }
    public void ChangeState(string key)
	{
        fsm.ChangeState(key);
	}

	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube((Vector2)transform.position + groundPos, groundSize);
        Gizmos.DrawWireCube((Vector2)transform.position + leftWallPos, leftWallSize);
        Gizmos.DrawWireCube((Vector2)transform.position + rightWallPos, rightWallSize);

        Gizmos.DrawRay((Vector2)transform.position + new Vector2(1f, ledgeHeight), Vector2.left * 2f);
        Gizmos.DrawRay((Vector2)transform.position + new Vector2(1f, wallHeight), Vector2.left * 2f);
    }
}
