using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{

    public AudioClip firstEffect; // Sound effect to play
    public AudioClip secondEffect; // Sound effect to play
    public AudioClip thirdEffect; // Sound effect to play
    public List<AudioClip> severalEffects; // for the hero and player, there are too much SFX so put them here
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayFirstEffect()
    {
        if (audioSource != null)
        {
            if (firstEffect != null)
            {
                audioSource.PlayOneShot(firstEffect);
            }
        }

    }
    public void PlaySecondEffect()
    {
        if (audioSource != null)
        {
            if (secondEffect != null)
            {
                audioSource.PlayOneShot(secondEffect);
            }
        }
    }
    public void PlayThirdEffect()
    {
        if (audioSource != null)
        {
            if (thirdEffect != null)
            {
                audioSource.PlayOneShot(thirdEffect);
            }
        }

    }

    public void PlayOneSpecific(int trackIndex)
    {
        if (audioSource != null)
        {
            if (severalEffects != null)
            {
                audioSource.PlayOneShot(severalEffects[trackIndex]);
            }
        }

    }
}


