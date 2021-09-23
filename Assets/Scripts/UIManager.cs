using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager _instance;

    public static UIManager Instance
    {
        get
        {
            if (_instance != null)
                return _instance;
            
            _instance = FindObjectOfType<UIManager>();

            if (_instance != null)
                return _instance;
            
            var container = new GameObject("UIManager");
            _instance = container.AddComponent<UIManager>();

            return _instance;
        }
    }
    #endregion
    
    public Text goldDisplayText;
    public Text goldPerClickDisplayText;
    public Text goldPerSecDisplayText;
    public Text goldDisplayTextInLowerPanel;
    public GameObject idleBonusPopup;

    private void Start()
    {
        StartCoroutine(UpdateInstantTexts());
        StartCoroutine(TryShowIdleEarning());
    }

    private IEnumerator TryShowIdleEarning() // 게임 실행 시 유휴 시간 동안 획득한 재화 획득 팝업 표시, 정상 처리를 위해 프레임 마지막까지 대기 후 처리
    {
        yield return new WaitForEndOfFrame();
        if (DataController.Instance.GetGoldEarnedFromIdle() > 0)
        {
            idleBonusPopup.SetActive(true);
        }

    }

    private void Update()
    {
        goldDisplayText.text = "GOLD : " + DataController.Gold.ToCurrencyString();
        goldPerClickDisplayText.text = "GOLD PER CLICK : " + DataController.Instance.GoldPerClick.ToCurrencyString();
        goldPerSecDisplayText.text = "GOLD PER SEC : " + DataController.Instance.GetGoldPerSecond().ToCurrencyString();
        goldDisplayTextInLowerPanel.text = "GOLD : " + DataController.Gold.ToCurrencyString();
    }

    private IEnumerator UpdateInstantTexts()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (DataController.Instance.GetGoldPerSecond() <= 0)
                continue;
            
            // 화면 좌하단에 재화 실시간 획득량 표시
            var goldText = GenerateInstantText("+" + (DataController.Instance.GetGoldPerSecond() * DataController.Instance.GetGoldMultiplier()).ToCurrencyString(),
                goldDisplayText.transform,
                Vector2.right * (goldDisplayText.preferredWidth + 4f), 0.5f, TextAnchor.UpperLeft, true);
            goldText._myText.fontSize = goldDisplayText.fontSize;
            goldText._myText.color = goldDisplayText.color;

            // 하단 메뉴 띄웠을 때 좌상단에 재화 실시간 획득량 표시
            GenerateInstantText("+" + (DataController.Instance.GetGoldPerSecond() * DataController.Instance.GetGoldMultiplier()).ToCurrencyString(),
                goldDisplayTextInLowerPanel.transform,
                Vector2.right * (goldDisplayTextInLowerPanel.preferredWidth + 4f), 0.5f);
        }
    }

    


    public InstantText GenerateInstantText(string content, Transform trans, Vector3 pos, float time, TextAnchor textAnchor = TextAnchor.UpperLeft, bool outline = false)
    {
        var newInstantText = GetObject(); 
        newInstantText.transform.SetParent(trans);
        var handle = newInstantText.GetComponent<InstantText>();
        handle.Initialize(content, time, textAnchor);
        handle.transform.localPosition = pos;
        handle._myText.GetComponent<Outline>().enabled = outline;

        return newInstantText;
    }

    #region  Object Pooling

    [SerializeField]
    private GameObject instantText;

    [SerializeField]
    private int poolSize;
    

    readonly Queue<InstantText> _pool = new Queue<InstantText>();

    private void Awake()
    {
        Initialize(poolSize);
    }

    private void Initialize(int initCount)
    {
        for (var i = 0; i < initCount; i++)
        {
            _pool.Enqueue(CreateNewObject());
        }
    }

    private InstantText CreateNewObject()
    {
        var newObj = Instantiate(instantText).GetComponent<InstantText>();
        newObj.gameObject.SetActive(false);
        newObj.transform.SetParent(transform);
        return newObj;
    }

    public InstantText GetObject()
    {
        if (Instance._pool.Count > 0)
        {
            var obj = Instance._pool.Dequeue();
            // obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            return obj;
        }
        else // 오브젝트 풀의 제한치를 초과할 경우, 작동을 위해 일단 만들지만 디버그를 위해 경고 표시
        {
            Debug.LogError("Exceed pool limitation!");

            var newObj = Instance.CreateNewObject();
            newObj.transform.SetParent(null);
            newObj.gameObject.SetActive(true);
            return newObj;
        }
    }
    public void ReturnObject(InstantText obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(Instance.transform);
        Instance._pool.Enqueue(obj);
    }
    
    #endregion
}

