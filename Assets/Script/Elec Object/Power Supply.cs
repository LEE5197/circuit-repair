using UnityEngine;
using System.Collections.Generic;

public class PowerSupply : MonoBehaviour, IPower
{
    [SerializeField] private List<IPower> connected_target;
    [SerializeField] private float power;

    public void Connected(IPower target)
    {
        target.SetPower(power);
        connected_target.Add(target);
    }

    public void DisConnected(IPower target)
    {
        target.SetPower(-power);
        connected_target.Remove(target);
    }

    public void SetPower(float power) { }

    private void Awake()
    {
        connected_target = new List<IPower>();
    }
}
