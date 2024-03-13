using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Knight : Player
{
    public override void ActiveSkill()
    {
        if (spawnManager.playerCard.equipWeapon[spawnManager.playerCard.curHand].plus.dmg == 0)
            spawnManager.playerCard.EquipWeapon(0);
        else
            spawnManager.playerCard.UpDmg(1);
    }

    public override void PassiveSkill()
    {
        spawnManager.playerCard.UpDefend(1);
    }
}
