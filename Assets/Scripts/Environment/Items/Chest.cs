using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractItem
{
    bool isOpen;
    bool isEmpty;
    public List<Sprite> openCloseSprites;
    public GameObject itemContained;

    private void Start()
    {
        itemContained = null;
        isOpen = false;
        isEmpty = true;
    }

    private void Update()
    {
        //if (itemContained != null)
        //{
        //    isEmpty = false;
        //}
        //else
        //{
        //    isEmpty = true;
        //}

        if (isOpen)
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = openCloseSprites[0];
        }
        else
        {
            transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = openCloseSprites[1];
        }
    }

    public override void InteractWith(Unit unit)
    {
        if (unit.GetType() == typeof(Hero))
        {
            if (isOpen)
            {
                if (!isEmpty)
                {
                    Debug.Log("Adding item from chest to inventory");
                }
                else
                {
                    Debug.Log("Chest is empty");
                }
            }
            else
            {
                OpenChest();
                Debug.Log("Chest was opened");
                Destroy(gameObject, 1);
            }
        }
    }


    public void InsertItem(GameObject insertItem)
    {
        itemContained = insertItem;
        isEmpty = false;
    }

    public GameObject LootItem()
    {
        isEmpty = true;
        return itemContained;

    }

    public void OpenChest()
    {
        isOpen = true;
    }

    public void CloseChest()
    {
        isOpen = false;
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
