using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.UI.Image;

public class Detector : MonoBehaviour, IPower
{
    [SerializeField] private float current_power;
    private List<IPower> connected_target = new List<IPower>();
    public void Connected(IPower target)
    {
        connected_target.Add(target);
    }

    public void DisConnected(IPower target)
    {
        connected_target.Remove(target);
    }

    public void SetPower(float power)
    {
        current_power += power;
        if (current_power < 0) current_power = 0;

        foreach (var it in connected_target)
        {
            it.SetPower(power);
        }
    }

    private DetectorDetecRange range;
    private bool isActive = false;

    private void Awake()
    {
        range = GetComponentInChildren<DetectorDetecRange>();
    }

    private void Update()
    {
        if (range.playerIsIn && !isActive)
        {
            isActive = true;
            foreach(var it in connected_target)
            {
                it.SetPower(current_power);
            }
        }
        else if (!range.playerIsIn && isActive)
        {
            isActive = false;
            foreach(var it in connected_target)
            {
                it.SetPower(-current_power);
            }
        }
    }
}
