using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Store : MonoBehaviour
{
    #region VARIABLES
    public static Store instance;
    StoreFilters _storeFilters;

    [Header("ALL ITEMS")]
    public List<GameObject> initialItems;
    public LinkedList<GameObject> activeItems = new();
    public LinkedList<GameObject> remainingItems = new();

    public GameObject itemOrganizer;
    public GameObject rowPrefab;

    [Header("NOTIFIERS")]
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI notifyText;
    public TextMeshProUGUI infoText;
    public TextMeshProUGUI dollarsText;
    public float notificationTime;

    [Header("SIZE ADJUST")]
    [Range(1,6)]
    public int xSize = 5;
    [Range(1,4)]
    public int ySize = 4;
    //[Range(0.25f, 2f)]
    //public float itemScale = 1;
    List<GameObject> spawnedRows = new();
    List<Tuple<int, int, string>> allItemInfo = new();
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        _storeFilters = GetComponent<StoreFilters>();
        allItemInfo = MakeItemInfo();

        foreach (var item in initialItems)
        {
            activeItems.AddLast(item);
            remainingItems.AddLast(item);
        }
    }

    private void Start()
    {
        //SaveItemID(initialItems);
        _storeFilters.UpdateFiltering();
        infoText.text = "Welcome! Right click any item to learn about them.";
        moneyText.text = $"Coins: {Wallet.money}";
        dollarsText.text = $"Dollars: ${Wallet.dollars}";

    }

    #region DESCRIPTION
    //private void Update()
    //{
    //    OnHoveringOverItems();
    //}

    //public void OnHoveringOverItems()
    //{
    //    Ray ray; 
    //    RaycastHit hit;

    //    ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //    if (Physics.Raycast(ray, out hit))
    //    {
    //        if (hit.collider.gameObject.GetComponentInParent<Item>())
    //        {
    //            infoText.text = hit.collider.gameObject.GetComponentInParent<Item>().itemInfo;
    //            print(hit.transform.gameObject.name);
    //        }
    //    }
    //}

    // Construye una descripcion del item en base a sus variables.
    public List<Tuple<int,int,string>> MakeItemInfo()
    {
        var firstList = initialItems.Select(x => x.GetComponent<Item>().firstInfo);
        var secondList = initialItems.Select(x => x.GetComponent<Item>().secondInfo);
        var descriptionList = initialItems.
                              Aggregate(new List<string>(), (acum, current) => 
                              {
                                  acum.Add(current.GetComponent<Item>().description);
                                  return acum;
                              }).
                              ToList();

        var itemInfoList = firstList.
                           Zip(secondList, (v1, v2) => new {var1 = v1, var2 = v2}).
                           Zip(descriptionList, (d1, d2) => new Tuple<int, int, string>(d1.var1, d1.var2, d2)).
                           ToList();

        foreach (var item in initialItems.Select(x => x.GetComponent<Item>()))
        {
            var currentInfo = itemInfoList[initialItems.IndexOf(item.gameObject)];

            if(item is ItemArmor)
            item.GetComponent<Item>().itemInfo = $"Defense = {currentInfo.Item1}.  |  Durability = {currentInfo.Item2}.  |  {currentInfo.Item3}";
            else if(item is ItemPotion)
            item.GetComponent<Item>().itemInfo = $"Level = {currentInfo.Item1}.  |  Duration = {currentInfo.Item2}.  |  {currentInfo.Item3}";
            else if (item is ItemWeapon)
            item.GetComponent<Item>().itemInfo = $"Damage = {currentInfo.Item1}.  |  Durability = {currentInfo.Item2}.  |  {currentInfo.Item3}";
        }

        return itemInfoList;
    }
    #endregion

    #region FILTROS
    // Si algun nombre de objeto no presenta coincidencias con el de el texto del input, se lo filtra.
    public IEnumerable<GameObject> FilterByText(string inputText, bool contextSensitivity = false)
    {
        // si el texto esta vacio, vuelve a mostrar todos los objetos.
        if (inputText == "")
        {
            _plusTextFilter = false;
            Redistribute(remainingItems);
            print(inputText);
            return activeItems;
        }

        var filteredItems = activeItems.
                            /*convertido a items*/ Select(x => x.GetComponent<Item>()).
                            /*si su nombre contiene al texto*/ Where(x => x.CompareNames(inputText.ToUpper(), contextSensitivity)).
                            /*ordenar por*/ OrderByTextMatch(inputText);

        if (filteredItems.Count() <= 0)
        {
            foreach (var fi in filteredItems)
            {
                print(fi.GetComponent<Item>().itemName);
            }
        }
        else
        {
            print("Ningun elemento tiene ese nombre");
        }

        _plusTextFilter = true;
        Redistribute(filteredItems);
        return filteredItems;
    }

    // Filtra todos los objetos que no pertenezcan al tipo seleccionado.
    public IEnumerable<GameObject> FilterByType(TMP_Dropdown.OptionData option, IEnumerable<GameObject> filteredByText = default)
    {
        string typeText = option.text.ToUpper();
        Redistribute(activeItems);

        var armorItems = activeItems.Select(x => x.GetComponent<Item>()).OfType<ItemArmor>().OrderBy(x => x.name.First()).ThenBy(x => x.name.Skip(1).FirstOrDefault());
        var potionItems = activeItems.Select(x => x.GetComponent<Item>()).OfType<ItemPotion>().OrderBy(x => x.name.First()).ThenBy(x => x.name.Skip(1).FirstOrDefault());
        var weaponItems = activeItems.Select(x => x.GetComponent<Item>()).OfType<ItemWeapon>().OrderBy(x => x.name.First()).ThenBy(x => x.name.Skip(1).FirstOrDefault());

        List<GameObject> fItems = new();

        if(typeText.Contains("ARMOR"))
        {
            fItems = armorItems.Select(x => x.gameObject).ToList();
        }
        else if (typeText.Contains("POTION"))
        {
            fItems = potionItems.Select(x => x.gameObject).ToList();
        }
        else if (typeText.Contains("POTION"))
        {
            fItems = weaponItems.Select(x => x.gameObject).ToList();
        }

        if (typeText == "ALL")
        {
            fItems = filteredByText.ToList();
        }


        if (_plusTextFilter) 
        fItems = fItems.
                Aggregate(/*valor por defecto*/ new List<GameObject>(), /*total acumulado, nodo actual*/ (acum, current) =>
                {
                    // Si el nodo actual tambien esta filtrado por el texto lo conservamos en la lista.
                    if (filteredByText.Contains(current))
                    {
                        acum.Add(current);
                    }

                    return acum;
                });
       
        Redistribute(fItems);
        return fItems;

    }

    // Ordena el texto en base a la propiedad que especifiquemos.
    public IEnumerable<GameObject> OrderByProperty(TMP_Dropdown.OptionData option, IEnumerable<GameObject> filteredByTextThenType = default)
    {
        var opt = option.text.ToUpper();

        var orderedItems = filteredByTextThenType;
        
        if(opt == "NAME")
        {
            orderedItems = orderedItems.Alphabetically(_storeFilters.inverseOrder);
        }
        else if (opt == "TYPE")
        {
            orderedItems = orderedItems.ByType(_storeFilters.inverseOrder);
        }
        else if (opt == "PRICE")
        {
            orderedItems = orderedItems.ByPrice(_storeFilters.inverseOrder);
        }

        infoText.text = $"The {orderedItems.Count()} items that match your request have been ordered by {opt.ToLower()}.";
        Redistribute(orderedItems);
        return orderedItems;
    }
    #endregion

    #region ORGANIZACION DE ITEMS
    bool _plusTextFilter = false;
    
    // Se deshace de los elementos anteriores y redistribuye los objetos restantes en la tienda.
    public void Redistribute(IEnumerable<GameObject> col = default)
    {
        foreach (var item in spawnedRows)
        {
            Destroy(item);
        }
        spawnedRows.Clear();

        DistributeItems(col);
    }

    // Instancia las filas, cada una con su cantidad de items correspondientes.
    public void DistributeItems(IEnumerable<GameObject> allItems)
    {
        var rows = SubdivideItems(allItems).ToList();
        if (activeItems.Any(x => x != null)) activeItems.Clear(); else print("La lista ya estaba vacía.");

        foreach (var row in rows)
        {
            var newRow = Instantiate(rowPrefab, itemOrganizer.transform);
            spawnedRows.Add(newRow);

            foreach (var item in row)
            {
                var i = Instantiate(item, newRow.transform);
                i.GetComponent<Image>().sprite = i.GetComponent<Item>().sprite;
                //i.RescaleItem(itemScale);
                activeItems.AddLast(i);
            }
        }

        //print($"REDISTRIBUTED: Rows = {rows.Count}; Total items = {activeItems.Count}.");
    }

    // Divide la coleccion en listas mas pequeñas (filas), cada una representando una fila de la distribucion.
    IEnumerable<List<GameObject>> SubdivideItems(IEnumerable<GameObject> allItems)
    {
        bool hasItemsLeft = true;
        List<List<GameObject>> rows = new();

        List<GameObject> i = new();
        while (hasItemsLeft)
        {
            List<GameObject> newRow = new();

            foreach (var item in allItems.Skip(i.Count))
            {
                newRow.Add(item);
                i.Add(item);
                if (newRow.Count >= xSize || i.Count >= allItems.Count()) break;
            }

            if (rows.Count <= ySize - 1) rows.Add(newRow); else print("Not enough space for your items.");
            if (i.Count >= allItems.Count()) hasItemsLeft = false;
        }

        return rows;
    }

    #endregion

    // Actualiza el texto cuando se realiza una compra
    public IEnumerator NotifyText(string newText)
    {
        notifyText.text = newText;
        moneyText.text = $"Money : {Wallet.money}";
        yield return new WaitForSeconds(notificationTime);
        notifyText.text = "";
    }

}





