using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Monster : Card
{
    [Title("자식 변수")]
    public int hp;

    // 자식 컴포넌트
    private Monster monster;

    public override void SetCard()
    {
        // 몬스터 소환
        monster = spawnManager.SpawnMonster_Ran(objTrans);

        // 변수 설정
        hp = monster.data.hp;

        SetCardName(monster.data.name);
        SetUI($"<sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnMonster(monster);
        DODestroy();
    }

    public override void DoCard()
    {
        spawnManager.playerCard.Atk(this);
    }

    public override void Damaged(int _amount)
    {
        hp = Mathf.Max(0, hp - _amount);

        if (hp <= 0)
            Die();

        DODamaged();
        SetUI($"<sprite=1>{hp}");
    }

    private void Die()
    {
        // 변수 설정
        hp = 0;

        spawnManager.ChangeCoinCard(this, monster.data.hp);
    }
}
