using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEffect : MonoBehaviour
{
    private ParticleSystem _ps;

    private void Awake()
    {
        _ps = GetComponent<ParticleSystem>();        
    }

    public void Initialize()
    {
        _ps.time = 0f; // 이펙트 처음으로 초기화
        _ps.Play();
        StartCoroutine(AutoReturn());        
    }

    private IEnumerator AutoReturn()
    {
        yield return new WaitForSeconds(_ps.main.duration);        
        EffectManager.Instance.ReturnObject(this);
    }
}
