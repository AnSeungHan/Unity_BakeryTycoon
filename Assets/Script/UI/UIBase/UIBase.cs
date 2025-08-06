using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase : MonoBehaviour
{
    [SerializeField]
    private string      uiTag;

    protected void Awake()
    {
        GameManager.Instance.Regist_HUD(uiTag, this);
    }

    public abstract void UIUpdate();
}
