using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
        relic = spawnManager.SpawnRelic(0, objTrans);

        SetCardName(relic.data.name);
        SetUI(relic.data.explanation);
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnRelic(relic);
    }

    public override void DoCard()
    {
    }

    public override void Damaged(int _amount)
    {
        return;
    }

    public override void Anim(AnimID id)
    {
    }
}