using UnityEngine;

public class ForceField : MonoBehaviour, IPower
{
    private float current_poser = 0f;
    public void Connected(IPower target) { }
    public void DisConnected(IPower target) { }
    public void SetPower(float power)
    {
        current_poser += power;
        if (current_poser < 0) current_poser = 0;
    }

    public AnimationCurve power_curve;
    public float field_force = 10f;
    public GameObject force_feild_range;
    
    public float maxDistance { get; private set; }

    public Vector2 force { get; private set; }

    private void Awake()
    {
        force = transform.up * field_force;
        maxDistance = GetComponent<BoxCollider2D>().size.y;
    }
    private void Update()
    {
        if (current_poser <= 0f) force_feild_range?.SetActive(false);
        else force_feild_range?.SetActive(true);
    }
}
