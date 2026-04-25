using Player;

public class AddMaxHealth : Buff
{
    private PlayerHealth m_playerHeath;
    private float m_maxHealth;
    private int m_maxHealthAddition;

    private int[] additionRange = { 1, 5 };

    public AddMaxHealth(PlayerHealth _player)
    {
        m_name = "Menschen Blut";
        m_maxHealthAddition = CalcAddition(additionRange);
        m_price = CalcPrice();
        m_description = "Fügt dem Spieler " + m_maxHealthAddition + "HP hinzu.";
        m_playerHeath = _player;
        m_maxHealth = m_playerHeath.MaxHealth;
    }

    private int CalcPrice()
    {
        int price = 5;
        for (int i = 0; i < m_maxHealthAddition; i++)
        {
            price += 3;
        }
        return price;
    }

    public override void Effect()
    {
        m_playerHeath.ChangeMaxHealth(m_maxHealth + m_maxHealthAddition);
        m_bs.RemoveBuff(this);
    }
}
