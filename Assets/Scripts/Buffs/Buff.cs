using UnityEngine;

public abstract class Buff : MonoBehaviour
{
    protected string m_name;
    protected string m_description;
    protected int m_price;

    public int Price => m_price;
    public string Name => m_name;
    public string Description => m_description;

    public abstract void Effect();
}