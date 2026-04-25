using UnityEngine;

public abstract class Buff
{
    protected BuffStorage m_bs;


    protected string m_name;
    protected string m_description;
    protected int m_price;

    public int Price => m_price;
    public string Name => m_name;
    public string Description => m_description;

    protected int CalcAddition(int[] _additionRange)
    {
        int addition = 0;

        addition = Random.Range(_additionRange[0], _additionRange[1]);

        return addition;
    }

    public abstract void Effect();
}