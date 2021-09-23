using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AchievementPanelController : MonoBehaviour
{
    private Animator _animator;
    [SerializeField]
    private RectTransform parentRect;
    private Vector3 _originalParentLocation;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _originalParentLocation = Vector3.zero;
        parentRect.anchoredPosition = Vector2.down * 2000f;
    }
    
    public void Open()
    {
        parentRect.transform.position = _originalParentLocation;
        _animator.Play("Open", -1, 0);
    }

    public void Close()
    {
        parentRect.anchoredPosition = Vector2.down * 2000f;
        
    }
}
