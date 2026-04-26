using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuffSelectionHandler : MonoBehaviour
{
    [SerializeField] private static int Buffs;
    GameObject[] buffSelectorWindow = new GameObject[Buffs];
    Buff[] buffs = new Buff[Buffs];

    void Start()
    {
        List<Buff> buffbuff = new List<Buff> { new AuraBuff(), new AddDamage(), new AddMaxHealth() };

        for (int i = 0; i < buffs.Length; i++)
        {
            var theChosenBuff = buffbuff[UnityEngine.Random.Range(0, buffbuff.Count)];
            buffs[i] = theChosenBuff;

            buffbuff.Remove(theChosenBuff);

        }

        for (int i = 0; i < buffSelectorWindow.Length; i++)
        {
            buffSelectorWindow[i] = (GameObject)GameObject.Instantiate(Resources.Load("Buff"), new Vector3(0, 0, 0), Quaternion.identity);
            buffSelectorWindow[i].transform.GetChild(1).GetComponent<TextMeshPro>().text = buffs[i].Name;
            buffSelectorWindow[i].transform.GetChild(2).GetComponent<TextMeshPro>().text = buffs[i].Description;
            buffSelectorWindow[i].transform.GetChild(1).GetComponent<TextMeshPro>().text = "Preis: " + buffs[i].Price;
        }
    }

    [Obsolete]
    private bool IsThereAlreadyTheBuff(Buff _buff)
    {
        foreach (Buff buff in buffs)
        {
            if (_buff == buff)
            {
                return true;
            }
        }
        return false;
    }
}
