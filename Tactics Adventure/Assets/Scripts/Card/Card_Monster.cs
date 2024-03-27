using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Monster : Card
{
    [Title("�ڽ� ����")]
    public int hp;
    protected int maxHP;

    // �ڽ� ������Ʈ
    [HideInInspector] public Monster monster;

    public override void SetCard()
    {
        // ���� ��ȯ
        monster = spawnManager.SpawnMonster_Ran(objTrans);

        int typeID = (int)monster.data.type;
        // �ִ� ü�� ���� ����: ���� Ÿ�� * �� ��������(����) + ����(0 ~ ���� Ÿ�� + �÷��̾� ����)
        maxHP = typeID * ((int)gameManager.stage + 1) + Random.Range(0, typeID + spawnManager.playerCard.lv);
        hp = maxHP;

        spriteRenderer.sprite = spriteData.ExportMonCard(monster.data.type);
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

        if (hp <= 0)
            yield return Die();
    }

    public IEnumerator Atk(int _amount)
    {
        hp = Mathf.Max(0, hp - _amount);

        DODamaged();
        SetUI($"<sprite=1>{hp}");

        yield return SetAnim(monster.anim, AnimID.Atk);

        if (hp <= 0)
            yield return Die();
    }

    private IEnumerator Die()
    {
        // ���� ����
        hp = 0;

        yield return SetAnim(monster.anim, AnimID.Die);

        spawnManager.playerCard.UpExp(maxHP);
        if (relicManager.CheckRelicCollection(14) && csvManager.luck.Probability(0.01f))
            csvManager.money.EarnMoney(500);
        if (relicManager.CheckRelicCollection(47))
            spawnManager.playerCard.DrainSoul(1);

        spawnManager.ChangeCoinCard(this, maxHP, false);
    }
}
