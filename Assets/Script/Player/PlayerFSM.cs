
public class PlayerFSM
{
	private StateMachine<Controller> fsm;
	private Controller target;

	public PlayerFSM(Controller _target)
	{
		target = _target;
		fsm = new StateMachine<Controller>(_target);
		InitState();

		ChangeState("Idle");
	}

	private void InitState()
	{
		fsm.AddState("Idle", new IdleState(target, "Idle"));
		fsm.AddState("Run", new RunState(target, "Run"));
		fsm.AddState("Jump", new JumpState(target, "Jump"));
		fsm.AddState("Fall", new FallState(target, "Fall"));
		fsm.AddState("WallSlide", new WallSlideState(target, "WallSlide"));
		fsm.AddState("WallJump", new WallJumpState(target, "Jump"));
		fsm.AddState("LedgeGrab", new LedgeGrabState(target, "WallSlide"));
		fsm.AddState("LedgeGrabJump", new LedgeGrabJump(target, "Jump"));
	}
	public void ChangeState(string key)
	{
		fsm.ChangeState(key);
	}
	public void Update()
	{
		fsm.Update();
	}
	public void FixedUpdate()
	{
		fsm.FixedUpdate();
	}
}
