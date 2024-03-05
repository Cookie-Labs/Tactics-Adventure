using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Card_Weapon : Card
{
    // 자식 컴포넌트
    [HideInInspector] public Weapon weapon;

    public override void SetCard()
    {
        weapon = spawnManager.SpawnWeapon_Ran(objTrans); // 무기 소환

        weapon.data.plus.dmg = csvManager.luck.TierToDmg(weapon.data.tier);

        SetCardName(weapon.data.name);
        SetUI($"<sprite=0>{weapon.data.plus.dmg}");
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
}