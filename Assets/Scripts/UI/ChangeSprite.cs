using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSprite : MonoBehaviour
{
    public List<Image> listOfImages = new List<Image>();
    public int currentMana;

    void Start()
    {
        currentMana = 3;
        LoadAllMana();
    }

    private void LoadAllMana()
    {
        foreach (Transform t in transform)
        {
            transform.gameObject.SetActive(true);
        }
    }

    public void RemoveMana(int manaToRemove)
    {
        currentMana = currentMana - manaToRemove;


        if (currentMana == 2)
        {
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        if (currentMana == 1)
        {
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        if (currentMana == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Too much mana spent");
        }
    }



}
