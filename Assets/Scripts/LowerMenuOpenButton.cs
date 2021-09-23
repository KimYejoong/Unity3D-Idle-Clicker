using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerMenuOpenButton : MonoBehaviour
{
    [SerializeField]
    private LowerMenu lowerMenuPanel;

    [SerializeField]
    private MainButtons mainButtons;

    public void OpenMenu()
    {
        lowerMenuPanel.Open();
        mainButtons.Close();
    }
}
