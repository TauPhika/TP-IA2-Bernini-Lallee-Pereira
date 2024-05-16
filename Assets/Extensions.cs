using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public static class Extensions
{
    //public static void RescaleItem(this GameObject i, float itemScale)
    //{
    //    i.transform.localScale *= itemScale;
    //    i.GetComponentInParent<HorizontalLayoutGroup>().padding.left *= (int)itemScale;
    //    i.GetComponentInParent<HorizontalLayoutGroup>().padding.right *= (int)itemScale;
    //    i.GetComponentInParent<VerticalLayoutGroup>().padding.top *= (int)itemScale;
    //    i.GetComponentInParent<VerticalLayoutGroup>().padding.bottom *= (int)itemScale;
    //    i.GetComponentInParent<HorizontalLayoutGroup>().spacing *= itemScale;
    //    i.GetComponentInParent<VerticalLayoutGroup>().spacing *= itemScale;
    //}

    public static IEnumerable<GameObject> OrderByTextMatch(this IEnumerable<Item> col, string inputText)
    {
        var newCol = col;

        foreach (var c in inputText)
        {
            if (inputText.Skip(inputText.IndexOf(c)).FirstOrDefault() != default)
            {
                newCol.OrderByDescending(x => x.itemName.Contains(inputText.Skip(inputText.IndexOf(c)).First()));
            }
        }

        return newCol.Select(x => x.gameObject);

    }

    public static IEnumerable<GameObject> Alphabetically(this IEnumerable<GameObject> col, bool firstToLast = true)
    {
        if(firstToLast) return col.OrderBy(x => x.name); 
        else return col.OrderByDescending(x => x.name);
    } 
    
    public static IEnumerable<GameObject> ByType(this IEnumerable<GameObject> col, bool firstToLast = true)
    {
        if (firstToLast) return col.OrderBy(x => x.GetComponent<Item>().itemType);
        else return col.OrderByDescending(x => x.GetComponent<Item>().itemType);
    }

    public static IEnumerable<GameObject> ByPrice(this IEnumerable<GameObject> col, bool cheapToExpensive = true)
    {
        if(cheapToExpensive) return col.OrderBy(x => x.GetComponent<Item>().price);
        else return col.OrderByDescending(x => x.GetComponent<Item>().price);
    }

}
