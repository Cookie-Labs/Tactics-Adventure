using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Chest : Card
{
    private Chest chest;

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
        chest = spawnManager.SpawnChest(ChestType.Coin, childTrans[0]); // �Ŀ� ���� ����
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnChest(chest);
    }
}