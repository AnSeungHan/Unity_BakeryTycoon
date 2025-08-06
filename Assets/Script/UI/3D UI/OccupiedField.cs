using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupiedField : MonoBehaviour
{
    [SerializeField]
    private float     targetScale = 0.03f;

    void Start()
    {
        
    }

    public void Action(bool _IsActivate)
    {
        if (_IsActivate)
            StartCoroutine(ScaleUp());
        else
            StartCoroutine(ScaleDown());
    }

    IEnumerator ScaleUp()
    {
        for (int i = 0; i < 10; ++i)
        { 
            transform.localScale += Vector3.one * targetScale;

            yield return null;
        }
    }

    IEnumerator ScaleDown()
    {
        for (int i = 0; i < 10; ++i)
        {
            transform.localScale -= Vector3.one * targetScale;

            yield return null;
        }
    }
}
