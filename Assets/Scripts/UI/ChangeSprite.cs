using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSprite : MonoBehaviour
{
    public List<Image> listOfImages = new List<Image>();
    public int currentMana;
    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        currentMana = 3;
        LoadAllMana();
    }

    private void LoadAllMana()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
        transform.GetChild(2).gameObject.SetActive(false);
        transform.GetChild(3).gameObject.SetActive(false);
    }

    private void Update()
    {
        RemoveMana(player.Mana);
    }

    public void RemoveMana(int actualMana)
    {
        currentMana = actualMana;

        if (currentMana >= 3)
        {
            currentMana = 3;
        }

        if (currentMana == 3)
        {
            LoadAllMana();
        }

        if (currentMana == 2)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        if (currentMana == 1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        if (currentMana == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(true);
        }
    }



}
