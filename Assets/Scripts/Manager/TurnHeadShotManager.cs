using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnHeadShotManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
   
     
    void Update()
    {
        if(gameManager.CurrentTurn == GameManager.TurnId.Player)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(false);
        }else
        if(gameManager.CurrentTurn == GameManager.TurnId.Hero)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            transform.GetChild(2).gameObject.SetActive(false);
        }else
        if(gameManager.CurrentTurn == GameManager.TurnId.Enemy)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
    }
}
