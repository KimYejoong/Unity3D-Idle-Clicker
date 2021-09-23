using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;


public class InstantText : MonoBehaviour
{
    [HideInInspector]
    public Text _myText;
    private float _lifetime;
    private Color _originalColor;
    private int _originalFontSize;

    private void Awake()
    {
        _myText = GetComponentInChildren<Text>();
        _originalColor = _myText.color;
        _originalFontSize = _myText.fontSize;
    }

    public void Initialize(string content, float lifetime, TextAnchor textAnchor)
    {
        _lifetime = lifetime;
        _myText.text = content;
        _myText.alignment = textAnchor;
        _myText.color = _originalColor;
        _myText.fontSize = _originalFontSize;
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float offset = Time.deltaTime / _lifetime;
        for (float f = 1f; f >= 0; f -= offset)
        {
            Color c = _myText.color;
            c.a = f;
            _myText.color = c;

            yield return null;
        }
        UIManager.Instance.ReturnObject(this);
    }
}
