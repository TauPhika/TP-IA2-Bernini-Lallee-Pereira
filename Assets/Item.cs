using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Item : Items
{
    public ItemType itemType;
    public Sprite sprite;
    public int price;
    Button _button;
    TextMeshProUGUI _text;

    private void Awake()
    {
        GetComponent<Image>().sprite = sprite;

        _button = GetComponentInChildren<Button>();

        _text = GetComponentInChildren<TextMeshProUGUI>();
        _text.text = $"Buy ({price})";

        //Store.instance.items.AddLast(this.gameObject);
    }

    public void Buy()
    {
        if (Wallet.money >= price) 
        {
            Store.instance.items.Remove(this.gameObject);

            Store.instance.Redistribute();

            print($"PURCHASED: Total items left = {Store.instance.items.Count}; Money left = {Wallet.money}.");
            Destroy(gameObject);
            Wallet.money -= price;
        } 
    }
}
