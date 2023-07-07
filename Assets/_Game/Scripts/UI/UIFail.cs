using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIFail : UICanvas
{
    private int coin;
    [SerializeField] TextMeshProUGUI coinTxt;
    [SerializeField] RectTransform x3Point;
    [SerializeField] RectTransform mainMenuPoint;

    public override void Open()
    {
        base.Open();
        GameManager.Ins.ChangeState(GameState.Finish);
    }

    public void x3Button()
    {
        LevelManager.Ins.Home();
        UIVfx.Ins.AddCoin(9, coin * 3, x3Point.position, UIVfx.Ins.CoinPoint);
    }

    public void MainMenuButton()
    {
        LevelManager.Ins.Home();
        UIVfx.Ins.AddCoin(3, coin, mainMenuPoint.position, UIVfx.Ins.CoinPoint);
    }

    internal void SetCoin(int coin)
    {
        this.coin = coin;
        coinTxt.SetText(coin.ToString());
    }
}
