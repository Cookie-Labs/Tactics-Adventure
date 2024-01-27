using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Weapon : Card
{
    public int dmg;

    // 자식 컴포넌트
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
        weapon = spawnManager.SpawnWeapon(WeaponType.LongSword, Tier.Common, objTrans); // 후에 생성 설정

        SetDmg(); // 변수 설정

        SetCardName(weapon.data.name);
        SetUI($"<sprite=0>{dmg}");
    }

    public override void DestroyCard()
    {
        spawnManager.DeSpawnWeapon(weapon);
    }

    public override void DoCard()
    {
    }

    private void SetDmg()
    {
        int dmgPer = gameManager.weaponPerDmg;
        int tierID = (int)weapon.data.tier;

        dmg = Random.Range(dmgPer * tierID, dmgPer * (tierID + 1) + 1); // 티어에 따라 무작위 공격력 설정
        dmg = Mathf.Max(1, dmg); // 무기 데미지 0 방지
    }
}