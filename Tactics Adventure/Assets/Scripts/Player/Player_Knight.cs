using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Knight : Player
{
    public override void ActiveSkill()
    {
        if (string.IsNullOrEmpty(spawnManager.playerCard.weaponData.name))
            spawnManager.playerCard.EquipWeapon(0, 1);
        else
            spawnManager.playerCard.dmg++;
    }

    public override void PassiveSkill()
    {
        spawnManager.playerCard.defend++;
    }
}
