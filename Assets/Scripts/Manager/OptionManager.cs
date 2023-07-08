using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionManager : MonoBehaviour
{
    public GameObject optionUI;

    void Awake()
    {
        int numGameSessions = FindObjectsOfType<OptionManager>().Length;
        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

    }
        private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (Time.timeScale == 0f)
            {
                Time.timeScale = 1f; // Unfreeze time
                CloseOptionUI();
            }
            else
            {
                Time.timeScale = 0f; // Freeze time
                OpenOptionUI();
            }
        }
    }

    private void OpenOptionUI()
    {
        optionUI.SetActive(true);
    }

    private void CloseOptionUI()
    {
        optionUI.SetActive(false);
    }

}
