using Enemy;
using UnityEngine;
using UnityEngine.UIElements;

public class Aura : MonoBehaviour
{
    public Transform m_player;

    public int Speed;
    public float Radius;

    public AuraBuff AuraBuff;

    private float m_angle;
    Vector3 newPositon = new Vector3();

    private void Start()
    {
        m_player = FindObjectsByType<Player.PlayerMovementHandler>(FindObjectsSortMode.None)[0].transform;
        Radius = AuraBuff.Radius;
        Speed = AuraBuff.Speed;
    }

    void FixedUpdate()
    {
        CalcPosition();
        transform.position = m_player.position + newPositon;
    }

    private void CalcPosition()
    {
        m_angle += AuraBuff.Speed * Time.deltaTime;
        float x = Mathf.Cos(m_angle * Mathf.Deg2Rad) * AuraBuff.Radius;
        float z = Mathf.Sin(m_angle * Mathf.Deg2Rad) * AuraBuff.Radius;

        newPositon = new Vector3(x, m_player.position.y, z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<EnemyBase>())
            return;
        EnemyBase enemy = other.GetComponent<EnemyBase>();

        enemy.DamageEnemy(AuraBuff.Damage);
    }
}
