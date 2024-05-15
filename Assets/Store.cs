using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Store : MonoBehaviour
{
    public static Store instance;
    public List<GameObject> items;
    public LinkedList<GameObject> _items = new();

    public GameObject itemOrganizer;
    public GameObject rowPrefab;
    [Range(2,10)]
    public int xSize = 5;
    [Range(2,10)]
    public int ySize = 4;
    [Range(0.25f, 2f)]
    public float itemScale = 1;
    List<GameObject> spawnedItems = new();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        foreach (var item in _items)
        {
            _items.AddLast(item);
        }
    }

    private void Start()
    {
        Redistribute();
    }

    // Se deshace de los elementos anteriores y redistribuye los objetos restantes en la tienda.
    public void Redistribute()
    {
        foreach (var item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();

        DistributeItems(items.Select(x => x.gameObject));
    }

    #region ORGANIZACION DE ITEMS
    // Genera filas, cada una con su cantidad de items correspondientes.
    public void DistributeItems(IEnumerable<GameObject> allItems)
    {       
        LinkedList<GameObject> _allItems = new();

        foreach (var item in allItems) _allItems.AddLast(item);

        var rows = OrganizeItems(_allItems);
        items.Clear();

        foreach (var row in rows)
        {
            var newRow = Instantiate(rowPrefab, itemOrganizer.transform);
            spawnedItems.Add(newRow);

            foreach (var item in row)
            {
                var i = Instantiate(item, newRow.transform);
                i.GetComponent<Image>().sprite = i.GetComponent<Item>().sprite;
                i.RescaleItem(itemScale);
                items.Add(i);
            }
        }

        print($"REDISTRIBUTED: Rows = {rows.Count}; Total items = {items.Count}.");
    }

    // Divide la lista en sublistas, cada una representando una fila de la distribucion.
    List<List<GameObject>> OrganizeItems(LinkedList<GameObject> allItems)
    {
        bool hasItemsLeft = true;
        List<List<GameObject>> rows = new();

        List<GameObject> i = new();
        while (hasItemsLeft)
        {
            int c = allItems.Count;
            List<GameObject> newRow = new();

            foreach (var item in allItems.Skip(i.Count))
            {
                newRow.Add(item);
                i.Add(item);
                if (newRow.Count >= xSize || i.Count >= c) break;
            }

            if (rows.Count <= ySize) rows.Add(newRow); else print("Not enough space for your items.");
            if (i.Count >= c) hasItemsLeft = false;
        }

        return rows;
    }

    #endregion
}
