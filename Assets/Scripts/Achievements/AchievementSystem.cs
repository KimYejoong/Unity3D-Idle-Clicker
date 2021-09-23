using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementSystem : MonoBehaviour
{
    #region Singleton
    private static AchievementSystem _instance;

    public static AchievementSystem Instance
    {
        get
        {
            if (_instance != null) 
                return _instance;
            
            _instance = FindObjectOfType<AchievementSystem>();

            if (_instance != null)
                return _instance;
            
            var container = new GameObject("AchievementSystem");
            _instance = container.AddComponent<AchievementSystem>();

            return _instance;
        }
    }
    #endregion

    private Queue<Achievement> _achievementDisplayQueue;
    private GameObject achievementButton;
    public Dictionary<string, Achievement> _achievements = new Dictionary<string, Achievement>();

    private void Awake()
    {
        InitializeAchievements();
        
        DataController.EarnGold += AchieveEarnGold;
        DataController.EarningGoldPerSec += AchieveEarningGoldPerSec;
        DataController.FluctGold += AchieveGoldFluct;
        SkillButton.SkillActivated += AchieveSkillUse;

        _achievementDisplayQueue = new Queue<Achievement>();

        StartCoroutine(ShowAchievementInstant());
    }
    
    private void InitializeAchievements()
    {
        _achievements.Add("Earned_10_gold", new Achievement("십원", 0, 10, false, "10골드 모으기"));
        _achievements.Add("Earned_100_gold", new Achievement("백원", 0, 100, false, "100골드 모으기"));
        _achievements.Add("Earned_500_gold", new Achievement("오백원", 0, 500, false, "500골드 모으기"));
        
        _achievements.Add("Earning_10_gold_per_sec", new Achievement("불로소득", 0, 10, false, "초당 획득 골드 10 달성하기"));
        _achievements.Add("Earning_50_gold_per_sec", new Achievement("숨쉬고 돈벌기", 0, 50, false, "초당 획득 골드 50 달성하기"));
        _achievements.Add("Earning_150_gold_per_sec", new Achievement("티끌모아 태산", 0, 150, false, "초당 획득 골드 150 달성하기"));
        
        _achievements.Add("Use_1_gold", new Achievement("알뜰한 소비", 0, -1, false, "1골드 사용하기"));
        _achievements.Add("Use_50_gold", new Achievement("FLEX", 0, -50, false, "50골드 사용하기"));
        
        _achievements.Add("Earn_10_gold_at_once", new Achievement("동전 줍기", 0, 10, false, "한번에 10골드 이상 획득"));
        _achievements.Add("Earn_200_gold_at_once", new Achievement("로또 5등", 0, 200, false, "한번에 200골드 이상 획득"));

        _achievements.Add("Use_skill_3_times", new Achievement("스킬 3회 사용하기", 0, 3, false, "스킬 3회 사용하기"));
        
        if (PlayerPrefs.HasKey("Time")) // 이전에 플레이한 적이 있을 경우에만 로드. PlayerPref 사용하므로 조건 안 걸어주면 goal 값이 기본값인 0이 돼버림
            LoadAllAchievements();
    }
    

    #region 도전과제 데이터 일괄 저장 및 불러오기
    private void LoadAllAchievements()
    {
        foreach (var keys in _achievements.Keys)
        {
            _achievements[keys].LoadAchievement();
        }
    }
    
    private void SaveAllAchievements()
    {
        foreach (var keys in _achievements.Keys)
        {
            _achievements[keys].SaveAchievement();
        }
    }
    
    private void OnApplicationQuit()
    {
        SaveAllAchievements();
    }
    #endregion



    private void AchieveEarnGold(double amount) // 소지 재화가 일정 수치에 도달하면 달성 가능한 도전과제
    {
        //if (amount >= 10)
            _achievements["Earned_10_gold"].TryAchieve(amount);
        
        //if(amount>= 100)
            _achievements["Earned_100_gold"].TryAchieve(amount);
        
        //if(amount>= 500)
            _achievements["Earned_500_gold"].TryAchieve(amount);
    }

    private void AchieveEarningGoldPerSec(double amount) // GoldPerSec가 일정 수치에 도달하면 달성 가능한 도전과제
    {
        //if (amount >= 10)
            _achievements["Earning_10_gold_per_sec"].TryAchieve(amount);
        
        //if (amount >= 50)
            _achievements["Earning_50_gold_per_sec"].TryAchieve(amount);
        
        //if (amount >= 150)
            _achievements["Earning_150_gold_per_sec"].TryAchieve(amount);
    }

    private void AchieveGoldFluct(double amount) // 한번에 얻거나 소비한 재화의 양에 따른 도전과제 처리
    {
        //if (amount <= -1)
            _achievements["Use_1_gold"].TryAchieve(amount);
        
        //if (amount <= -50)
            _achievements["Use_50_gold"].TryAchieve(amount);
        
        //if (amount >= 10)
            _achievements["Earn_10_gold_at_once"].TryAchieve(amount);
        
        //if (amount >= 200)
            _achievements["Earn_200_gold_at_once"].TryAchieve(amount);
    }

    private void AchieveSkillUse() // 스킬 사용에 따른 도전과제 처리
    {
        string achieveName = "Use_skill_3_times";
        var nextProgress = _achievements[achieveName].GetProgress() + 1;
        var goal = _achievements[achieveName].GetGoal();
        _achievements[achieveName].TryAchieve(nextProgress);
    }

    public void AddAchievedQueue(Achievement achievement)
    {
        _achievementDisplayQueue.Enqueue(achievement);
    }

    IEnumerator ShowAchievementInstant() // 한꺼번에 여러 도전과제 달성 시, 겹치지 않게 큐에 넣어서 순차적으로 보여줌
    {
        if (_achievementDisplayQueue.Count > 0)
        {
            var achievement = _achievementDisplayQueue.Dequeue();
            var newInstantText = UIManager.Instance.GenerateInstantText("도전과제 \"" + achievement.GetName() + "\" 달성!",
                UIManager.Instance.transform,
                Vector3.up * 240, 2f, TextAnchor.MiddleCenter, true);
            newInstantText._myText.fontSize = 36;
            newInstantText._myText.color = Color.white;
        }

        yield return new WaitForSeconds(3f);

        StartCoroutine(ShowAchievementInstant());
    }
}

    public class Achievement
    {
        private string _name;
        private double _progress;
        private double _goal;
        private bool _unlocked;
        private string _description;

        public event Action<float, bool> ChangeInProgress;

        public Achievement(string name, double progress, double goal, bool unlocked, string description)
        {
            _name = name;
            _progress = progress;
            _goal = goal;
            _unlocked = unlocked;
            _description = description;
        }

        public double GetProgress()
        {
            return _progress;
        }

        public double GetGoal()
        {
            return _goal;
        }
        
        public float GetProgressInPercent()
        {
            return (float)(_progress / _goal);
        }

        public string GetDescription()
        {
            return _description;
        }

        public string GetName()
        {
            return _name;
        }

        public bool GetUnlocked()
        {
            return _unlocked;
        }

        public void TryAchieve(double progress)
        {
            _progress = progress;

            if (_unlocked)
                return;

            if (Math.Abs(_progress) >= Math.Abs((_goal)) &&
                Math.Sign(_progress) == Math.Sign(_goal)) // 골드 획득(+)과 소모(-)에 따른 처리를 한번에 하기 위해 절대값 처리 및 부호 구분
            {
                _unlocked = true;
                AchievementSystem.Instance.AddAchievedQueue(this);
                SaveAchievement();
            }

            ChangeInProgress?.Invoke((float) (_progress / _goal), _unlocked);
        }


        // 도전과제 내용 설명은 진행에 따라 변경되는 게 아니므로 굳이 저장, 불러오기 하지 않음
        public void LoadAchievement()
        {
            string key = _name;

            _progress = PlayerPrefsExtended.GetDouble(key + "_progress");
            _goal = PlayerPrefsExtended.GetDouble(key + "_goal");
            _unlocked = PlayerPrefs.GetInt(key + "_unlocked") == 1;
        }

        public void SaveAchievement()
        {
            string key = _name;

            PlayerPrefsExtended.SetDouble(key + "_progress", _progress);
            PlayerPrefsExtended.SetDouble(key + "_goal", _goal);
            PlayerPrefs.SetInt(key + "_unlocked", _unlocked ? 1 : 0);
        }
    }