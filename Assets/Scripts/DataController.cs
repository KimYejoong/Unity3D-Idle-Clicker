using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Globalization;
using System.Text;
// using System.Numerics;
using System.Linq;
using Purchasables;

public class DataController : MonoBehaviour
{
    #region Singleton
    private static DataController _instance;

    public static DataController Instance
    {
        get
        {
            if (_instance != null) 
                return _instance;
            
            _instance = FindObjectOfType<DataController>();

            if (_instance != null)
                return _instance;
            
            var container = new GameObject("DataController");
            _instance = container.AddComponent<DataController>();

            return _instance;
        }
    }
    #endregion
    
    private CharacterButton[] _characterButtons; // 종료 후 유휴 시간 동안 쌓이는 재화 체크 시 자동 재화 생산 담당하는 캐릭터 버튼을 순회하기 위해 배열로 관리
    private SkillButton[] _skillButtons; // 마찬가지로  유휴 시간 중 스킬 버프가 적용될 수 있으므로 스킬 버튼을 순회하기 위해 배열로 관리
    private DateTime _lastPlayDateWhenStart;

    #region 게임 종료 시점 기록 처리를 위한 메소드들
    private DateTime GetLastPlayDate()
    {
        if (!PlayerPrefs.HasKey("Time")) {
            return DateTime.Now;
        }

        var timeBinaryInString = PlayerPrefs.GetString("Time");
        var timeBinaryInLong = Convert.ToInt64(timeBinaryInString);
        
        return DateTime.FromBinary(timeBinaryInLong);
    }

    private void UpdateLastPlayDate()
    {
        PlayerPrefs.SetString("Time", DateTime.Now.ToBinary().ToString());
    }

    private void OnApplicationQuit()
    {
        UpdateLastPlayDate();
    }
    #endregion

    #region 도전과제를 위한 이벤트 선언
    public static event Action<double> EarnGold; // 재화 획득 시 업적 달성 등 체크하기 위해 이벤트 사용
    public static event Action<double> FluctGold;
    public static event Action<double> EarningGoldPerSec;
    
    #endregion

    // 재화
    public static double Gold
    {
        get => PlayerPrefsExtended.GetDouble("Gold", 0);
        set
        {
            double fluct = value - PlayerPrefsExtended.GetDouble("Gold", 0);
            FluctGold?.Invoke(fluct);
            
            PlayerPrefsExtended.SetDouble("Gold", value);
            EarnGold?.Invoke(value);
            
        }
    }
    
    
    
    public double GoldPerClick
    {
        get => PlayerPrefsExtended.GetDouble(("GoldPerClick"), 1); // 클릭 시 획득 재화는 최소 1 이상이어야 하므로 기본값 1
        set => PlayerPrefsExtended.SetDouble("GoldPerClick", value);
    }

    public int maximumIdleTime = 6 * 3600; // 유휴 기간 보상 최대 누적 시간(초)
    
    public int TimeAfterLastPlay
    {
        get
        {
            var currentTime = DateTime.Now;
            return Mathf.Min(maximumIdleTime, (int)currentTime.Subtract(_lastPlayDateWhenStart).TotalSeconds); // 현재 접속 시점 ~ 최종 접속 시점의 시간 간격(초), 최대 6시간
        }
    }

    private double _goldEarnedDuringIdleTime;

    private void Awake()
    {        
        _characterButtons = FindObjectsOfType<CharacterButton>();
        _skillButtons = FindObjectsOfType<SkillButton>();
        _goldEarnedDuringIdleTime = 0;
        _lastPlayDateWhenStart = GetLastPlayDate();
        // GetLasPlayDate()는 비상 종료를 위해 게임 시작 후 UpdateLastPlayDate()에 의해 기록이 갱신되기 시작하므로, 최종 접속 시각은 Awake에서 한번 받아놓고 계속 씀
    }

    private void Start()
    {
        CalculateGoldEarnedDuringIdle(); // Awake에서 각 스킬 버튼의 remaining 등의 정보를 로드한 후, DataController의 Start에서 미접속 기간 동안 획득한 재화의 양을 계산하면서 remaining 차감 처리함
        // 실제 재화 추가는 AddGoldEarnedFromIdle()을 통해 이뤄지고, 게임 시작 시 띄우는 별도의 팝업창을 통해 획득할 수 있도록 GetIdleBonusButton에서 호출함
        
        InvokeRepeating(nameof(UpdateLastPlayDate), 0f, 5f); // 예기치 못한 종료를 대비하여 매 5초마다 최종 접속 시각 기록하도록 함
    }

    #region Purchasables Save & Data
    public static void LoadUpgradeButton(UpgradeButton upgradeButton)
    {
        var key = upgradeButton.upgradeName;
        upgradeButton.level = PlayerPrefs.GetInt(key + "_level", 1);
        upgradeButton.goldByUpgrade = PlayerPrefsExtended.GetDouble(key + "_goldByUpgrade", upgradeButton.initialGoldByUpgrade);
        upgradeButton.currentCost = PlayerPrefsExtended.GetDouble(key + "_cost", upgradeButton.initialCurrentCost);
    }

    public static void SaveUpgradeButton(UpgradeButton upgradeButton)
    {
        var key = upgradeButton.upgradeName;
        PlayerPrefs.SetInt(key + "_level", upgradeButton.level);
        PlayerPrefsExtended.SetDouble(key + "_goldByUpgrade", upgradeButton.goldByUpgrade);
        PlayerPrefsExtended.SetDouble(key + "_cost", upgradeButton.currentCost);
    }

    public static void LoadCharacterButton(CharacterButton characterButton)
    {
        var key = characterButton.characterName;
        characterButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        characterButton.currentCost = PlayerPrefsExtended.GetDouble(key + "_cost", characterButton.initialCurrentCost);
        characterButton.goldPerSec = PlayerPrefsExtended.GetDouble(key + "_goldPerSec");
        characterButton.isPurchased = (PlayerPrefs.GetInt(key + "_isPurchased") == 1);        
    }

    public static void SaveCharacterButton(CharacterButton characterButton)
    {
        var key = characterButton.characterName;
        PlayerPrefs.SetInt(key + "_level", characterButton.level);
        PlayerPrefsExtended.SetDouble(key + "_cost", characterButton.currentCost);
        PlayerPrefsExtended.SetDouble(key + "_goldPerSec", characterButton.goldPerSec);
        PlayerPrefs.SetInt(key + "_isPurchased", characterButton.isPurchased ? 1 : 0);
    }


    public static void LoadSkillButton(SkillButton skillButton)
    {
        var key = skillButton.skillName;
        skillButton.level = PlayerPrefs.GetInt(key + "_level", 0);
        skillButton.currentCost = PlayerPrefsExtended.GetDouble(key + "_cost", skillButton.initialCurrentCost);
        skillButton.goldMultiplier = PlayerPrefs.GetFloat(key + "_goldMultiplier");
        skillButton.remaining = PlayerPrefs.GetFloat(key + "_remaining", 0);        
        skillButton.cooldownRemaining = PlayerPrefs.GetFloat(key + "_cooldownRemaining", 0);        
        skillButton.isPurchased = (PlayerPrefs.GetInt(key + "_isPurchased") == 1);
        skillButton.isActivated = (PlayerPrefs.GetInt(key + "_isActivated") == 1);
    }

    public static void SaveSkillButton(SkillButton skillButton)
    {
        var key = skillButton.skillName;
        PlayerPrefs.SetInt(key + "_level", skillButton.level);
        PlayerPrefsExtended.SetDouble(key + "_cost", skillButton.currentCost);
        PlayerPrefs.SetFloat(key + "_goldMultiplier", skillButton.goldMultiplier);
        PlayerPrefs.SetFloat(key + "_remaining", skillButton.remaining);        
        PlayerPrefs.SetFloat(key + "_cooldownRemaining", skillButton.cooldownRemaining);

        PlayerPrefs.SetInt(key + "_isPurchased", skillButton.isPurchased ? 1 : 0);
        PlayerPrefs.SetInt(key + "_isActivated", skillButton.isActivated ? 1 : 0);
    }
    #endregion


    public double GetGoldPerSecond()
    {
        double goldPerSec = 0;

        for (int i = 0; i < _characterButtons.Length; i++)
        {
            if (_characterButtons[i].isPurchased) // 구매한 아이템일 경우에만 계산에 고려함
                goldPerSec += _characterButtons[i].goldPerSec;
        }
        
        EarningGoldPerSec?.Invoke(goldPerSec); // 도전과제 달성 여부를 체크하기 위해 액션 호출

        return goldPerSec;
    }

    public float GetGoldMultiplier()
    {
        float goldMultiplier = 1;

        for (var i = 0; i < _skillButtons.Length; i++)
        {
            if (_skillButtons[i].isPurchased && _skillButtons[i].isActivated) // 해당 스킬을 구매했고, 현재 사용했을 경우에만 고려                
                goldMultiplier *= _skillButtons[i].goldMultiplier;
        }

        return goldMultiplier;
    }

    private void CalculateGoldEarnedDuringIdle()
    {
        var goldMultipliers = new List<SkillData>();

        for (var i = 0; i < _skillButtons.Length; i++)
        {
            if (!_skillButtons[i].isPurchased)
                continue;
            
            if (_skillButtons[i].isActivated)
            {
                goldMultipliers.Add(new SkillData(_skillButtons[i].remaining, _skillButtons[i].goldMultiplier)); // 잔여 시간 정보는 필요하니 SkillData로 넘기고
                _skillButtons[i].remaining = Mathf.Max(_skillButtons[i].remaining - TimeAfterLastPlay, 0); // 지난 시간만큼 해당 스킬 버튼의 잔여 시간, 잔여 재사용 대기 시간을 감소시킴
            }
            _skillButtons[i].cooldownRemaining = Mathf.Max(_skillButtons[i].cooldownRemaining - TimeAfterLastPlay, 0);
        }        

        if (goldMultipliers.Count == 0) // 적용되는 스킬 버프가 따로 없었으면 그냥 지난 시간만큼 바로 처리하고,
        {
            _goldEarnedDuringIdleTime += GetGoldPerSecond() * TimeAfterLastPlay;
        }
        else // 스킬 버프 적용 중에 종료했다 다시 켠 경우 버프를 고려하여 획득 재화를 계산함
        {               
            var prevRemaining = 0f;
            
            goldMultipliers = goldMultipliers.OrderBy(skill => skill.remaining).ToList(); // remaining 오름차순으로 정렬하여 버프가 가장 많이 겹치는 구간 ~ 적게 겹치는 구간 순으로 처리

            for (var i = 0; i < goldMultipliers.Count; i++)
            {
                var tempMultiplier = 1f; // 기본 배수를 일단 1배율로 잡아주고,
                var tempRemaining = goldMultipliers[i].remaining - prevRemaining; // i번째 구간만의 remaining을 구함 (이전 구간분 차감)

                for (var j = i; j < goldMultipliers.Count; j++)
                {
                    tempMultiplier *= goldMultipliers[j].multiplier; // 이전 구간을 제외하고(j = i ~ Count), 적용 가능한 버프 계수를 누적해서 곱해줌
                }
                
                prevRemaining = tempRemaining;
                tempRemaining = Mathf.Min(goldMultipliers[i].remaining, TimeAfterLastPlay); // 게임을 껐다 켠지 얼마 안돼서 잔여 시간이 더 긴 경우, 종료 기간분만큼만 계산해줌. 단, 실제 지속 시간은 이미 위에서 처리해줬음

                _goldEarnedDuringIdleTime += GetGoldPerSecond() * tempMultiplier * tempRemaining;
                //Debug.Log("Idle Bonus amplified by buff = " + GetGoldPerSecond() * tempMultiplier * tempRemaining);
            }

            _goldEarnedDuringIdleTime += GetGoldPerSecond() * Mathf.Max(TimeAfterLastPlay - goldMultipliers[goldMultipliers.Count - 1].remaining, 0);
            //Debug.Log("Idle Bonus without buff = " + GetGoldPerSecond() * Mathf.Max(TimeAfterLastPlay - goldMultipliers[goldMultipliers.Count - 1].remaining, 0));
            //Debug.Log("Total Idle time = " + TimeAfterLastPlay);
            //Debug.Log("Idle time without buff = " + Mathf.Max(TimeAfterLastPlay - goldMultipliers[goldMultipliers.Count - 1].remaining, 0));
            // 스킬 버프 적용 시간이 끝난 뒤로, 남아있는 유휴 시간만큼 재화 획득 마저 처리
        }
    }

    public void AddGoldEarnedFromIdle()
    {
        Gold += _goldEarnedDuringIdleTime;
        _goldEarnedDuringIdleTime = 0;
    }

    public double GetGoldEarnedFromIdle()
    {
        return _goldEarnedDuringIdleTime;
    }

    private class SkillData
    {
        public readonly float remaining;
        public readonly float multiplier;

        public SkillData(float time, float multi)
        {
            remaining = time;
            multiplier = multi;
        }
    }
}
