using Player;

public class AddDamage : Buff
{

    private PlayerAttackHandler m_pah;
    private float m_currentAttack;
    private int m_attackAddition;

    private int[] additionRange = { 1, 5 };

    public AddDamage()
    {
        m_name = "Protein Drink";
        m_attackAddition = CalcAddition(additionRange);
        m_price = CalcPrice();
        m_description = "Fügt dem Spieler " + m_attackAddition + "attack hinzu.";
        m_currentAttack = m_pah.Damage;
    }

    private int CalcPrice()
    {
        int price = 5;
        for (int i = 0; i < m_attackAddition; i++)
        {
            price += 3;
        }
        return price;
    }

    public override void Effect()
    {
        m_pah.ChangePlayerDamage(m_currentAttack + m_attackAddition);
        m_bs.RemoveBuff(this);
    }
}
