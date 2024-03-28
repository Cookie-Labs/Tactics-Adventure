using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Snake : Monster
{
    public override void SetMonster(string name)
    {
        base.SetMonster(name);
        anim.runtimeAnimatorController = SpriteData.Instance.ExportRanController(data.name);
    }
}
