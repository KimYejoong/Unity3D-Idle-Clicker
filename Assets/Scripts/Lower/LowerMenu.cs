using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LowerMenu : MonoBehaviour
{
    [SerializeField]
    private MainButtons mainButtons;

    [SerializeField]
    private TopLeftDisplay topLeftDisplay;

    private Animator _animator;    

    private enum Tabs
    {
        Character,
        Skill,
        Item
    }

    [SerializeField]
    private ListPanelController characterListController;
    [SerializeField]
    private ListPanelController itemListController;
    [SerializeField]
    private ListPanelController skillListController;

    private Tabs currentTab = Tabs.Character; // 기본값은 캐릭터 탭

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _animator.Play("Close", -1, 1.0f); // 게임 시작 시 보이지 않도록 닫기 애니메이션이 즉시 완료된 상태로 실행
    }

    public void Open()
    {
        topLeftDisplay.Open();        

        _animator.Play("Open");

        CloseAllTabs();
        
       if (currentTab == Tabs.Character)
        {            
            characterListController.OpenTab();
        }

        if (currentTab == Tabs.Item)
        {         
            itemListController.OpenTab();
        }

        if (currentTab == Tabs.Skill)
        {         
            skillListController.OpenTab();
        }
    }

    public void CloseAllTabs()
    {
        if (characterListController != null)
            characterListController.CloseTab();

        if (itemListController != null)
            itemListController.CloseTab();

        if (skillListController != null)
            skillListController.CloseTab();
    }

    public void Close()
    {
        
        topLeftDisplay.Close();
        StartCoroutine(CloseAfterDelay());
    }

    private IEnumerator CloseAfterDelay()
    {
        _animator.SetTrigger("Close");
        yield return new WaitForSeconds(0.2f);
        _animator.ResetTrigger("Close");        

       mainButtons.Open();        
    }
}
