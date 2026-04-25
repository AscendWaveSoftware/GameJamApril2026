using System;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    private List<IAmEquipable> m_equipables = new List<IAmEquipable>();
    private List<Buff> m_buffs = new List<Buff>();

    private Weapon m_equipedWeapon;
    private Armor m_equipedArmor;

    public Weapon EquipedWeapon => m_equipedWeapon;
    public Armor EquipedArmor => m_equipedArmor;
    

    /// <summary>
    /// returns equipables in Backpack
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

    /// <summary>
    /// Removes Equipable by item from Backpack
    /// </summary>
    /// <param name="_equipable">Item to remove</param>
    public void RemoveEquipable(IAmEquipable _equipable)
    {
        m_equipables.Remove(_equipable);
    }

    /// <summary>
    /// Removes Equipable by index from Backpack
    /// </summary>
    /// <param name="_index"></param>
    public void RemoveEquipable(int _index)
    {
        if (_index < m_equipables.Count)
            throw new IndexOutOfRangeException("index out of range");
        m_equipables.RemoveAt(_index);
    }

    /// <summary>
    /// Adds Equipable to Backpack
    /// </summary>
    /// <param name="_equipable">the item to equip</param>
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
        if (_index < m_equipables.Count)
            throw new IndexOutOfRangeException("_index out of range");
        return m_buffs[_index];
    }

    /// <summary>
    /// Returns all Weapons in List
    /// </summary>
    /// <returns></returns>
    public IAmEquipable[] GetWeapons()
    {
        var weapons = m_equipables.FindAll((IAmEquipable q) => q is Weapon);

        return weapons.ToArray();
    }

    /// <summary>
    /// Equip weapon
    /// </summary>
    /// <param name="_weapon">The Weapon to Equip</param>
    public void EquipWeapon(Weapon _weapon)
    {
        bool weaponIsInList = false;
        foreach (IAmEquipable item in m_equipables)
        {
            if (item == _weapon)
            {
                weaponIsInList = true;
                break;
            }
        }
        if (weaponIsInList)
            m_equipedWeapon = _weapon;
        else
            throw new InvalidOperationException(_weapon + "is not in List");
    }

    /// <summary>
    /// sets equipedWeapon null
    /// </summary>
    public void UnequipWeapn()
    {
        m_equipedWeapon = null;
    }

    /// <summary>
    /// Returns all Armors in List
    /// </summary>
    /// <returns>IAmEquipable[]</returns>
    public IAmEquipable[] GetArmor()
    {
        var armors = m_equipables.FindAll((IAmEquipable q) => q is Armor);

        return armors.ToArray();
    }


    public void EquipArmor(Armor _armor)
    {
        bool ArmorIsInList = false;
        foreach (IAmEquipable item in m_equipables)
        {
            if (item == _armor)
            {
                ArmorIsInList = true;
                break;
            }
        }
        if (ArmorIsInList)
            m_equipedArmor = _armor;
        else
            throw new InvalidOperationException(_armor + "is not in List");
    }

    public void UnequipArmor()
    {
        m_equipedArmor = null;
    }

}
