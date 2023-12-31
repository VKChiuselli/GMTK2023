using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    #region Singleton
    public static Inventory Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

    }
    #endregion
    


    [SerializeField]
    private int _keyCount = 0;

    [SerializeField]
    private List<InventoryItem> _items = new List<InventoryItem>();

    public void AddItem(InventoryItem item)
    {
        _items.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        _items.Remove(item);
    }

    public bool CheckForItem(InventoryItem item)
    {
        if (_items.Contains(item))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CheckForItem(string itemName)
    {
        if (_items.Any(item => item.name == itemName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// Adds a key.
    /// </summary>
    public void AddKey()
    {
        _keyCount++;
    }

    /// <summary>
    /// Removes a single key, if there are keys to remove.
    /// </summary>
    /// <returns>True if removal was successful, false if there are no keys.</returns>
    public bool TryUseKey()
    {
        if (_keyCount > 0)
        {
            _keyCount--;
            return true;
        }
        else
        {
            return false;
        }
    }





}
