using System;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<IAmEquipable> m_equipables = new List<IAmEquipable>();
    private List<Buff> m_buffs = new List<Buff>();

    private Weapon m_equipedWeapon;

    public Weapon EquipedWeapon => m_equipedWeapon;

    /// <summary>
    /// returns Item in Backpack
    /// </summary>
    /// <param name="_index">index of Item</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public IAmEquipable GetEquipable(int _index)
    {
        if (_index > m_equipables.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_equipables[_index];
    }

    public void RemoveEquipable(IAmEquipable _equipable)
    {
        m_equipables.Remove(_equipable);
    }

    public void RemoveEquipable(int _index)
    {
        m_equipables.RemoveAt(_index);
    }

    public void AddEquipable(IAmEquipable _equipable)
    {
        m_equipables.Add(_equipable);
    }


    /// <summary>
    /// returns Buff in Backpack
    /// </summary>
    /// <param name="_index">index of Buff</param>
    /// <returns></returns>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public Buff GetBuff(int _index)
    {
        if (_index > m_equipables.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_buffs[_index];
    }

    public IAmEquipable[] GetWeapons()
    {
        var weapons = m_equipables.FindAll((IAmEquipable q) => q is Weapon);

        return weapons.ToArray();
    }

    public void EquipWeapon(Weapon _weapon)
    {
        m_equipedWeapon = _weapon;
    }

    public void UnEquipWeapon()
    {
        m_equipedWeapon = null;
    }

}
