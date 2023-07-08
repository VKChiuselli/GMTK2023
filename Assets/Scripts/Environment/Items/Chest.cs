using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableEntity
{
    bool isOpen;
    bool isEmpty;
    public List<Sprite> openCloseSprites;
    public InventoryItem itemContained;

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
            OpenChest();
            
            if (!isEmpty)
            {
                Inventory.Instance.AddItem(LootItem());
                Debug.Log("Looted the chest");
            }
            else
            {
                Debug.Log("Chest is empty");
            }
        }
    }

    public void InsertItem(InventoryItem insertItem)
    {
        itemContained = insertItem;
        isEmpty = false;
    }

    public InventoryItem LootItem()
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
