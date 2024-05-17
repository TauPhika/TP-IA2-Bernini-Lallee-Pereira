using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    
    [HideInInspector] public List<GameObject> myItems = new();
    public Transform inventoryPanel;
    public GameObject inventoryMenu;
    public GameObject storeMenu;

    List<GameObject> inventoryDisplay = new();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        TurnInventoryMenu();
    }

    public IEnumerable<GameObject> GotNewItem(GameObject newItem)
    {
        var newCol = inventoryDisplay.TakeWhile(x => x.GetComponent<TextMeshProUGUI>());

        foreach (var item in inventoryDisplay)
        {
            Destroy(item);
        }
        inventoryDisplay.Clear();

        Destroy(newItem.GetComponentInChildren<Button>().gameObject);
        myItems.Add(newItem);

        inventoryDisplay = newCol.Concat(myItems).ToList();

        foreach (var item in newCol.Concat(myItems))
        {
            var i = Instantiate(item, inventoryPanel);
            inventoryDisplay.Add(i);
        }

        return inventoryDisplay;
    }

    public void TurnInventoryMenu()
    {
        if (inventoryMenu.activeInHierarchy)
        {
            inventoryMenu.SetActive(false);
            storeMenu.SetActive(true);
        }
        else 
        {
            inventoryMenu.SetActive(true);
            storeMenu.SetActive(false);
        } 
    }

}
