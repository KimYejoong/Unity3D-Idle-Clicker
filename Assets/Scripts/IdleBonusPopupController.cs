using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IdleBonusPopupController : MonoBehaviour
{
    [SerializeField]
    private Text idleBonusDisplay;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        //_animator.Play("Open", -1);
    }

    void Start()
    {
        idleBonusDisplay.text = "자리를 비운 " +  GetHMSFormat(DataController.Instance.TimeAfterLastPlay) + "동안 획득한 재화 : " +
                              DataController.Instance.GetGoldEarnedFromIdle().ToCurrencyString() +
        "\n보상은 최대 " + GetHMSFormat(DataController.Instance.maximumIdleTime) + "동안 누적됩니다.";
    }

    private string GetHMSFormat(int time)
    {
        var Time = time;

        var hour = (int)(Time / 3600);
        var min = (int)(Time - hour * 3600) / 60;
        var sec = (int)(Time % 60);
        
        return ((hour > 0) ? hour + "시간 " : "") + ((min > 0) ? min + "분 " : "") + ((sec > 0) ? sec + "초 " : "");         
    }
}
