using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Money : UIBase
{
    [SerializeField]
    private Text txtMoneyCount;

    public override void UIUpdate()
    {
        txtMoneyCount.text = GameManager.Instance.Player.Money.ToString();
    }
}
