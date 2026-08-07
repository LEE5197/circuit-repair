using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PowerCableManager : MonoBehaviour
{
    public List<PowerCable> cables;
    private PowerCable current_cable;
    public float allowDistance=1f;

    private void Awake()
    {
        current_cable = null;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && current_cable == null)
        {
            MouseDown();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame && current_cable != null)
        {
            MouseUp();
        }
        else if (current_cable != null)
        {
            MouseDrag();
        }

        foreach(var it in cables)
        {
            if (it != current_cable)
            {
                it.UpdateCablePosition();
            }
        }
    }

    private void MouseDown()
    {
        Vector2 mousePos = GetMousePos();
        float minDistance = allowDistance;

        foreach(var cable in cables)
        {
            float distance = cable.GetDistance(mousePos);
            if (distance < minDistance && distance < allowDistance)
            {
                minDistance = distance;
                current_cable = cable;
            }
        }

        current_cable?.CheckNearPoint(mousePos);
        current_cable?.UpdateCablePosition(mousePos);
    }

    private void MouseUp()
    {
        Vector2 mousePos = GetMousePos();
        GameObject target = null;

        Collider2D[] hit = Physics2D.OverlapPointAll(mousePos);

        foreach (var it in hit)
        {
            if (it != null && it.gameObject != gameObject && it.CompareTag("Power"))
            {
                target = it.gameObject;
                break;
            }
        }
        foreach(var it in cables)
        {
            if (it.IsDuplicated(current_cable.start_obj,target))
            {
                target = null;
                break;
            }
        }

        current_cable.UpdateTargetObject(target);
        current_cable = null;
    }

    private void MouseDrag()
    {
        current_cable.UpdateCablePosition(GetMousePos());
    }

    private Vector2 GetMousePos()
    {
        if (Mouse.current == null) return Vector2.zero;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
