using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Player : Card
{
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
        spawnManager.SpawnPlayer(gameManager.playerType, childTrans[0]);
    }
}
