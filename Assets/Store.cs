using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Store : MonoBehaviour
{
    public static Store instance;
    StoreFilters _storeFilters;
    public List<GameObject> initialItems;
    public LinkedList<GameObject> activeItems = new();
    public LinkedList<GameObject> remainingItems = new();

    public GameObject itemOrganizer;
    public GameObject rowPrefab;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI notifyText;
    public float notificationTime;
    [Range(1,10)]
    public int xSize = 5;
    [Range(1,4)]
    public int ySize = 4;
    [Range(0.25f, 2f)]
    public float itemScale = 1;
    List<GameObject> spawnedRows = new();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        _storeFilters = GetComponent<StoreFilters>();

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
    }

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
                            /*si su nombre contiene al texto*/ Where(x => x.CompareNames(inputText, contextSensitivity)).
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

        var filteredItems = activeItems.
                            Where(x => typeText.Contains(x.GetComponent<Item>().itemType.ToString())).
                            OrderBy(x => x.name.First()).
                            ThenBy(x => x.name.Skip(1).FirstOrDefault()).
                            ToList();

        if (typeText == "ALL")
        {
            filteredItems = filteredByText.ToList();
        }


        if (_plusTextFilter) 
        filteredItems = filteredItems.
                        Aggregate(/*valor por defecto*/ new List<GameObject>(), /*total acumulado, nodo actual*/ (acum, current) =>
                        {
                            // Si el nodo actual tambien esta filtrado por el texto lo conservamos en la lista. Es un Where casero.
                            if (filteredByText.Contains(current))
                            {
                                acum.Add(current);
                            }

                            return acum;
                        });
        
        print($"There are {filteredItems.Count} items that match the type {typeText}");
        Redistribute(filteredItems);
        return filteredItems;

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

        print($"Ordered {opt}");
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
                i.RescaleItem(itemScale);
                activeItems.AddLast(i);
            }
        }

        print($"REDISTRIBUTED: Rows = {rows.Count}; Total items = {activeItems.Count}.");
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

    // Todavia nada.
    public void SaveItemID(List<GameObject> items)
    {
        var itemID = Tuple.Create("", Item.ItemType.ARMOR, 0);

        foreach (var item in items)
        {

        }
    }

}





