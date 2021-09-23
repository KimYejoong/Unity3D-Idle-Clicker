using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetIdleBonusButton : MonoBehaviour
{
    [SerializeField]
    private GameObject idlePopupParent;

    public void OnClick()
    {
        var newInstantText = UIManager.Instance.GenerateInstantText("+"+ DataController.Instance.GetGoldEarnedFromIdle().ToCurrencyString(), 
            UIManager.Instance.goldDisplayText.transform,
            UnityEngine.Vector2.right * (UIManager.Instance.goldDisplayText.preferredWidth) + Vector2.up * (UIManager.Instance.goldDisplayText.preferredHeight + 4f)
            , 0.5f, TextAnchor.UpperRight, true);

        newInstantText._myText.fontSize = UIManager.Instance.goldDisplayText.fontSize;
        newInstantText._myText.color = UIManager.Instance.goldDisplayText.color;
        
        // 순서에 주의, 실제 재화 추가가 이뤄지면 GetGoldEarnedFromIdle()이 0을 반환하므로, 그 전에 인스턴트 텍스트를 만들어서 띄워야 정상적으로 표기됨
        
        DataController.Instance.AddGoldEarnedFromIdle();
        idlePopupParent.SetActive(false);
    }
}
