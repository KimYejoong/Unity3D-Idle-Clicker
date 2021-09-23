using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabNameDisplay : MonoBehaviour
{
    private Text _tabNameDisplayText;

    // Start is called before the first frame update
    private void Awake()
    {
        _tabNameDisplayText = GetComponentInChildren<Text>();
    }

    public void UpdateTabName(string tabName)
    {
        _tabNameDisplayText.text = tabName;
    }

}
