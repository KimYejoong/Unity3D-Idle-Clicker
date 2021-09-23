using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AchievementDisplayManager : MonoBehaviour
{
    [SerializeField] private Transform contentsPanel;
    [SerializeField] private GameObject contents; // 개개의 도전과제 항목을 표시하는 게임 오브젝트 프리팹

    
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        StartCoroutine(Init());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();

        foreach (var key in AchievementSystem.Instance._achievements.Keys)
        {
            var achievementGO = Instantiate(contents, contentsPanel);
            var achievementDisplayHandle = achievementGO.GetComponent<AchievementDisplayInstance>();
            var achievement = AchievementSystem.Instance._achievements[key];
            achievementDisplayHandle.Init(achievement);
        }
    }
}