using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TMP_Text text;



    public void SetGameOverReason(string reason)
    {
        text.text = reason;
    }

    public void ResetRoom()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
