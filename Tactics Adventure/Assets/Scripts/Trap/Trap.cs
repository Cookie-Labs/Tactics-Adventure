using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Trap : MonoBehaviour, IPoolObject
{
    public TrapData data;

    protected Animator anim;

    public virtual void OnCreatedInPool()
    {
        name = name.Replace("(Clone)", "");

        anim = GetComponent<Animator>();
    }

    public virtual void OnGettingFromPool()
    {
    }

    public void RanWait()
    {
        data.wait = Random.Range(2, 5); // 난이도 설정
    }
}

public enum TrapType { Spike1, Spike2, Spike3, Spike4, Flame, Suriken}