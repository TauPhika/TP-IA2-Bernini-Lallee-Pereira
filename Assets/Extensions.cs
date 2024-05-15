using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public static class Extensions
{
    public static void RescaleItem(this GameObject i, float itemScale)
    {
        i.transform.localScale *= itemScale;
        i.GetComponentInParent<HorizontalLayoutGroup>().padding.left *= (int)itemScale;
        i.GetComponentInParent<HorizontalLayoutGroup>().padding.right *= (int)itemScale;
        i.GetComponentInParent<VerticalLayoutGroup>().padding.top *= (int)itemScale;
        i.GetComponentInParent<VerticalLayoutGroup>().padding.bottom *= (int)itemScale;
        i.GetComponentInParent<HorizontalLayoutGroup>().spacing *= itemScale;
        i.GetComponentInParent<VerticalLayoutGroup>().spacing *= itemScale;
    }

    public static IEnumerable<GameObject> OrderByTextMatch(this IEnumerable<Item> col, string inputText)
    {
        var newCol = col;

        foreach (var c in inputText)
        {
            if (inputText.Skip(inputText.IndexOf(c) - 1).FirstOrDefault() != default)
            {
                newCol.OrderByDescending(x => x.itemName.Contains(inputText.Skip(inputText.IndexOf(c) - 1).First()));
            }
        }

        return newCol.Select(x => x.gameObject);

    }
}
