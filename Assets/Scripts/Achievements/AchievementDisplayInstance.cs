using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplayInstance : MonoBehaviour
{
    [SerializeField] private Text _textTitle;
    [SerializeField] private Text _textDescription;
    [SerializeField] private Button _getRewardButton;
    [SerializeField] private Slider _slider;

    private Achievement _myAchievement;

    public Achievement MyAchievement
    {
        get { return _myAchievement;  }
    }
    
    private float _progressPercent;
    private bool _unlocked;

    public void Init(Achievement achievement)
    {
        _myAchievement = achievement;
        _textTitle.text = _myAchievement.GetName();
        _textDescription.text = _myAchievement.GetDescription();
        _progressPercent = _myAchievement.GetProgressInPercent();
        _unlocked = _myAchievement.GetUnlocked();

        _myAchievement.ChangeInProgress += UpdateUI;
        
        UpdateUI(_progressPercent, _unlocked);
    }
    
    // Update is called once per frame
    private void UpdateUI(float updatedProgressInPercent, bool updatedUnlocked)
    {
        _progressPercent = updatedProgressInPercent;
        _unlocked = updatedUnlocked;
        _getRewardButton.interactable = _unlocked;
        _slider.value = (_unlocked == true ? 1 : _progressPercent);
    }
}
