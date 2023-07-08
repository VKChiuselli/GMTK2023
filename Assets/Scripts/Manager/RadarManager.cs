using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarManager : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject radar;

    bool LoseTheGame;
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other collider has a specific tag
        if (other.CompareTag("Player"))
        {
            LoseTheGame = true;
            TriggerLoseAction();
        }
    }

    private void Update()
    {


    }


    private void TriggerLoseAction()
    {
        Debug.Log("TODO");
        //stop the player to move
        //Active a text AND A ECLAMATIVE MARK!
        //show END GAME
    }

}
