using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Monster : Card
{
    private Monster monster;

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
        monster = spawnManager.SpawnRanMonster(childTrans[0]);
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnMonster(monster);
    }
}
