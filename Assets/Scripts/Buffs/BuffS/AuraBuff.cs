using Unity.VisualScripting;
using UnityEngine;

public class AuraBuff : Buff
{
    private int m_damage;
    private int m_speed;
    private float m_radius;
    public int Damage => m_damage;
    public int Speed => m_speed;
    public float Radius => m_radius;


    private Transform m_player;

    public AuraBuff(Transform _player, BuffStorage _buffStorage)
    {
        
        if (_buffStorage.TypeIsInList(this))    
        {
            var auraBuff = _buffStorage.GetBuffByType<AuraBuff>();
            _buffStorage.RemoveBuff(auraBuff);
            m_name = "Steppe durch die city,";
            m_description = "mit Aura!";
            m_damage = auraBuff.Damage + 2;
            m_speed = auraBuff.m_speed + 50;

            if (auraBuff.m_radius <= 5)
                m_radius = auraBuff.m_radius + 0.25f;
            else
                m_radius = auraBuff.m_radius;

            m_player = _player;
            m_price = auraBuff.m_price + 2;
            return;
        }

        m_name = "Meine handbag?";
        m_description = "Die is Aura!";
        m_speed = 300;
        m_damage = 5;
        m_radius = 2.5f;
        m_player = _player;
        m_price = 20;
    }

    public AuraBuff()
    {
    }

    public override void Effect()
    {
        Aura aura = GameObject.Instantiate(Resources.Load("Aura"), m_player.position, Quaternion.identity).GetComponent<Aura>();
        aura.m_player = m_player;
        aura.AuraBuff = this;
    }
}
