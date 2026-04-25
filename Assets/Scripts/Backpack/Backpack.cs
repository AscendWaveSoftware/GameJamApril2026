using System;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<IAmEquipable> m_Equipable = new List<IAmEquipable>();
    private List<Buff> m_buffs = new List<Buff>();

    /// <summary>
    /// returns Item in Backpack
    /// </summary>
    /// <param name="_index">index of Item</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public IAmEquipable GetEquipable(int _index)
    {
        if (_index > m_Equipable.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_Equipable[_index];
    }

    public void AddEquipable(IAmEquipable _item)
    {
        m_Equipable.Add(_item);
    }


    /// <summary>
    /// returns Buff in Backpack
    /// </summary>
    /// <param name="_index">index of Buff</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public Buff GetBuff(int _index)
    {
        if (_index > m_Equipable.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_buffs[_index];
    }

}
