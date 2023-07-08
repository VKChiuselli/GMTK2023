using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : InteractItem
{
    bool isPickedUp;
    Door door;

    private void Start()
    {
        isPickedUp = false;
        door = FindObjectOfType<Door>();
    }

   

    public override void InteractWith(Unit unit)
    {
        isPickedUp = true;
        OpenDoor();
        PlayPickKeySFX();
    }

    public void OpenDoor()
    {
        door.OpenDoor();
    }
    public void PlayPickKeySFX()
    {
        GetComponent<SFX>().PlayFirstEffect();
    }

}
