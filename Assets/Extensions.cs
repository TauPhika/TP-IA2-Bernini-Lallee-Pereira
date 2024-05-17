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
        IEnumerable<GameObject> newCol;

        if (firstToLast) 
        {
            var armorItems = col.Select(x => x.GetComponent<Item>()).OfType<ItemArmor>().OrderBy(x => x.name.First()).ThenBy(x => x.name.Skip(1).FirstOrDefault());
            var potionItems = col.Select(x => x.GetComponent<Item>()).OfType<ItemPotion>().OrderBy(x => x.name.First()).ThenBy(x => x.name.Skip(1).FirstOrDefault());
            var weaponItems = col.Select(x => x.GetComponent<Item>()).OfType<ItemWeapon>().OrderBy(x => x.name.First()).ThenBy(x => x.name.Skip(1).FirstOrDefault());

            newCol = armorItems.Select(x => x.gameObject).
                                Concat(potionItems.Select(x => x.gameObject)).
                                Concat(weaponItems.Select(x => x.gameObject));
        }
        else
        {
            var armorItems = col.Select(x => x.GetComponent<Item>()).OfType<ItemArmor>().OrderByDescending(x => x.name.First()).ThenByDescending(x => x.name.Skip(1).FirstOrDefault());
            var potionItems = col.Select(x => x.GetComponent<Item>()).OfType<ItemPotion>().OrderByDescending(x => x.name.First()).ThenByDescending(x => x.name.Skip(1).FirstOrDefault());
            var weaponItems = col.Select(x => x.GetComponent<Item>()).OfType<ItemWeapon>().OrderByDescending(x => x.name.First()).ThenByDescending(x => x.name.Skip(1).FirstOrDefault());

            newCol = weaponItems.Select(x => x.gameObject).
                                Concat(potionItems.Select(x => x.gameObject)).
                                Concat(armorItems.Select(x => x.gameObject));
        }

        return newCol;
    }

    public static IEnumerable<GameObject> ByPrice(this IEnumerable<GameObject> col, bool cheapToExpensive = true)
    {
        IEnumerable<GameObject> newCol;
        
        if (cheapToExpensive) 
        { 
            var regularCol = col.Where(x => !x.GetComponent<Item>().isPremium).OrderBy(x => x.GetComponent<Item>().price);
            var premiumCol = col.Where(x => x.GetComponent<Item>().isPremium).OrderBy(x => x.GetComponent<Item>().price);

            newCol = regularCol.Concat(premiumCol);
        }
        else
        {
            var regularCol = col.Where(x => !x.GetComponent<Item>().isPremium).OrderByDescending(x => x.GetComponent<Item>().price);
            var premiumCol = col.Where(x => x.GetComponent<Item>().isPremium).OrderByDescending(x => x.GetComponent<Item>().price);

            newCol = premiumCol.Concat(regularCol);
        }

        return newCol;
    }

}
