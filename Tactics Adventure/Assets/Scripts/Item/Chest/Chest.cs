using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Chest : MonoBehaviour, IPoolObject
{
    public ChestType type;

    private Animator anim;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
    }

    public virtual void OnGettingFromPool()
    {
        
    }

    public virtual void Open()
    {
        anim.SetTrigger("Open");
    }
}

public enum ChestType { Coin= 0, Consumable, Monster, Relics, Weapon }