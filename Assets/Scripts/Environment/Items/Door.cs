using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : InteractItem
{
    bool isOpen;
    public List<Sprite> openCloseSprites;
    private int currentSceneIndex;

    private void Start()
    {
        isOpen = false;
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
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


    public override void InteractWith(Unit unit)
    {
        ChangeFloor();
    }

    public void ChangeFloor()
    {
        currentSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(currentSceneIndex);
    }

}
