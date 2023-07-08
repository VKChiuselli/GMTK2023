using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanelMusicSlider : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] Slider volumeSlider;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("GameVolume"))
        {
            float volumeSaved = PlayerPrefs.GetFloat("GameVolume");
            volumeSlider.value = volumeSaved;
        }
        else
        {
            volumeSlider.value = audioSource.volume;
        }
    }

    public void OnSliderValueChanged()
    {
        audioSource.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("GameVolume", audioSource.volume);
    }
}
