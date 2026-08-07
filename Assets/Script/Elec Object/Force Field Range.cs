using UnityEngine;
using System.Collections.Generic;

public class ForceFieldRange : MonoBehaviour
{
    public ForceField parent;
    public Controller player;

    private void Start()
    {
        if (parent == null)
            parent = GetComponentInParent<ForceField>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 force = parent.force;

        if(collision.TryGetComponent<Rigidbody2D>(out var rigid))
        {
            if (player == null && collision.CompareTag("Player")) player = collision.GetComponent<Controller>();
            
            float distance = Vector2.Distance((Vector2)parent.transform.position, rigid.position);
            float maxDistance = parent.maxDistance;
            if (maxDistance == 0) maxDistance = 1;

            force *= parent.power_curve.Evaluate(distance / maxDistance);

            if (collision.CompareTag("Player"))
            {
                player.rigid.AddForce(force);
            }
            else
            {
                
                rigid.AddForce(force);
            }
        }
    }
}
