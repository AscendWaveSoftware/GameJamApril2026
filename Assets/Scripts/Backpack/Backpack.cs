using System;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<Item> m_items = new List<Item>();
    private List<Buff> m_buffs = new List<Buff>();

    /// <summary>
    /// returns Item in Backpack
    /// </summary>
    /// <param name="_index">index of Item</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public Item GetItem(int _index)
    {
        if (_index > m_items.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_items[_index];
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
