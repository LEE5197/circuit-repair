using UnityEngine;
using System.Collections;
public class IdleState : State<Controller>
{
	public IdleState(Controller _target, string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}

	public override void Enter()
	{
		target.anim.Play(anim);
	}

	public override void Update()
	{
		if (target.onGround && target.inputVec.x != 0)
		{
			target.ChangeState("Run");
			return;
		}
		if (target.coyoteTimer > 0 && target.bufferTimer > 0)
		{
			target.ChangeState("Jump");
			return;
		}
		if (target.rigid.linearVelocityY < 0 && !target.onGround)
		{
			target.ChangeState("Fall");
			return;
		}
	}

	public override void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		float targetSpeed = target.inputVec.x * target.maxSpeed;
		float speedDif = targetSpeed - target.rigid.linearVelocityX;
		float force = Mathf.Pow(Mathf.Abs(speedDif) * target.decceleration, target.velPower) * Mathf.Sign(speedDif);

		target.rigid.AddForceX(force);
	}
}

public class RunState : State<Controller>
{
	public RunState(Controller _target, string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}

	public override void Enter()
	{
		target.anim.Play(anim);
	}
	public override void Update()
	{
		if (target.onGround && target.inputVec.x == 0)
		{
			target.ChangeState("Idle");
			return;
		}
		if (target.coyoteTimer > 0 && target.bufferTimer > 0)
		{
			target.ChangeState("Jump");
			return;
		}
		if (target.rigid.linearVelocityY < 0 && !target.onGround) {
			target.ChangeState("Fall");
			return;
		}

		if (target.inputVec.x == 1) target.render.flipX = false;
		else if (target.inputVec.x == -1) target.render.flipX = true;
	}
	public override void FixedUpdate()
	{
		Move();
	}
	private void Move()
	{
		float targetSpeed = target.inputVec.x * target.maxSpeed;
		float speedDif = targetSpeed - target.rigid.linearVelocityX;
		float force = Mathf.Pow(Mathf.Abs(speedDif) * target.acceleration, target.velPower) * Mathf.Sign(speedDif);

		target.rigid.AddForceX(force);
	}
}

public class JumpState : State<Controller>
{
	private float jumpTime;
	private bool jumpCut;
	public JumpState(Controller _target,string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}

	public override void Enter()
	{
		jumpTime = 0.1f;
		target.anim.Play(anim);
		target.rigid.AddForceY(target.jumpForce, ForceMode2D.Impulse);

		jumpCut = false;
		target.rigid.gravityScale = target.gravityScale;

		target.StartCoroutine(Stretch());
	}

	public override void Update()
	{
		if (jumpTime >= 0) jumpTime -= Time.deltaTime;

		if (jumpTime < 0 && target.onGround)
		{
			target.ChangeState("Idle");
			return;
		}
		if (target.rigid.linearVelocityY < 0)
		{
			target.ChangeState("Fall");
			return;
		}

		if (target.onRightWall && target.inputVec.x == 1)
		{
			target.ChangeState("WallSlide");
			return;
		}
		else if (target.onLeftWall && target.inputVec.x == -1)
		{
			target.ChangeState("WallSlide");
			return;
		}

		if (!jumpCut && !target.jumpPress)
		{
			jumpCut = true;
			target.rigid.AddForceY(-target.rigid.linearVelocityY * (1 - target.jumpCutMultiflier), ForceMode2D.Impulse);
		}
	}
	public override void FixedUpdate()
	{
		Move();
	}
	public override void Exit()
	{
		target.StopCoroutine(Stretch());
		target.transform.localScale = new Vector3(1f, 1f, 1f);
	}
	private void Move()
	{
		float targetSpeed = target.inputVec.x * target.maxSpeed;
		float speedDif = targetSpeed - target.rigid.linearVelocityX;
		float force = Mathf.Pow(Mathf.Abs(speedDif) * target.acceleration, target.velPower) * Mathf.Sign(speedDif);

		if (Mathf.Sign(target.inputVec.x) == Mathf.Sign(target.rigid.linearVelocityX) && target.rigid.linearVelocityX > target.maxSpeed) return;
		target.rigid.AddForceX(force);
	}

	IEnumerator Stretch()
	{
		float duration = target.duration;
		float rateX = target.stretchRate.x - target.transform.localScale.x;
		float rateY = target.stretchRate.y - target.transform.localScale.y;
		float deltaX = rateX / duration;
		float deltaY = rateY / duration;

		Vector2 scale = target.transform.localScale;

		// stretch
		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		// return origin
		duration = target.duration;
		rateX = 1f - target.transform.localScale.x;
		rateY = 1f - target.transform.localScale.y;
		deltaX = rateX / duration;
		deltaY = rateY / duration;
		
		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		target.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}

public class FallState : State<Controller>
{
	public FallState(Controller _target,string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}

	public override void Enter()
	{
		target.anim.Play(anim);
		target.rigid.gravityScale = target.gravityScale * target.fallMultiflier;
	}
	public override void Update()
	{
		if (target.onGround)
		{
			target.ChangeState("Idle");
			target.StartCoroutine(Squash());
			return;
		}
		else if (!target.onLedge && target.onWall && target.onRightWall && target.inputVec.x == 1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}
		else if (!target.onLedge && !target.onWall && target.onRightWall && target.inputVec.x == 1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}
		if (!target.onLedge && target.onWall && target.onLeftWall && target.inputVec.x == -1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}
		else if (!target.onLedge && !target.onWall && target.onLeftWall && target.inputVec.x == -1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}

		if (target.onRightWall && target.inputVec.x == 1)
		{
			target.ChangeState("WallSlide"); return;
		}
		else if (target.onLeftWall && target.inputVec.x == -1) { 
			target.ChangeState("WallSlide");
			return;
		}
	}
	public override void FixedUpdate()
	{
		Move();
		if (-target.rigid.linearVelocityY > target.maxFallSpeed) target.rigid.linearVelocityY = -target.maxFallSpeed;
	}
	public override void Exit()
	{
		target.rigid.gravityScale = target.gravityScale;
	}
	private void Move()
	{
		float targetSpeed = target.inputVec.x * target.maxSpeed;
		float speedDif = targetSpeed - target.rigid.linearVelocityX;
		float force = Mathf.Pow(Mathf.Abs(speedDif) * target.acceleration, target.velPower) * Mathf.Sign(speedDif);

		target.rigid.AddForceX(force);
	}
	IEnumerator Squash()
	{
		float duration = target.duration;
		float rateX = target.squashRate.x - target.transform.localScale.x;
		float rateY = target.squashRate.y - target.transform.localScale.y;
		float deltaX = rateX / duration;
		float deltaY = rateY / duration;

		Vector2 scale = target.transform.localScale;

		// stretch
		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		// return origin
		duration = target.duration;
		rateX = 1f - target.transform.localScale.x;
		rateY = 1f - target.transform.localScale.y;
		deltaX = rateX / duration;
		deltaY = rateY / duration;

		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		target.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}

public class WallSlideState : State<Controller>
{
	float rightInputTime;
	float leftInputTime;
	public WallSlideState(Controller _target, string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}

	public override void Enter()
	{
		target.anim.Play(anim);
		target.render.flipX = target.onRightWall ? false : true;
		float force = target.render.flipX ? -10 : 10;
		target.rigid.linearVelocity = Vector2.zero;
		target.rigid.AddForceX(force, ForceMode2D.Impulse);

		rightInputTime = leftInputTime = 0f;
	}

	public override void Update()
	{
		if (target.render.flipX && rightInputTime > 0.2)
		{
			target.render.flipX = false;
			target.ChangeState("Idle");
			return;
		}
		else if (!target.render.flipX && leftInputTime > 0.2)
		{
			target.render.flipX = true;
			target.ChangeState("Idle");
			return;
		}

		if (target.render.flipX && !target.onLeftWall)
		{
			target.ChangeState("Idle");
			return;
		}
		else if (!target.render.flipX && !target.onRightWall)
		{
			target.ChangeState("Idle");
			return;
		}

		if (target.bufferTimer > 0)
		{
			target.ChangeState("WallJump");
			return;
		}

		if (!target.onLedge && target.onWall && target.onRightWall && target.inputVec.x == 1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}
		else if (!target.onLedge && !target.onWall && target.onRightWall && target.inputVec.x == 1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}
		if (!target.onLedge && target.onWall && target.onLeftWall && target.inputVec.x == -1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}
		else if (!target.onLedge && !target.onWall && target.onLeftWall && target.inputVec.x == -1)
		{
			target.ChangeState("LedgeGrab");
			return;
		}

		if (target.inputVec.x == 1) rightInputTime += Time.deltaTime;
		else rightInputTime = 0f;
		if (target.inputVec.x == -1) leftInputTime += Time.deltaTime;
		else leftInputTime = 0f;
	}

	public override void FixedUpdate()
	{
		if (target.inputVec.y == -1) target.rigid.linearVelocityY = -target.wallSlideSpeed * 5;
		else if (-target.rigid.linearVelocityY > target.wallSlideSpeed) target.rigid.linearVelocityY = -target.wallSlideSpeed;
	}
}

public class WallJumpState : State<Controller>
{
	private float jumpTime;
	private bool jumpCut;
	public WallJumpState(Controller _target, string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}
	public override void Enter()
	{
		jumpTime = 0.1f;
		target.anim.Play(anim);
		Vector2 jumpDir = target.wallJumpRate.normalized;
		if (!target.render.flipX) jumpDir.x = -jumpDir.x;
		target.rigid.AddForce(jumpDir * target.wallJumpForce, ForceMode2D.Impulse);

		jumpCut = false;
		target.rigid.gravityScale = target.gravityScale;
		target.render.flipX = !target.render.flipX;

		target.StartCoroutine(Stretch());
	}

	public override void Update()
	{
		if (jumpTime >= 0)
		{
			jumpTime -= Time.deltaTime;
			return;
		}

		if (jumpTime < 0 && target.onGround)
		{
			target.ChangeState("Idle");
			return;
		}
		if (target.rigid.linearVelocityY < 0)
		{
			target.ChangeState("Fall");
			return;
		}
		if (target.onRightWall && target.inputVec.x == 1)
		{
			target.ChangeState("WallSlide");
			return;
		}
		else if (target.onLeftWall && target.inputVec.x == -1)
		{
			target.ChangeState("WallSlide");
			return;
		}

		if (!jumpCut && !target.jumpPress)
		{
			jumpCut = true;
			target.rigid.AddForceY(-target.rigid.linearVelocityY * (1 - target.jumpCutMultiflier), ForceMode2D.Impulse);
		}
	}
	public override void FixedUpdate()
	{
		if (jumpTime > 0 && target.inputVec.x * target.rigid.linearVelocityX < 0) return;
		Move();
	}
	public override void Exit()
	{
		target.StopCoroutine(Stretch());
		target.transform.localScale = new Vector3(1f, 1f, 1f);
	}
	private void Move()
	{
		float targetSpeed = target.inputVec.x * target.maxSpeed;
		float speedDif = targetSpeed - target.rigid.linearVelocityX;
		float force = Mathf.Pow(Mathf.Abs(speedDif) * target.acceleration, target.velPower) * Mathf.Sign(speedDif);

		target.rigid.AddForceX(force);
	}
	IEnumerator Stretch()
	{
		float duration = target.duration;
		float rateX = target.stretchRate.x - target.transform.localScale.x;
		float rateY = target.stretchRate.y - target.transform.localScale.y;
		float deltaX = rateX / duration;
		float deltaY = rateY / duration;

		Vector2 scale = target.transform.localScale;

		// stretch
		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		// return origin
		duration = target.duration;
		rateX = 1f - target.transform.localScale.x;
		rateY = 1f - target.transform.localScale.y;
		deltaX = rateX / duration;
		deltaY = rateY / duration;

		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		target.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}

public class LedgeGrabState : State<Controller>
{
	float rightInputTime;
	float leftInputTime;
	public LedgeGrabState(Controller _target,string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}

	public override void Enter()
	{
		target.anim.Play(anim);
		target.rigid.gravityScale = 0f;
		target.rigid.linearVelocity = Vector2.zero;

		target.render.flipX = target.onRightWall ? false : true;
		float force = target.render.flipX ? -10 : 10;
		target.rigid.linearVelocity = Vector2.zero;
		target.rigid.AddForceX(force, ForceMode2D.Impulse);

		rightInputTime = 0f;
		leftInputTime = 0f;
	}

	public override void Update()
	{
		if (!target.onLedge && !target.onWall)
		{
			SlideDown();
			return;
		}
		else if (!target.onLedge && target.onWall)
		{
			target.rigid.linearVelocityY = 0;
		}

		if (target.render.flipX && target.inputVec.x == 1 && target.bufferTimer > 0f) {
			target.ChangeState("WallJump");
			return; 
		}
		if (!target.render.flipX && target.inputVec.x == -1 && target.bufferTimer > 0f)
		{
			target.ChangeState("WallJump");
			return;
		}

		if (target.render.flipX && rightInputTime > 0.2f)
		{
			target.ChangeState("Idle");
			return;
		}
		if (!target.render.flipX && leftInputTime > 0.2f)
		{
			target.ChangeState("Idle");
			return;
		}

		if (target.bufferTimer > 0)
		{
			target.ChangeState("LedgeGrabJump");
			return;
		}

		if (!target.render.flipX && !target.onRightWall)
		{
			target.ChangeState("Idle");
			return;
		}
		if (target.render.flipX && !target.onLeftWall)
		{
			target.ChangeState("Idle");
			return;
		}


		if (target.inputVec.x == 1) rightInputTime += Time.deltaTime;
		else rightInputTime = 0f;
		if (target.inputVec.x == -1) leftInputTime += Time.deltaTime;
		else leftInputTime = 0f;
	}

	public override void Exit()
	{
		target.rigid.gravityScale = target.gravityScale;
	}

	void SlideDown()
	{
		target.rigid.AddForceY(-3f);
	}
}

public class LedgeGrabJump : State<Controller>
{
	float minimumCangeTime;
	public LedgeGrabJump(Controller _target, string _anim) : base(_target)
	{
		anim = Animator.StringToHash(_anim);
	}
	public override void Enter()
	{
		target.anim.Play(anim);
		target.rigid.gravityScale = target.gravityScale;
		target.rigid.AddForceY(10f, ForceMode2D.Impulse);

		target.StartCoroutine(Stretch());

		minimumCangeTime = 0.2f;
	}

	public override void Update()
	{
		if (target.onGround && minimumCangeTime < 0f) target.ChangeState("Idle");
		if (target.rigid.linearVelocityY < 0) target.ChangeState("Fall");

		if (minimumCangeTime >= 0f) minimumCangeTime -= Time.deltaTime;
	}

	public override void FixedUpdate()
	{
		Move();
	}
	public override void Exit()
	{
		target.StopCoroutine(Stretch());
		target.transform.localScale = new Vector3(1f, 1f, 1f);
	}
	private void Move()
	{
		float targetSpeed = target.inputVec.x * target.maxSpeed;
		float speedDif = targetSpeed - target.rigid.linearVelocityX;
		float force = Mathf.Pow(Mathf.Abs(speedDif) * target.acceleration, target.velPower) * Mathf.Sign(speedDif);

		target.rigid.AddForceX(force);
	}
	IEnumerator Stretch()
	{
		float duration = target.duration;
		float rateX = target.stretchRate.x - target.transform.localScale.x;
		float rateY = target.stretchRate.y - target.transform.localScale.y;
		float deltaX = rateX / duration;
		float deltaY = rateY / duration;

		Vector2 scale = target.transform.localScale;

		// stretch
		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		// return origin
		duration = target.duration;
		rateX = 1f - target.transform.localScale.x;
		rateY = 1f - target.transform.localScale.y;
		deltaX = rateX / duration;
		deltaY = rateY / duration;

		while (duration >= 0f)
		{
			duration -= Time.deltaTime;
			scale.x += deltaX * Time.deltaTime;
			scale.y += deltaY * Time.deltaTime;
			target.transform.localScale = scale;
			yield return null;
		}

		target.transform.localScale = new Vector3(1f, 1f, 1f);
	}
}