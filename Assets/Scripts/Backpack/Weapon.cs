public abstract class Weapon : IAmEquipable
{
    protected float m_fireRate;
    protected float m_damage;

    public float FireRate => m_fireRate;
    public float Damage => m_damage;

    protected Weapon(float _rate, float _damage)
    {
        m_fireRate = _rate;
        m_damage = _damage;
    }
}
