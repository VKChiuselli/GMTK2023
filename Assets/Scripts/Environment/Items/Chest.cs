using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractableEntity
{
    bool isOpen;
    bool isEmpty;
    public List<Sprite> openCloseSprites; 
    public InventoryItem itemContained;
     
    FunBarManager funBarManager; 
    private void Start()
    {
        itemContained = null;
        isOpen = false;
        isEmpty = true;
        funBarManager = FindObjectOfType<FunBarManager>();
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
        Debug.Log("Interacting with chest test");
        if (unit.GetType().DerivesFrom(typeof(Hero)))
        {
            Debug.Log("Interacting with chest");
            OpenChest();
            wasVisited = true;
            
            if (!isEmpty)
            {
                Inventory.Instance.AddItem(LootItem());
                Debug.Log("Looted the chest");
 
                if (!isEmpty)
                {
                    Debug.Log("Adding item from chest to inventory");
                    if (funBarManager != null)
                    {
                        funBarManager.ChangeFunBarCounter(20);
                    }
                }
                else
                {
          
                    Debug.Log("Chest is empty");
                }
 
            }
            else
            {
                Debug.Log("Chest is empty");
                if (funBarManager != null)
                {
                    funBarManager.ChangeFunBarCounter(-10);
                }
                Destroy(gameObject);
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
