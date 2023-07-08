using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
 
    public AudioClip soundEffect; // Sound effect to play
    private AudioSource audioSource;

    private void Start()
    {
        // Get the AudioSource component attached to this game object
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect()
    {
        // Play the sound effect
        audioSource.PlayOneShot(soundEffect);
    }
}
 
