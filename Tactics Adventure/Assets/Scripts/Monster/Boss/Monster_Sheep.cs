using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Sheep : Monster
{
    public override void OnGettingFromPool()
    {
        base.OnGettingFromPool();
        anim.runtimeAnimatorController = SpriteData.Instance.ExportRanController(data.name);
    }
}
