using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Redcode.Pools;

public abstract class Player : MonoBehaviour, IPoolObject
{
    public PlayerType type;

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

public enum PlayerType { Knight, Archer, Ninja, Magician }