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
        int typeID = (int)type; // 돈 타입 int형으로 변환

        // 돈 단위를 넘어가는 경우
        if(money > gameManager.coinUnit[typeID])
        {
            type++; // 단위 증가
            UpdateAnim(money); // 함수 재실행

            return;
        }

        anim.SetInteger("Change", typeID); // 해당 돈 애니메이션 실행
    }
}

public enum CoinType { Bronze = 0, Silver, Gold }