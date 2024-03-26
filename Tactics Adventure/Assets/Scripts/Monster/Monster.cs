using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Monster : MonoBehaviour, IPoolObject
{
    [HideInInspector] public MonsterData data;

    [HideInInspector] public Animator anim;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
    }

    public virtual void OnGettingFromPool()
    {

    }
}

public enum MonsterType { Common = 3, CommonElite = 5, SubBoss = 10, Boss = 15 }