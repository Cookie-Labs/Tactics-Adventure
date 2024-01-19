using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Player : Card
{
    private Player player;

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
        player = spawnManager.SpawnPlayer(gameManager.playerType, childTrans[0]);
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
    }
}
