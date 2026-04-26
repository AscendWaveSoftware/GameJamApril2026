using Player;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuffSelectionHandler : MonoBehaviour
{
    public int Buffs;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject uicamera;
    [SerializeField] private GameObject canvas;
    GameObject[] buffSelectorWindow;
    Buff[] buffs;

    void OnEnable()
    {
        player.SetActive(false);
        uicamera.SetActive(true);
        camera.SetActive(false);

        buffSelectorWindow = new GameObject[Buffs];
        buffs = new Buff[Buffs];

        List<Buff> buffbuff = new List<Buff> { 
            new AuraBuff(player.transform, player.GetComponent<PlayerEquipmentHandler>().Backpack.BuffStorage),
            new AddDamage(player.GetComponent<PlayerAttackHandler>()),
            new AddMaxHealth(player.GetComponent<PlayerHealth>()) 
        };

        for (int i = 0; i < buffs.Length; i++)
        {
            var theChosenBuff = buffbuff[UnityEngine.Random.Range(0, buffbuff.Count)];
            buffs[i] = theChosenBuff;
            
            buffbuff.Remove(theChosenBuff);
        }

        for (int i = 0; i < buffSelectorWindow.Length; i++)
        {
            buffSelectorWindow[i] = (GameObject)GameObject.Instantiate(Resources.Load("Buff"), new Vector3(0, 0, 0), Quaternion.identity);

            Transform buffTransform = buffSelectorWindow[i].transform;

            buffTransform.parent = transform;
            buffTransform.localScale = new Vector3(8.7608f, 8.7608f, 8.7608f);
            buffTransform.position = new Vector3(0, 0, 0);

            buffTransform.name = i.ToString();

            buffTransform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buffs[i].Name;
            buffTransform.GetChild(2).GetComponent<TextMeshProUGUI>().text = buffs[i].Description;
            buffTransform.GetChild(3).GetComponent<TextMeshProUGUI>().text = "Preis: " + buffs[i].Price;

            int x = new int();
            x = i;
            buffTransform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => OnButtonClick(buffs[x]));
        }
    }

    public void OnButtonClick(Buff _buff)
    {
        PlayerGold playerGold = player.GetComponent<PlayerGold>();
        canvas = transform.parent.GameObject();
        if (playerGold.CurrentGold < _buff.Price)
        {
            return;
        }

        if (playerGold.TrySpendGold(_buff.Price))
        {
            player.GetComponent<PlayerEquipmentHandler>().Backpack.BuffStorage.AddBuff(_buff);
            canvas.SetActive(false);
            player.SetActive(true);
            camera.SetActive(true);
            uicamera.SetActive(false);
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
