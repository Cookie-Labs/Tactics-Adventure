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
        SetAnim(monster.anim, AnimID.Damaged);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(monster.anim.GetCurrentAnimatorStateInfo(0).length);

        if (hp <= 0)
            Die();
    }

    public IEnumerator Atk(int _amount)
    {
        hp = Mathf.Max(0, hp - _amount);

        DODamaged();
        SetUI($"<sprite=1>{hp}");

        SetAnim(monster.anim, AnimID.Atk);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(monster.anim.GetCurrentAnimatorStateInfo(0).length);

        if (hp <= 0)
            yield return Die();
    }

    private IEnumerator Die()
    {
        // ���� ����
        hp = 0;

        SetAnim(monster.anim, AnimID.Die);
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(monster.anim.GetCurrentAnimatorStateInfo(0).length);

        spawnManager.ChangeCoinCard(this, monster.data.hp);
    }
}
