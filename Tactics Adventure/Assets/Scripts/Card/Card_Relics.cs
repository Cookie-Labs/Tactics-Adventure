using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Relics : Card
{
    private Relic relic;

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
        relic = spawnManager.SpawnRelic(0, childTrans[0]);
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnRelic(relic);
    }
}