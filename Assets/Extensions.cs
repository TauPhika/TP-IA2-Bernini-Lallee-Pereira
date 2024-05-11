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
}
