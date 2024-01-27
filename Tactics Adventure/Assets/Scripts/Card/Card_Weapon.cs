using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Weapon : Card
{
    public int dmg;

    // �ڽ� ������Ʈ
    private Weapon weapon;

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
        weapon = spawnManager.SpawnWeapon(WeaponType.LongSword, Tier.Common, objTrans); // �Ŀ� ���� ����

        SetDmg(); // ���� ����

        SetCardName(weapon.data.name);
        SetUI($"<sprite=0>{dmg}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnWeapon(weapon);
    }

    private void SetDmg()
    {
        int dmgPer = gameManager.weaponPerDmg;
        int tierID = (int)weapon.data.tier;

        dmg = Random.Range(dmgPer * tierID, dmgPer * (tierID + 1) + 1); // Ƽ� ���� ������ ���ݷ� ����
        dmg = Mathf.Max(1, dmg); // ���� ������ 0 ����
    }
}