using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementMenuOpenButton : MonoBehaviour
{
    [SerializeField] private AchievementPanelController AchievementPanel;

    public void OnClick()
    {
        AchievementPanel.Open();
    }
}
