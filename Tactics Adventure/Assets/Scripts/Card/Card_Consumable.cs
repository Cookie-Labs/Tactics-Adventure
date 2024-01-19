using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Consumable : Card
{
    public Consumable consumable;

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
        consumable = spawnManager.SpawnPortion(PortionType.HP, 7, childTrans[0]); // 후에 설정
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnConsumable(consumable);
    }
}