using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Relics : Card
{
    private Relic relic;

    public override void SetCard()
    {
        relic = spawnManager.SpawnRelic_Ran(objTrans); // 유물 소환

        SetCardName(relic.data.name);
        SetUI(relic.data.explanation);
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnRelic(relic);
        DODestroy();
    }

    public override void DoCard()
    {
        spawnManager.playerCard.Move(pos);
    }

    public override void Damaged(int _amount)
    {
        return;
    }
}