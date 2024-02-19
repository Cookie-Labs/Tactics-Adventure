using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Weapon : Card
{
    [Title("�ڽ� ����")]
    public int dmg;

    // �ڽ� ������Ʈ
    [HideInInspector] public Weapon weapon;

    public override void SetCard()
    {
        weapon = spawnManager.SpawnWeapon_Ran(objTrans); // ���� ��ȯ

        SetDmg(); // ���� ����

        SetCardName(weapon.data.name);
        SetUI($"<sprite=0>{dmg}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnWeapon(weapon);
        DODestroy();
    }

    public override IEnumerator DoCard()
    {
        spawnManager.playerCard.EquipWeapon(this);

        yield return spawnManager.playerCard.Move(pos);
    }

    public override IEnumerator Damaged(int _amount)
    {
        yield return new WaitForEndOfFrame();
    }

    private void SetDmg()
    {
        int dmgPer = gameManager.weaponPerDmg;
        int tierID = (int)weapon.data.tier;

        dmg = Random.Range(dmgPer * tierID, dmgPer * (tierID + 1)); // Ƽ� ���� ������ ���ݷ� ����
        dmg = Mathf.Max(1, dmg); // ���� ������ 0 ����
    }
}