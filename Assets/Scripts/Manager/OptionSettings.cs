using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class OptionSettings : MonoBehaviour
{
    [SerializeField]
    private AudioMixerGroup _SFXMixer;
    [SerializeField]
    private AudioMixerGroup _musicMixer;
    [SerializeField]
    private AudioMixerGroup _masterMixer;

    [SerializeField]
    [Header("Hidden master volume dB adjustment")]
    private float _adjustMaxMasterVolumeDB = 0.0f;

    private static float _mixerGroupDBRange = 80f;
    private static float _mixerGroupDBAdjustment = 20f;

    /// <summary>
    /// Sets the SFX volume, slider MUST be between 0 and 1
    /// </summary>
    public void SetSFXVolume(float sliderValue)
    {
        float desiredVolume = Mathf.Log10(sliderValue) * 20f;
        _SFXMixer.audioMixer.SetFloat("SFXVolume", desiredVolume);
    }

    /// <summary>
    /// Sets the Music volume, slider MUST be between 0 and 1
    /// </summary>
    public void SetMusicVolume(float sliderValue)
    {
        float desiredVolume = Mathf.Log10(sliderValue) * 20f;
        desiredVolume = Mathf.Clamp(desiredVolume, -80f, 0f);
        _SFXMixer.audioMixer.SetFloat("MusicVolume", desiredVolume);
    }

    /// <summary>
    /// Sets the Master volume, slider MUST be between 0 and 1
    /// </summary>
    public void SetMasterVolume(float sliderValue)
    {

        float desiredVolume = Mathf.Log10(sliderValue) * 20f + _adjustMaxMasterVolumeDB;
        desiredVolume = Mathf.Clamp(desiredVolume, -80f, 0f);
        _SFXMixer.audioMixer.SetFloat("MasterVolume", desiredVolume);   
    }

    private void Update()
    {
        SetMasterVolume(1.0f);
    }

}
