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

        moneyDisplay.text = $"Coins: {Wallet.money}";
        dollarsDisplay.text = $"Dollars: ${Wallet.dollars}";

        inventoryDisplay.Add(Instantiate(moneyDisplay.gameObject, inventoryPanel));
        inventoryDisplay.Add(Instantiate(dollarsDisplay.gameObject, inventoryPanel));
    }

    private void Start()
    {
        inventoryMenu.SetActive(false);
    }

    #region GotNewItem 1
    //public void GotNewItem(GameObject newItem = default)
    //{
    //    if (inventoryDisplay.DefaultIfEmpty() == default) print("Algo salio mal.");

    //    newItem.GetComponent<Image>().sprite = newItem.GetComponent<Item>().sprite;
    //    Destroy(newItem.GetComponentInChildren<Button>().gameObject);

    //    inventoryDisplay.Add(Instantiate(newItem, inventoryPanel));

    //    List<GameObject> keepCol = inventoryDisplay.
    //                               TakeWhile(x => x.GetComponent<TextMeshProUGUI>()).
    //                               SetAllActive(true).
    //                               ToList();



    //    print($"Textos = {keepCol.Count()}.");
    //    inventoryDisplay.Remove(newItem);

    //    foreach (var item in inventoryDisplay)
    //    {
    //        Destroy(item.gameObject);
    //    }

    //    myItems.Add(newItem);
    //    print($"Items = {myItems.Count()}.");

    //    LinkedList<GameObject> newCol = keepCol.Concat(myItems.ByType()).ToLinkedList();

    //    print($"Textos + items = {newCol.Count()}.");

    //    foreach (var item in newCol)
    //    {            
    //        var i = Instantiate(item, inventoryPanel);
    //        inventoryDisplay.Add(i);
    //    }

    //    newCol.Clear();
    //}
    #endregion

    #region GotNewItem 2
    public void GotNewItem(GameObject newItem = default)
    {
        if (inventoryDisplay.DefaultIfEmpty() == default) print("Algo salio mal.");

        newItem.GetComponent<Image>().sprite = newItem.GetComponent<Item>().sprite;
        Destroy(newItem.GetComponentInChildren<Button>().gameObject);

        //inventoryDisplay.Add(Instantiate(newItem, inventoryPanel));

        List<GameObject> keepCol = inventoryDisplay.
                                   TakeWhile(x => x.GetComponent<TextMeshProUGUI>()).
                                   SetAllActive(true).
                                   ToList();



        print($"Textos = {keepCol.Count()}.");
        inventoryDisplay.Remove(newItem);

        foreach (var item in inventoryDisplay)
        {
            Destroy(item.gameObject);
        }
        inventoryDisplay.Clear();

        myItems.Add(newItem);
        print($"Items = {myItems.Count()}.");

        LinkedList<GameObject> newCol = keepCol.Concat(myItems).ToLinkedList();

        print($"Textos + items = {newCol.Count()}.");

        foreach (var item in newCol)
        {
            var i = Instantiate(item, inventoryPanel);
            inventoryDisplay.Add(i);
        }

        newCol.Clear();
    }
    #endregion

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
