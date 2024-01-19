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
        coin = spawnManager.SpawnCoin(100, childTrans[0]); // �Ŀ� ��ġ �־ �ٲٱ�
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnCoin(coin);
    }
}
