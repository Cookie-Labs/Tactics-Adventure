using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card_Weapon : Card
{
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
        weapon = spawnManager.SpawnWeapon(WeaponType.LongSword, Tier.Common, childTrans[0]); // 후에 생성 설정
    }

    public override void DestroyCard()
    {
        
    }
}