using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public class Coin : MonoBehaviour, IPoolObject
{
    public CoinType type;

    private Animator anim;

    private GameManager gameManager;

    public void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
        gameManager = GameManager.Instance;
    }

    public void OnGettingFromPool()
    {
    }

    public void UpdateAnim(int money)
    {
        int[] coinUnit = gameManager.coinUnit; // 돈 단위 받아오기

        for(int i = 0; i < coinUnit.Length; i++)
        {
            if (money > coinUnit[i])
                type++;
            else
                break;
        }

        anim.SetInteger("Change", (int)type); // 해당 돈 애니메이션 실행
    }
}

public enum CoinType { Bronze = 0, Silver, Gold }