using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Monster : MonoBehaviour, IPoolObject
{
    public MonsterType type;

    private Animator anim;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
    }

    public virtual void OnGettingFromPool()
    {

    }
}

public enum MonsterType { Goblin = 0, Slime, Mushroom, Skeleton, Ghost, FireSoul, IceSoul, PoisonSoul, Monkey}