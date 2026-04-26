using Player;
using UnityEngine;

public class AddSpeed : Buff
{
    private PlayerMovementHandler m_pmh;
    private int[] additionRange = { 1, 2};
    private int newSpeed;

    public AddSpeed(PlayerMovementHandler _pmh)
    {
        m_pmh = _pmh;
        m_name = "Energy Drink";
        newSpeed = CalcAddition(additionRange);
        m_description = "Setzt Speed auf " + newSpeed;
        m_price = CalcPrice();
    }
    public override void Effect()
    {
        m_pmh.SetMovespeed(newSpeed);
        m_bs.RemoveBuff(this);
    }

    private int CalcPrice()
    {
        int price = 7;
        for (int i = 0; i < newSpeed; i++)
        {
            price += 2;
        }
        return price;
    }
}
