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

    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI dollarsDisplay;

    List<GameObject> inventoryDisplay = new();

    private void Awake()
    {
        instance = this;

        inventoryDisplay.Add(moneyDisplay.gameObject);
        inventoryDisplay.Add(dollarsDisplay.gameObject);
    }

    private void Start()
    {
        inventoryMenu.SetActive(false);
    }

    public void GotNewItem(GameObject newItem = default)
    {
        List<GameObject> keepCol = inventoryDisplay.TakeWhile(x => x.GetComponent<TextMeshProUGUI>()).ToList();
        print($"Textos = {keepCol.Count()}.");
        //{ if (!x.activeInHierarchy) x.SetActive(true); return x; }
        inventoryDisplay.Remove(newItem);
        
        foreach (var item in inventoryDisplay)
        {
            Destroy(item.gameObject);
        }
        inventoryDisplay.Clear();

        Destroy(newItem.GetComponentInChildren<Button>().gameObject);
        myItems.Add(newItem);
        print($"Items = {myItems.Count()}.");

        List<GameObject> newCol = keepCol.Concat(myItems).ToList();

        print($"Textos + items = {newCol.Count()}.");

        foreach (var item in newCol)
        {            
            var i = Instantiate(item.gameObject, inventoryPanel);
            inventoryDisplay.Add(i);
        }
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
