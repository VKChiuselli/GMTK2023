using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractItem
{
    bool isOpen;
    public List<Sprite> openCloseSprites;

    private void Start()
    {
        isOpen = false;
    }

    private void Update()
    {
        if (isOpen)
        {
           transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = openCloseSprites[0];
        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = openCloseSprites[1];
        }
    }

    public void PlayOpenChestSFX()
    {
        GetComponent<SFX>().PlayFirstEffect();
    }
    public void PlayCloseChestSFX()
    {
        GetComponent<SFX>().PlaySecondEffect();
    }

}
