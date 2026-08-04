using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.VisualScripting;

public class ElecLineManager : MonoBehaviour
{
    public List<ElecLine> elecLines;
    private ElecLine curLine;
    public float allowDistance;

    private void Awake()
    {
        curLine = null;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            MouseDown();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && curLine != null)
        {
            MouseUp();
        }
        else if (curLine != null)
        {
            MouseDrag();
        }
    }
    private void MouseDown()
    {
        float minDist = 10f;
        foreach(var line in elecLines)
        {
            float dist = line.GetDistance();
            if (dist < allowDistance && dist < minDist)
            {
                minDist = dist;
                curLine = line;
            }
        }
        if (curLine == null) return;

        Vector2[] point = curLine.GetNearPoint();
        curLine.updatePos(GetMousePos());
    }
    private void MouseDrag()
    {
        if (curLine == null) return;

        curLine.updatePos(GetMousePos());
    }
    private void MouseUp()
    {
        if (curLine == null) return;

        Vector2 mousePos = GetMousePos();
        Collider2D[] hit = Physics2D.OverlapPointAll(mousePos);

        foreach(var it in hit)
        {
            if (it != null && it.gameObject != gameObject && it.CompareTag("Elec"))
            {
                curLine.UpdateTarget(it.gameObject);
                break;
            }
        }

        if (curLine != null)
        {
            curLine.Cancel();
        }

        curLine = null;
    }

    private Vector2 GetMousePos()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
