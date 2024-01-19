using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Trap : MonoBehaviour, IPoolObject
{
    public TrapName trapName;

    protected Animator anim;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
    }

    public virtual void OnGettingFromPool()
    {
    }
}

public enum TrapName { Spike1, Spike2, Spike3, Spike4, Flame, Suriken}