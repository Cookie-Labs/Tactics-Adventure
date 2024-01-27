using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Player : Card
{
    // 변수
    public int hp, mp, defend, dmg;

    // 자식 컴포넌트
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
        // 플레이어 소환
        player = spawnManager.SpawnPlayer(gameManager.playerType, objTrans);

        // 변수 설정
        hp = player.data.hp;
        mp = player.data.mp;
        defend = player.data.defend;
        dmg = 0;

        // 카드 UI 설정
        SetCardName(player.data.name);
        SetUI($"<sprite=0>{dmg}  <sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnPlayer(player);
    }
}
