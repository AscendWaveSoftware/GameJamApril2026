using System;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<IAmEquipable> m_items = new List<IAmEquipable>();
    private List<Buff> m_buffs = new List<Buff>();

    /// <summary>
    /// returns Item in Backpack
    /// </summary>
    /// <param name="_index">index of Item</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public IAmEquipable GetItem(int _index)
    {
        if (_index > m_items.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_items[_index];
    }

    public void AddItem(IAmEquipable _item)
    {
        m_items.Add(_item);
        Debug.Log("COOL! NEUES ITEM!! " + _item);
    }

    /// <summary>
    /// returns Buff in Backpack
    /// </summary>
    /// <param name="_index">index of Buff</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public Buff GetBuff(int _index)
    {
        if (_index > m_items.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_buffs[_index];
    }

}
