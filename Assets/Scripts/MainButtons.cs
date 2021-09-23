using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainButtons : MonoBehaviour
{
    private Vector3 _originalPos;

    private void Awake()
    {
        _originalPos = transform.position;
    }
    
    // Start is called before the first frame update
    public void Open()
    {
        transform.position = _originalPos;
    }

    public void Close()
    {
        transform.position = Vector3.up * 1000f;
    }
}
