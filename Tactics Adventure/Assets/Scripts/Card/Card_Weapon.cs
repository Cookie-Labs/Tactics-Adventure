using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Weapon : Card
{
    [Title("자식 변수")]
    public int dmg;

    // 자식 컴포넌트
    [HideInInspector] public Weapon weapon;

    public override void SetCard()
    {
        weapon = spawnManager.SpawnWeapon_Ran(objTrans); // 무기 소환

        SetDmg(); // 변수 설정

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

        dmg = Random.Range(dmgPer * tierID, dmgPer * (tierID + 1)); // 티어에 따라 무작위 공격력 설정
        dmg = Mathf.Max(1, dmg); // 무기 데미지 0 방지
    }
}