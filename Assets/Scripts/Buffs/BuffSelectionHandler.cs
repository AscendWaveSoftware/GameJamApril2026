using Player;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuffSelectionHandler : MonoBehaviour
{
    public int Buffs;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject camera;
    [SerializeField] private GameObject uicamera;
    [SerializeField] private GameObject statPrefab;
    [SerializeField] private GameObject canvas;
    GameObject[] buffSelectorWindow;
    Buff[] buffs;
    GameObject[] GO_stats;

    List<float> stats = new List<float>();
    List<string> statName = new List<string>();

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

        stats.Add(player.GetComponent<PlayerGold>().CurrentGold);
        statName.Add("Gold");
        stats.Add(player.GetComponent<PlayerAttackHandler>().Damage);
        statName.Add("Damage");
        stats.Add(player.GetComponent<PlayerHealth>().MaxHealth);
        statName.Add("Max Health");

        BuffStorage buffstorage = player.GetComponent<PlayerEquipmentHandler>().Backpack.BuffStorage;
        AuraBuff auraBuff = buffstorage.GetBuffByType<AuraBuff>();
        if (auraBuff != null)
        {
            stats.Add(auraBuff.Radius);
            statName.Add("Aura Radius");
            stats.Add(auraBuff.Damage);
            statName.Add("Aura Damage");
        }

        GO_stats = new GameObject[stats.Count];
        for (int i = 0; i <= stats.Count; i++)
        {
            try
            {
            GO_stats[i] = GameObject.Instantiate(statPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Transform statTransform = GO_stats[i].transform;

            statTransform.position = new Vector3(0, 0, 0);
            canvas = transform.parent.GameObject();
            statTransform.parent = canvas.transform.GetChild(3);
            statTransform.GetComponent<RectTransform>().localScale = new Vector3(1.02168f, 1.02168f, 1.02168f);

            statTransform.name = i.ToString();

            statTransform.GetComponent<TextMeshProUGUI>().text = statName[i] + ": " + stats[i];
            }
            catch
            {
            }
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
