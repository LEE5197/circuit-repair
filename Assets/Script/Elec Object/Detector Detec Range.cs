using System.Collections.Generic;
using UnityEngine;

public class DetectorDetecRange : MonoBehaviour
{
    private LineRenderer line;
    private PolygonCollider2D coll;
    public bool playerIsIn { get; private set; } = false;

    private void Awake()
    {
        line = GetComponent<LineRenderer>();
        coll = GetComponent<PolygonCollider2D>();

        line.useWorldSpace = false;
        line.loop = true;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = coll.points.Length;
    }

    private void Update()
    {
        DrawLine();
    }

    private void DrawLine()
    {
        Vector2[] points = coll.points;

        for (int i = 0; i < points.Length; i++)
        {
            line.SetPosition(i, points[i]);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsIn = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerIsIn)
        {
            playerIsIn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerIsIn = false;
        }
    }

    private bool CheckPlayer(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !playerIsIn)
        {
            Vector2 dir = collision.gameObject.transform.position - transform.position;
            dir = dir.normalized;
            float distance = Vector2.SqrMagnitude(collision.gameObject.transform.position - transform.position);
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance);

            if (hit.collider == null) return false;
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                playerIsIn = true;
                return true;
            }
        }
        return false;
    }
}
