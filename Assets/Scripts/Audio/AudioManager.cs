using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{


    private void Awake()
    {

        DontDestroyOnLoad(gameObject);


        if (PlayerPrefs.HasKey("GameVolume"))
        {
            float volumeSaved = PlayerPrefs.GetFloat("GameVolume");
            GetComponent<AudioSource>().volume = volumeSaved;
        }
        else
        {
            GetComponent<AudioSource>().volume = 0.01f;
        }
        // Set initial slider value to match audio source volume
    }
}
