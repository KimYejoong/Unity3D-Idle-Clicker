using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TopLeft;

public class BuffDisplayManager : MonoBehaviour
{
    #region Singleton
    private static BuffDisplayManager _instance;

    public static BuffDisplayManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            
            _instance = FindObjectOfType<BuffDisplayManager>();

            if (_instance != null)
                return _instance;
            
            var container = new GameObject("BuffDisplayManager");
            _instance = container.AddComponent<BuffDisplayManager>();

            return _instance;
        }
    }
    #endregion

    [SerializeField]
    private GameObject buffDisplayText; // 버프 표시 게임오브젝트 Prefab
    [SerializeField]
    private int spacing = 3; // 줄 간격

    private List<BuffDisplayText> list;

    private void Awake()
    {
        list = new List<BuffDisplayText>();
    }

    private void Start()
    {
        StartCoroutine(SortWhenLoaded());
        // SkillButton에서 Fixed Update 시점에 버프 표시 오브젝트를 생성하므로, Start에서 바로 정렬을 시도해도 정렬할 대상이 없음. 따라서 프레임 종료 시점까지 대기 후 정렬 시도
        StartCoroutine(CorutineUpdate());    
    }

    private IEnumerator SortWhenLoaded()
    {
        yield return new WaitForEndOfFrame();
        SortBuffDisplay();
    }
    
    IEnumerator CorutineUpdate() // BuffDisplayText에게 업데이트 시도를 맡길 경우 초 감소가 각기 이뤄져 표시가 산만해지므로, 이곳에서 1초마다 일괄 처리 해줌
    {
        while (true)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].UpdateDisplayText();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    
    public void AddBuffDisplay(SkillButton skill)
    {
        var newBuffDisplayGo = Instantiate(buffDisplayText, transform);
        var newBuffDisplay = newBuffDisplayGo.GetComponent<BuffDisplayText>();
        newBuffDisplay.Initialize(this, skill);
        newBuffDisplay.transform.position = transform.position + Vector3.down * (newBuffDisplay.displayText.preferredHeight + spacing) * list.Count;

        list.Add(newBuffDisplay);
        SortBuffDisplay();
    }

    public void UpdateBuffDisplayList(BuffDisplayText buffDisplay)
    {
        list.Remove(buffDisplay);

        for (var i = 0; i < list.Count; i++)
        {
            list[i].transform.position = transform.position + Vector3.down * (list[i].displayText.preferredHeight + spacing) * i;
        }
    }

    private void SortBuffDisplay()
    {
        list = list.OrderBy(item => item.skill.remaining).ToList();

        for (var i = 0; i < list.Count; i++)
        {
            list[i].transform.position = transform.position + Vector3.down * (list[i].displayText.preferredHeight + spacing) * i;
        }
    }
    
}
