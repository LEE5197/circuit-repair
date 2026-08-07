
public interface IPower
{
    public void Connected(IPower target);
    public void DisConnected(IPower target);
    public void SetPower(float power);
}
