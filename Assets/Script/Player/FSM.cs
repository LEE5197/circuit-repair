using System.Collections.Generic;

public class FSM<T>
{
	private T target;
	private State<T> curState;
	private Dictionary<string, State<T>> stateDic;

	public FSM(T _target)
	{
		target = _target;
		stateDic = new Dictionary<string, State<T>> ();
	}

	public void AddState(string key, State<T> value)
	{
		stateDic.Add(key, value);
	}

	public void ChangeState(string key)
	{
		if (stateDic[key] == null) return;
		if (stateDic[key] == curState) return;

		curState?.Exit();
		curState = stateDic[key];
		curState?.Enter();
	}

	public void Update()
	{
		curState.Update();
	}
	public void FixedUpdate()
	{
		curState.FixedUpdate();
	}
}
