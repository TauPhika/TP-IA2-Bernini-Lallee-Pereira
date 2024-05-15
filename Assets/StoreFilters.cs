using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class StoreFilters : MonoBehaviour
{
    public TMP_Dropdown filterDropdown;
    public TMP_Dropdown typeDropdown;
    public TMP_InputField inputField;
    public bool inputIsContextSensitive;

    public void FilterByInputText()
    {
        Store.instance.FilterByText(inputField.text, inputIsContextSensitive);
    }
}
