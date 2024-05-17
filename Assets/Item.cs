using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public abstract class Item : Items
{
    public ItemType itemType;
    public Sprite sprite;
    public string itemName;
    [HideInInspector] public string itemInfo;
    public int price;

    public int firstInfo;
    public int secondInfo;
    public string description;

    Button _button;
    TextMeshProUGUI _buttonText;
    TextMeshProUGUI _nameText;

    public bool isPremium; 

    private void Awake()
    {
        GetComponent<Image>().sprite = sprite;

        _nameText = GetComponentInChildren<TextMeshProUGUI>();
        _nameText.text = $"{itemName}";

        _button = GetComponentInChildren<Button>();
        _buttonText = _button.GetComponentInChildren<TextMeshProUGUI>();
        if (!isPremium) _buttonText.text = $"Buy ({price})";
        else
        { 
            _buttonText.text = $"Pay ${price}";
            _buttonText.color = new Color(0,0.8f,0);
        }
    }


    // Obtiene este item desde la lista de la tienda y provoca su redistribucion.
    public void Buy()
    {
        if (!isPremium) Purchase(Wallet.money >= price); else Purchase(Wallet.dollars >= price);
    }

    public void Purchase(bool buyCondition)
    {
        if (buyCondition)
        {
            Store.instance.remainingItems.Remove(this.gameObject);
            print(Store.instance.remainingItems.Count);
            Store.instance.activeItems.Remove(this.gameObject);

            Inventory.instance.GotNewItem(gameObject);
            
            Store.instance.Redistribute(Store.instance.activeItems);

            Store.instance.StartCoroutine(Store.instance.NotifyText("PURCHASED"));
            Destroy(gameObject);
            Wallet.money -= price;
        }
        else
        {
            StartCoroutine(Store.instance.NotifyText("NOT ENOUGH"));
        }


        Store.instance.moneyText.text = $"Coins: {Wallet.money}";
        Store.instance.dollarsText.text = $"Dollars: ${Wallet.dollars}";
    }


    public void ShowInfo()
    {
        Store.instance.infoText.text = itemInfo;
    }

    // Compara el texto pasado con el nombre de este objeto
    public bool CompareNames(string inputText, bool contextSensitive = false)
    {
        var matching = new List<bool>();
        
        foreach (var c in inputText)
        {
            if (!contextSensitive)
            {
                if (itemName.Skip(inputText.IndexOf(c)).First() == c) matching.Add(true); 
                else matching.Add(false);

                print(matching[inputText.IndexOf(c)]);
            }
            else
            {
                if (itemName.Contains(c)) matching.Add(true);
                else matching.Add(false);

            }
            
        }

        return matching.All(x => x == true); 
    }
}
