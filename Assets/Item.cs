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
    public string itemName;
    public int price;

    Button _button;
    TextMeshProUGUI _buttonText;
    TextMeshProUGUI _nameText;


    private void Awake()
    {
        GetComponent<Image>().sprite = sprite;

        _nameText = GetComponentInChildren<TextMeshProUGUI>();
        _nameText.text = $"{itemName}";
        
        _button = GetComponentInChildren<Button>();
        _buttonText = _button.GetComponentInChildren<TextMeshProUGUI>();
        _buttonText.text = $"Buy ({price})";

        //Store.instance.items.AddLast(this.gameObject);
    }

    // Obtiene este item desde la lista de la tienda y provoca su redistribucion.
    public void Buy()
    {
        if (Wallet.money >= price)
        {
            Store.instance.remainingItems.Remove(this.gameObject);
            print(Store.instance.remainingItems.Count);
            Store.instance.activeItems.Remove(this.gameObject);

            Store.instance.Redistribute(Store.instance.activeItems);

            Store.instance.StartCoroutine(Store.instance.NotifyText("PURCHASED"));
            Destroy(gameObject);
            Wallet.money -= price;
        }
        else 
        {
            StartCoroutine(Store.instance.NotifyText("NOT ENOUGH"));
        } 
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
