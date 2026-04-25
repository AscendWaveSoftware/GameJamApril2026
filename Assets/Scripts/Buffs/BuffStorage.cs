using System;
using System.Collections.Generic;
using UnityEngine;

public class BuffStorage : MonoBehaviour
{
    private List<Buff> m_buffs = new List<Buff>();

    public Buff[] Buffs => m_buffs.ToArray();

    public void RemoveBuff(int _index)
    {
        if (_index < m_buffs.Count)
            throw new IndexOutOfRangeException("index out of range");
        m_buffs.RemoveAt(_index);
    }

    public void RemoveBuff(Buff _buff)
    {
        if (IsInList(_buff))
            m_buffs.Remove(_buff);
    }

    public void AddBuff(Buff _buff)
    {
        m_buffs.Add(_buff);
    }

    private bool IsInList(Buff _buff)
    {
        foreach (Buff buff in m_buffs)
        {
            if (buff == _buff)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// returns Buff in Backpack
    /// </summary>
    /// <param name="_index">index of Buff</param>
    /// <returns>Buff</returns>
    public Buff GetBuff(int _index)
    {
        if (_index < m_buffs.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_buffs[_index];
    }
}
