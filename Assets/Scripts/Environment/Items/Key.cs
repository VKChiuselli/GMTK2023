using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Key : InteractableEntity
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
        Inventory.Instance.AddKey();
        //OpenDoor();
        PlayPickKeySFX();
        Destroy(gameObject, 0.5f);
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
