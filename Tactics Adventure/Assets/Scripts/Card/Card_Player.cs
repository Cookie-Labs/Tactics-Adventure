using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Player : Card
{
    // ����
    public int hp, mp, defend, dmg;

    // �ڽ� ������Ʈ
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
        // �÷��̾� ��ȯ
        player = spawnManager.SpawnPlayer(gameManager.playerType, objTrans);

        // ���� ����
        hp = player.data.hp;
        mp = player.data.mp;
        defend = player.data.defend;
        dmg = 0;

        // ī�� UI ����
        SetCardName(player.data.name);
        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
    }
}
