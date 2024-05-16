using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class StoreFilters : MonoBehaviour
{
    public TMP_Dropdown orderDropdown;
    public TMP_Dropdown typeDropdown;
    public TMP_InputField inputField;
    public Toggle inverseToggle;
    public Toggle contextToggle;
    [HideInInspector] public bool inputIsContextSensitive = false;
    [HideInInspector] public bool inverseOrder = false;

    public void UpdateFiltering()
    {
        InverseOrder();
        ContextSensitive();

        var filteredByText = Store.instance.FilterByText(inputField.text, inputIsContextSensitive);

        var filteredByTextThenType = Store.instance.FilterByType(typeDropdown.options[typeDropdown.value], filteredByText);

        Store.instance.OrderByProperty(orderDropdown.options[orderDropdown.value], filteredByTextThenType);
    }

    public void InverseOrder()
    {
        inverseOrder = !inverseToggle.isOn;
        print(inverseOrder);
    }
    
    public void ContextSensitive()
    {
        inputIsContextSensitive = contextToggle.isOn;
        print(inputIsContextSensitive);
    }

}
