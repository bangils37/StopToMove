using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIVfx : Singleton<UIVfx>
{
    [SerializeField] Image bg;
    private Vector3 RandomDirect => Vector3.right * (Random.value - 0.5f) * 200 + Vector3.up * (Random.value - 0.5f) * 200;
    public Vector3 CoinPoint => UIManager.Ins.GetUI<UIMainMenu>().CoinPoint.position;

    private MiniPool<FlyCoin> flyCoinPool = new MiniPool<FlyCoin>();
    [SerializeField] Transform flyCoinContain;
    [SerializeField] FlyCoin flyCoinPrefab;

    private void Awake()
    {
        flyCoinPool.OnInit(flyCoinPrefab, 9, flyCoinContain);
    }

    public void AddCoin(int amount, int coin, Vector3 startPoint, Vector3 finishPoint)
    {
        int coinUnit = coin / amount;

        for (int i = 0; i < amount; i++)
        {
            coinUnit = i == amount - 1 ? coinUnit + coin % amount : coinUnit;
            flyCoinPool.Spawn().OnInit(startPoint + RandomDirect, finishPoint, null);
        }

        StartCoroutine(IEFade(1.5f, new Color(0, 0, 0, 150f / 255f), Color.clear));
    }

    private IEnumerator IEFade(float time, Color start, Color finish)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            bg.color = Color.Lerp(start, finish, t / time);
            yield return null;
        }
    }
}

