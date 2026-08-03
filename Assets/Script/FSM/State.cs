using UnityEngine;

abstract public class State <T>
{
	protected T target;
	protected int anim;
	public State(T _target)
	{
		target = _target;
	}
	virtual public void Enter() { }
	virtual public void Update() { }
	virtual public void FixedUpdate() { }
	virtual public void Exit() { }
}