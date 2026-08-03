using UnityEngine;

public class PlayerCam : MonoBehaviour
{
	private Controller player;
	public float boundary = 3f;

	private void Start()
	{
		player = GetComponentInParent<Controller>();
	}
	private void Update()
	{
		UpdateTargetPos();
	}

	void UpdateTargetPos()
	{
		Vector2 pos;
		float posDif = ((Vector2)player.transform.position - player.mousePos).sqrMagnitude;

		if (posDif > boundary * boundary)
		{
			Vector2 dir = player.mousePos - (Vector2)player.transform.position;
			pos = dir.normalized * boundary + (Vector2)player.transform.position;
		}
		else
		{
			pos = player.mousePos;
		}

		transform.position = pos;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, 0.5f);
	}
}