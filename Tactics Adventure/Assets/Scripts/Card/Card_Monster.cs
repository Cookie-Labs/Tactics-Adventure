using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Monster : Card
{
    [Title("�ڽ� ����")]
    public int hp;

    // �ڽ� ������Ʈ
    [HideInInspector] public Monster monster;

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
        DODestroy();
    }

    public override IEnumerator DoCard()
    {
        yield return spawnManager.playerCard.Atk(this);
    }

    public override IEnumerator Damaged(int _amount)
    {
        hp = Mathf.Max(0, hp - _amount);

        DODamaged();
        SetUI($"<sprite=1>{hp}");

        // �ǰ� �ִϸ��̼� (������ ����)
        yield return SetAnim(monster.anim, AnimID.Damaged);
        yield return new WaitForSeconds(animTime);

        if (hp <= 0)
            yield return Die();
    }

    public IEnumerator Atk(int _amount)
    {
        hp = Mathf.Max(0, hp - _amount);

        DODamaged();
        SetUI($"<sprite=1>{hp}");

        yield return SetAnim(monster.anim, AnimID.Atk);
        yield return new WaitForSeconds(animTime);

        if (hp <= 0)
            yield return Die();
    }

    private IEnumerator Die()
    {
        // ���� ����
        hp = 0;

        yield return SetAnim(monster.anim, AnimID.Die);
        yield return new WaitForSeconds(animTime);

        spawnManager.ChangeCoinCard(this, monster.data.hp);
    }
}
