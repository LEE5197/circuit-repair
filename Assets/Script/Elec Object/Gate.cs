using UnityEngine;

public class Gate : MonoBehaviour, IPower
{
    [SerializeField] private float required_power = 3f;
    public float current_power = 0f;
    public void Connected(IPower target) { }

    public void DisConnected(IPower target) { }

    public void SetPower(float power)
    {
        current_power += power;
        if (current_power < 0) current_power = 0f;

        if (current_power >= required_power) Debug.Log("Gate open");
        else if (current_power < required_power) Debug.Log("Need more power");
    }


    private void Awake()
    {

    }

    private void Update()
    {

    }

}
