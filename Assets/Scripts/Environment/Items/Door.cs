using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : InteractItem
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

    public void OpenDoor()
    {
        isOpen = true;
    }

    public void CloseDoor()
    {
        isOpen = false;
    }

    public void PlayOpenDoorSFX()
    {
        GetComponent<SFX>().PlayFirstEffect();
    } 

    public void ChangeFloor()
    {
        Debug.Log("TODO");
    }

}
