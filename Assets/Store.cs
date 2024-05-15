using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public static Store instance;
    public List<GameObject> initialItems;
    public LinkedList<GameObject> activeItems = new();
    public LinkedList<GameObject> remainingItems = new();

    public GameObject itemOrganizer;
    public GameObject rowPrefab;
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

        foreach (var item in initialItems)
        {
            activeItems.AddLast(item);
            remainingItems.AddLast(item);
        }
    }

    private void Start()
    {
        //SaveItemID(initialItems);
        Redistribute(remainingItems);
    }

    // Todavia nada.
    public void SaveItemID(List<GameObject> items)
    {
        //var itemID = new Tuple<string, Item.ItemType, int>("", Item.ItemType.armor, 0);
        
        foreach (var item in items)
        {

        }
    }
    
    // Se deshace de los elementos anteriores y redistribuye los objetos restantes en la tienda.
    public void Redistribute(IEnumerable<GameObject> col = default)
    {
        foreach (var item in spawnedRows)
        {
            Destroy(item);
        }
        spawnedRows.Clear();

        DistributeItems(col.Select(x => x.gameObject));
    }

    // Si algun nombre de objeto no presenta coincidencias con el de el texto del input, se lo filtra.
    public void FilterByText(string inputText, bool contextSensitivity = false)
    {
        // si el texto esta vacio, vuelve a mostrar todos los objetos.
        if (inputText == "") 
        {
            Redistribute(remainingItems);
            print(inputText);
            return;
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

        Redistribute(filteredItems);
    }

    #region ORGANIZACION DE ITEMS
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
}
