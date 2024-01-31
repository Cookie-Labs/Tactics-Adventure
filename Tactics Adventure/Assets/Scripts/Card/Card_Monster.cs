using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Monster : Card
{
    [Title("�ڽ� ����")]
    public int hp;

    // �ڽ� ������Ʈ
    private Monster monster;

    public override void OnCreatedInPool()
    {
        base.OnCreatedInPool();
    }

    public override void SetCard()
    {
        // ���� ��ȯ
        monster = spawnManager.SpawnMonster_Ran(objTrans);

        // ���� ����
        hp = monster.data.hp;

        SetCardName(monster.data.name);
        SetUI($"<sprite=1>{hp}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnMonster(monster);
    }

    public override void DoCard()
    {
        spawnManager.playerCard.Atk(this);
    }

    public override void Damaged(int _amount)
    {
        hp -= _amount;

        if (hp <= 0)
            Die();

        SetUI($"<sprite=1>{hp}");
    }

    public override void Anim(AnimID id)
    {
    }

    private void Die()
    {
        // ���� ����
        hp = 0;

        spawnManager.ChangeCoinCard(this, monster.data.hp);
    }
}
