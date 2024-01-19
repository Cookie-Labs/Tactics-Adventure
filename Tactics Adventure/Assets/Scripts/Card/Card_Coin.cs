using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Coin : Card
{
    private Coin coin;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
    }

    public override void OnGettingFromPool()
    {
        base.OnGettingFromPool();
    }

    public override void SetCard()
    {
        coin = spawnManager.SpawnCoin(100, childTrans[0]); // 후에 수치 넣어서 바꾸기
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnCoin(coin);
    }
}
