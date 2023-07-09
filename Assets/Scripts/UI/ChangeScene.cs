using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    AudioManager audioManager;
    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }


    public void GoToScene(string sceneName)
    {
        if (audioManager != null)
        {
            audioManager.StopMusic();
            audioManager.PlayGameLoopBackground();
            audioManager.PlayStartGameEffect();
        }
        SceneManager.LoadScene(sceneName);
    }


}
